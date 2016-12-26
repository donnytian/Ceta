using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;

namespace Ceta.Core.Builder
{
    /// <summary>
    /// A default implementation of <see cref="IThreadBuilder"/>.
    /// </summary>
    public class ThreadBuilder : IThreadBuilder
    {
        private readonly IList<Func<TestDelegate, TestDelegate>> _components = new List<Func<TestDelegate, TestDelegate>>();

        /// <summary>
        /// Initializes a new instance of the type <see cref="IThreadBuilder"/>.
        /// </summary>
        /// <param name="properties">The <see cref="Properties"/> object.</param>
        /// <param name="services">The <see cref="Services"/> object.</param>
        internal ThreadBuilder(IDictionary<string, object> properties, IServiceProvider services)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            Properties = properties;
            Services = services;
        }

        /// <summary>
        /// Initializes a new instance of the type <see cref="IThreadBuilder"/>.
        /// </summary>
        /// <param name="services">The service provider object.</param>
        public ThreadBuilder(IServiceProvider services) : this(new Dictionary<string, object>(), services)
        {
        }

        /// <inheritdoc />
        public IServiceProvider Services
        {
            get
            {
                return GetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices);
            }

            set
            {
                SetProperty(Constants.BuilderProperties.ApplicationServices, value);
            }
        }

        /// <inheritdoc />
        public IDictionary<string, object> Properties { get; }

        /// <inheritdoc />
        public IThreadBuilder Use(Func<TestDelegate, TestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        /// <inheritdoc />
        public TestDelegate Build()
        {
            TestDelegate app = context =>
            {
                return TaskCache.CompletedTask;
            };

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            return app;
        }

        /// <inheritdoc />
        public IThreadBuilder New()
        {
            return new ThreadBuilder(Properties, null);
        }

        private T GetProperty<T>(string key)
        {
            object value;
            return Properties.TryGetValue(key, out value) ? (T)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }
    }
}
