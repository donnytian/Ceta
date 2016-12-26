using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ceta.Core
{
    /// <summary>
    /// Encapsulates all test thread specific information.
    /// </summary>
    public interface ITestContext
    {
        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this thread.
        /// </summary>
        IDictionary<object, object> Items { get; }

        /// <summary>
        /// Gets the test status.
        /// </summary>
        TestStatus TestStatus { get; }
    }
}
