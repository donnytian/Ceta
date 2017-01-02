using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Ceta.Core
{
    /// <summary>
    /// The default implementation of interface <see cref="ITestContext"/>.
    /// </summary>
    public class DefaultTestContext : ITestContext
    {
        /// <summary>
        /// Initializes a new instance of the type <see cref="DefaultTestContext"/>.
        /// </summary>
        /// <param name="items">The <see cref="Items"/> object.</param>
        /// <param name="configuration">The <see cref="Configuration"/> object.</param>
        public DefaultTestContext(IDictionary<object, object> items, IConfiguration configuration)
        {
            Items = items;
            Configuration = configuration;
            TestStatus = TestStatus.NotStarted;
        }

        /// <summary>
        /// Initializes a new instance of the type <see cref="DefaultTestContext"/>.
        /// </summary>
        public DefaultTestContext() :this(new Dictionary<object, object>(), new ConfigurationBuilder().Build())
        {
        }

        /// <inheritdoc />
        public IConfiguration Configuration { get; protected set; }

        /// <inheritdoc />
        public IDictionary<object, object> Items { get; protected set; }

        /// <inheritdoc />
        public TestStatus TestStatus { get; set; }
    }
}
