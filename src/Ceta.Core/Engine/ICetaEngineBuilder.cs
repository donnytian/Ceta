using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ceta.Core
{
    /// <summary>
    /// A builder for <see cref="ICetaEngine"/>.
    /// </summary>
    public interface ICetaEngineBuilder
    {
        /// <summary>
        /// Builds an <see cref="ICetaEngine"/> which runs a automation test.
        /// </summary>
        ICetaEngine Build();

        /// <summary>
        /// Specify the <see cref="ILoggerFactory"/> to be used by the automation test.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to be used.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/> itself.</returns>
        ICetaEngineBuilder UseLoggerFactory(ILoggerFactory loggerFactory);

        /// <summary>
        /// Specify the delegate that is used to configure the services.
        /// </summary>
        /// <param name="configureServices">The delegate that configures the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/> itself.</returns>
        ICetaEngineBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// Adds a delegate for configuring the provided <see cref="ILoggerFactory"/>. This may be called multiple times.
        /// </summary>
        /// <param name="configureLogging">The delegate that configures the <see cref="ILoggerFactory"/>.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/> itself.</returns>
        ICetaEngineBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);

        /// <summary>
        /// Add or replace a setting in the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The <see cref="ICetaEngineBuilder"/> itself.</returns>
        ICetaEngineBuilder UseSetting(string key, string value);

        /// <summary>
        /// Get the setting value from the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        string GetSetting(string key);
    }
}
