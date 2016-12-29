using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ceta.Core
{
    /// <summary>
    /// The point to configure the testing.
    /// </summary>
    public interface IStartup
    {
        IServiceProvider ConfigureServices(IServiceCollection services);

        void Configure(IThreadBuilder app);
    }
}
