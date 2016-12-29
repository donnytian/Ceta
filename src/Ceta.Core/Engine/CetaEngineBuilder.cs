using System;
using System.Collections.Generic;
using Ceta.Framework.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ceta.Core
{
    /// <summary>
    /// The default implementation for <see cref="ICetaEngineBuilder"/>.
    /// </summary>
    public class CetaEngineBuilder : ICetaEngineBuilder
    {
        private readonly List<Action<IServiceCollection>> _configureServicesDelegates;
        private readonly List<Action<ILoggerFactory>> _configureLoggingDelegates;
        private readonly IConfiguration _config;

        private ILoggerFactory _loggerFactory;
        private bool _engineBuilt;

        /// <summary>
        /// Initializes a new instance for type <see cref="CetaEngineBuilder"/>.
        /// </summary>
        public CetaEngineBuilder()
        {
            _configureServicesDelegates = new List<Action<IServiceCollection>>();
            _configureLoggingDelegates = new List<Action<ILoggerFactory>>();

            _config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ceta_")
                .Build();
        }

        /// <inheritdoc />
        public ICetaEngine Build()
        {
            if (_engineBuilt)
            {
                throw new InvalidOperationException("Builder allows creation only of a single instance.");
            }
            _engineBuilt = true;

            var services = BuildCommonServices();
            var engineServiceProvider = services.Clone().BuildServiceProvider();
            var engine = new CetaEngine(services, engineServiceProvider, _config);

            engine.Initialize();

            return engine;
        }

        /// <inheritdoc />
        public ICetaEngineBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) { throw new ArgumentNullException(nameof(loggerFactory)); }

            _loggerFactory = loggerFactory;
            return this;
        }

        /// <inheritdoc />
        public ICetaEngineBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null) { throw new ArgumentNullException(nameof(configureServices)); }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        /// <inheritdoc />
        public ICetaEngineBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            if (configureLogging == null) { throw new ArgumentNullException(nameof(configureLogging)); }

            _configureLoggingDelegates.Add(configureLogging);
            return this;
        }

        /// <inheritdoc />
        public ICetaEngineBuilder UseSetting(string key, string value)
        {
            _config[key] = value;
            return this;
        }

        /// <inheritdoc />
        public string GetSetting(string key)
        {
            return _config[key];
        }

        private IServiceCollection BuildCommonServices()
        {
            var services = new ServiceCollection();

            // The configured ILoggerFactory is added as a singleton here.
            // AddLogging below will not add an additional one.
            if (_loggerFactory == null)
            {
                _loggerFactory = new LoggerFactory();
                services.AddSingleton(provider => _loggerFactory);
            }
            else
            {
                services.AddSingleton(_loggerFactory);
            }

            foreach (var configureLogging in _configureLoggingDelegates)
            {
                configureLogging(_loggerFactory);
            }

            // This is required to add ILogger of T.
            services.AddLogging();

            services.AddTransient<IThreadBuilderFactory, ThreadBuilderFactory>();

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(services);
            }

            return services;
        }
    }
}
