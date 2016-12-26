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
        /// Initializes and starts the test.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the test.
        /// </summary>
        void Stop();
    }
}
