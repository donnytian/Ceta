using System;
using System.Threading.Tasks;

namespace Ceta.Core
{
    /// <summary>
    /// Represents a load test engine to overall control and monitor the process.
    /// </summary>
    public interface ICetaEngine
    {
        /// <summary>
        /// Indicates whether a test is still running or not.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The <see cref="IServiceProvider"/> for the test.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Asynchronously starts the automation process.
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// Stops the test.
        /// </summary>
        void Stop();
    }
}
