using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ceta.Core
{
    /// <summary>
    /// The point to configure the testing.
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Configures the service collection. This is the inject point for DI.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> object.</param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Configures thread builder to build a custom thread.
        /// </summary>
        /// <param name="thread">The <see cref="IThreadBuilder"/> object.</param>
        void Configure(IThreadBuilder thread);
    }
}
