using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;

namespace Ceta.Core.Builder
{
    /// <summary>
    /// Extension methods for adding middleware.
    /// </summary>
    public static class UseExtensions
    {
        private const string InvokeMethodName = "Invoke";

        private static readonly MethodInfo GetServiceInfo = typeof(UseExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// Adds a middleware delegate defined in-line to the application's request pipeline.
        /// </summary>
        /// <param name="thread">The <see cref="IThreadBuilder"/> instance.</param>
        /// <param name="middleware">A function that handles the request or calls the given next function.</param>
        /// <returns>The <see cref="IThreadBuilder"/> instance.</returns>
        public static IThreadBuilder Use(this IThreadBuilder thread, Func<ITestContext, Func<Task>, Task> middleware)
        {
            return thread.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        /// <summary>
        /// Adds a middleware type to the application's request pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware">The middleware type.</typeparam>
        /// <param name="thread">The <see cref="IThreadBuilder"/> instance.</param>
        /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
        /// <returns>The <see cref="IThreadBuilder"/> instance.</returns>
        public static IThreadBuilder Use<TMiddleware>(this IThreadBuilder thread, params object[] args)
        {
            return thread.Use(typeof(TMiddleware), args);
        }

        /// <summary>
        /// Adds a middleware type to the application's request pipeline.
        /// </summary>
        /// <param name="thread">The <see cref="IThreadBuilder"/> instance.</param>
        /// <param name="middleware">The middleware type.</param>
        /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
        /// <returns>The <see cref="IThreadBuilder"/> instance.</returns>
        public static IThreadBuilder Use(this IThreadBuilder thread, Type middleware, params object[] args)
        {
            var services = thread.Services;

            return thread.Use(next =>
            {
                var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                var invokeMethods = methods.Where(m => string.Equals(m.Name, InvokeMethodName, StringComparison.Ordinal)).ToArray();
                if (invokeMethods.Length > 1)
                {
                    throw new InvalidOperationException($"Multiple '{InvokeMethodName}' methods found!");
                }

                if (invokeMethods.Length == 0)
                {
                    throw new InvalidOperationException($"No '{InvokeMethodName}' method found!");
                }

                var methodinfo = invokeMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                {
                    throw new InvalidOperationException($"'{InvokeMethodName}' does not return a type of '{nameof(Task)}'.");
                }

                var parameters = methodinfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(ITestContext))
                {
                    throw new InvalidOperationException($"The '{InvokeMethodName}' method's first argument must be of type '{nameof(ITestContext)}'.");
                }

                var ctorArgs = new[] {next}.Concat(args).ToArray();
                var instance = ActivatorUtilities.CreateInstance(thread.Services, middleware, ctorArgs);
                if (parameters.Length == 1)
                {
                    return (TestDelegate)methodinfo.CreateDelegate(typeof(TestDelegate), instance);
                }

                var factory = Compile<object>(methodinfo, parameters);

                return context =>
                {
                    if (services == null)
                    {
                        throw new InvalidOperationException($"'{nameof(IServiceProvider)}' is not available.");
                    }

                    return factory(instance, context, services);
                };
            });
        }

        private static Func<T, ITestContext, IServiceProvider, Task> Compile<T>(MethodInfo methodinfo, ParameterInfo[] parameters)
        {
            // If we call something like
            // 
            // public class Middleware
            // {
            //    public Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
            //    {
            //         
            //    }
            // }
            //

            // We'll end up with something like this:
            //   Generic version:
            //
            //   Task Invoke(Middleware instance, HttpContext httpContext, IServiceprovider provider)
            //   {
            //      return instance.Invoke(httpContext, (ILoggerFactory)UseExtensions.GetService(provider, typeof(ILoggerFactory));
            //   }

            //   Non generic version:
            //
            //   Task Invoke(object instance, HttpContext httpContext, IServiceprovider provider)
            //   {
            //      return ((Middleware)instance).Invoke(httpContext, (ILoggerFactory)UseExtensions.GetService(provider, typeof(ILoggerFactory));
            //   }

            var middleware = typeof(T);

            var instanceArg = Expression.Parameter(middleware, "middleware");
            var contextArg = Expression.Parameter(typeof(ITestContext), "context");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = contextArg;
            for (int i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    throw new NotSupportedException($"The '{InvokeMethodName}' method must not have ref or out parameters.");
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodinfo.DeclaringType, typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodinfo.DeclaringType != typeof(T))
            {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodinfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodinfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, ITestContext, IServiceProvider, Task>>(body, instanceArg, contextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider services, Type type, Type middleware)
        {
            var service = services.GetService(type);
            if (service == null)
            {
                throw new InvalidOperationException($"Unable to resolve service for type '{type}' while attempting to Invoke middleware '{middleware}'.");
            }

            return service;
        }
    }
}
