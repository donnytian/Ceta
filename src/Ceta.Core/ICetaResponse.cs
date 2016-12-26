using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ceta.Core
{
    /// <summary>
    /// Represents a response received from a specific <see cref="ICetaRequest"/>.
    /// </summary>
    public interface ICetaResponse
    {
        /// <summary>
        /// Gets the request for the current response.
        /// </summary>
        ICetaRequest Request { get; }
    }
}
