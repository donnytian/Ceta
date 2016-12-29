using System.Collections.Generic;

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
        public DefaultTestContext(IDictionary<object, object> items)
        {
            Items = items;
            TestStatus = TestStatus.NotStarted;
        }

        /// <summary>
        /// Initializes a new instance of the type <see cref="DefaultTestContext"/>.
        /// </summary>
        public DefaultTestContext() :this(new Dictionary<object, object>())
        {
        }

        /// <inheritdoc />
        public IDictionary<object, object> Items { get; protected set; }

        /// <inheritdoc />
        public TestStatus TestStatus { get; set; }
    }
}
