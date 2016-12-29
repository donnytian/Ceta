using System;

namespace Ceta.Core
{
    /// <summary>
    /// Default implementation for type <see cref="IThreadBuilderFactory"/>.
    /// </summary>
    public class ThreadBuilderFactory : IThreadBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of type <see cref="ThreadBuilderFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> object.</param>
        public ThreadBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public IThreadBuilder Create()
        {
            return new ThreadBuilder(_serviceProvider);
        }
    }
}
