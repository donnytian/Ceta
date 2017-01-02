using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Ceta.Core
{
    /// <summary>
    /// Extensions for type of <see cref="ICetaEngineBuilder"/>.
    /// </summary>
    public static class CetaEngineBuilderExtensions
    {
        private static readonly Type StartupType = typeof(IStartup);

        /// <summary>
        /// Specify the startup method to be used to configure the thread.
        /// </summary>
        /// <param name="builder">The <see cref="ICetaEngineBuilder"/> to configure.</param>
        /// <param name="configureThread">The delegate that configures the <see cref="IThreadBuilder"/>.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/>.</returns>
        public static ICetaEngineBuilder Configure(this ICetaEngineBuilder builder, Action<IThreadBuilder> configureThread)
        {
            if (configureThread == null) { throw new ArgumentNullException(nameof(configureThread)); }

            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartup>(new SimpleStartup(configureThread));
            });
        }

        /// <summary>
        /// Specify the startup type to be used by the engine.
        /// </summary>
        /// <param name="builder">The <see cref="ICetaEngineBuilder"/> to configure.</param>
        /// <param name="startupType">The <see cref="Type"/> to be used.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/>.</returns>
        public static ICetaEngineBuilder UseStartup(this ICetaEngineBuilder builder, Type startupType)
        {
            if (startupType == null) { throw new ArgumentNullException(nameof(startupType)); }

            return builder.ConfigureServices(services =>
            {
                if (StartupType.GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                {
                    services.AddSingleton(StartupType, startupType);
                }
                else
                {
                    throw new ArgumentException($"The given type '{startupType}' does not implemented '{StartupType}'.");
                }
            });
        }

        /// <summary>
        /// Specify the startup type to be used by the engine.
        /// </summary>
        /// <param name="builder">The <see cref="ICetaEngineBuilder"/> to configure.</param>
        /// <typeparam name ="TStartup">The type containing the startup methods for the application.</typeparam>
        /// <returns>The <see cref="ICetaEngineBuilder"/>.</returns>
        public static ICetaEngineBuilder UseStartup<TStartup>(this ICetaEngineBuilder builder) where TStartup : IStartup
        {
            return builder.UseStartup(typeof(TStartup));
        }
    }
}
