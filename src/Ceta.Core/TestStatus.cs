namespace Ceta.Core
{
    /// <summary>
    /// Represents the status for the automation test.
    /// </summary>
    public enum TestStatus
    {
        NotStarted          = 0,
        Initializing        = 10,
        Warmup              = 20,
        InProgress          = 30,
        Paused              = 40,
        Cooldown            = 50,
        Succeeded           = 60,
        Failed              = 70,
        Terminated          = 80,
        Completed           = 90
    }
}
