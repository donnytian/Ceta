using System.Threading.Tasks;

namespace Ceta.Core
{
    /// <summary>
    /// A function that can perform, examine or monitor an automation test.
    /// </summary>
    /// <param name="context">The <see cref="ITestContext"/> for the test thread.</param>
    /// <returns>A task that represents the completion of test processing.</returns>
    public delegate Task TestDelegate(ITestContext context);
}
