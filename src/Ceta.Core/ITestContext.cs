using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Ceta.Core
{
    /// <summary>
    /// Encapsulates all test thread specific information.
    /// </summary>
    public interface ITestContext
    {
        /// <summary>
        /// Gets system configuration values.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this thread.
        /// </summary>
        IDictionary<object, object> Items { get; }

        /// <summary>
        /// Gets or sets the test status.
        /// </summary>
        TestStatus TestStatus { get; set; }
    }
}
