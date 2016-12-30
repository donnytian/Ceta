using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ceta.Core
{
    /// <summary>
    /// Wraps configure action.
    /// </summary>
    public class SimpleStartup : IStartup
    {
        private readonly Action<IThreadBuilder> _configureThread;

        /// <summary>
        /// Initializes a new instance of type <see cref="SimpleStartup"/>.
        /// </summary>
        /// <param name="configureThread">Action to configure a <see cref="IThreadBuilder"/> object.</param>
        public SimpleStartup(Action<IThreadBuilder> configureThread)
        {
            _configureThread = configureThread;
        }

        /// <inheritdoc />
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void Configure(IThreadBuilder thread)
        {
            _configureThread(thread);
        }
    }
}
