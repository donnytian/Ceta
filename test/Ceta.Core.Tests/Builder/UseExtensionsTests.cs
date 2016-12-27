using System;
using System.Threading.Tasks;
using Ceta.Core.Builder;
using Xunit;
using _Ceta.TestingFramework.Fakes;

namespace Ceta.Core.Tests.Builder
{
    public class UseExtensionsTests
    {
        [Fact]
        public void UseFunc()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());
            var result = "1";
            Func<ITestContext, Func<Task>, Task> func = (context, next) =>
            {
                result += "2";
                return next();
            };

            // Act
            builder.Use(func)
                .Use(next =>
                {
                    return context =>
                    {
                        result += "3";
                        return next(context);
                    };
                });

            builder.Build().Invoke(new DefaultTestContext()).Wait();

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public void UseMiddleware()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());
            var context = new DefaultTestContext { Items = { ["result"] = "1" } };

            // Act
            builder.Use<MyMiddleware>(2)
                .Use(next =>
                {
                    return ctx =>
                    {
                        ctx.Items["result"] += "3";
                        return next(ctx);
                    };
                });

            builder.Build().Invoke(context).Wait();

            // Assert
            Assert.Equal("123", context.Items["result"]);
        }

        [Fact]
        public void UseMiddleware_NoSuitableConstructor()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());

            // Act
            builder.Use<MyMiddleware>(2, "cause exception parameter");

            // Assert
            var expection = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.StartsWith("a suitable constructor", expection.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UseMiddleware_NoSuitableInvokeMethod()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());

            // Act
            builder.Use<NoSuitableInvoke>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        public class MyMiddleware
        {
            private readonly TestDelegate _next;
            private readonly int _i;

            public MyMiddleware(TestDelegate next) : this(next, 0) { }

            public MyMiddleware(TestDelegate next, int i)
            {
                _next = next;
                _i = i;
            }

            public Task Invoke(ITestContext context)
            {
                context.Items["result"] += _i.ToString();
                return _next(context);
            }
        }

        public class NoSuitableInvoke
        {
            private readonly TestDelegate _next;

            public NoSuitableInvoke(TestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(ITestContext context, string second)
            {
                return _next(context);
            }

            public string Invoke(ITestContext context)
            {
                return "";
            }
        }
    }
}
