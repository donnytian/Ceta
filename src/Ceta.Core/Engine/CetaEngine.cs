using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ceta.Core
{
    /// <summary>
    /// The default implementation of <see cref="ICetaEngine"/>.
    /// </summary>
    public class CetaEngine : ICetaEngine
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _engineServiceProvider;
        private readonly IConfiguration _config;
        private TestDelegate _thread;
        private IStartup _startup;
        private IServiceProvider _serviceProvider;
        private ILogger<CetaEngine> _logger;

        /// <summary>
        /// Initializes a new instance for type <see cref="CetaEngine"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object.</param>
        /// <param name="engineServiceProvider">To provide services for engine itself.</param>
        /// <param name="config">The <see cref="IConfiguration"/> object.</param>
        internal CetaEngine(IServiceCollection services, IServiceProvider engineServiceProvider, IConfiguration config)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }
            if (engineServiceProvider == null) { throw new ArgumentNullException(nameof(engineServiceProvider)); }
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            _services = services;
            _engineServiceProvider = engineServiceProvider;
            _config = config;
        }

        /// <inheritdoc />
        public bool IsRunning { get; }

        /// <inheritdoc />
        public IServiceProvider Services
        {
            get
            {
                EnsureServiceProvider();
                return _serviceProvider;
            }
        }

        /// <inheritdoc />
        public Task StartAsync()
        {
            _logger = _serviceProvider.GetRequiredService<ILogger<CetaEngine>>();
            _logger.LogInformation("Ceta engine is starting.");

            var context = new DefaultTestContext(new Dictionary<object, object>(), _config);
            return ProcessThreadAsync(_thread, context);
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            // TODO: Implement Stop logic
        }

        /// <summary>
        /// Initializes the engine.
        /// </summary>
        public void Initialize()
        {
            if (_thread == null)
            {
                _thread = BuildThread();
            }
        }

        private void EnsureServiceProvider()
        {
            if (_serviceProvider == null)
            {
                EnsureStartup();
                _startup.ConfigureServices(_services);
                _serviceProvider = _services.BuildServiceProvider();
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _engineServiceProvider.GetRequiredService<IStartup>();
        }

        private TestDelegate BuildThread()
        {
            try
            {
                EnsureServiceProvider();

                var builderFactory = _serviceProvider.GetRequiredService<IThreadBuilderFactory>();
                var builder = builderFactory.Create();
                builder.Services = _serviceProvider;

                _startup.Configure(builder);

                return builder.Build();
            }
            catch (Exception ex)
            {
                // EnsureServices may have failed due to a missing or throwing Startup class.
                if (_serviceProvider == null)
                {
                    _serviceProvider = _services.BuildServiceProvider();
                }

                // Write errors to standard out so they can be retrieved when not in development mode.
                var message = "Ceta engine startup exception: " + ex;
                Console.Out.WriteLine(message);
                var logger = _serviceProvider.GetRequiredService<ILogger<CetaEngine>>();
                logger.LogCritical(message);

                return context =>
                {
                    context.TestStatus = TestStatus.Failed;
                    return Task.CompletedTask;
                };
            }
        }

        private Task ProcessThreadAsync(TestDelegate threaDelegate, ITestContext context)
        {
            return threaDelegate(context);
        }
    }
}
