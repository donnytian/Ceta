using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ceta.Core
{
    /// <summary>
    /// Represents a request to be tested in a chained test thread.
    /// </summary>
    public interface ICetaRequest
    {
        /// <summary>
        /// Gets the response asynchronously.
        /// </summary>
        /// <param name="context">The <see cref="ITestContext"/> for the request.</param>
        /// <returns>A task that represents the request sending.</returns>
        Task<ICetaResponse> GetResponseAsync(ITestContext context);
    }
}
