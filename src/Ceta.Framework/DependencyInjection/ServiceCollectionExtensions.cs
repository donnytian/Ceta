using Microsoft.Extensions.DependencyInjection;

namespace Ceta.Framework.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a new collection with same service references.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> object.</param>
        /// <returns>The new <see cref="IServiceCollection"/> object.</returns>
        public static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in serviceCollection)
            {
                clone.Add(service);
            }

            return clone;
        }
    }
}
