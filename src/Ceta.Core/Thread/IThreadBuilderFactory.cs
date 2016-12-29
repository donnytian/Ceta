namespace Ceta.Core
{
    /// <summary>
    /// Factory for <see cref="IThreadBuilder"/> object.
    /// </summary>
    public interface IThreadBuilderFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IThreadBuilder"/>.
        /// </summary>
        /// <returns>New instance of <see cref="IThreadBuilder"/>.</returns>
        IThreadBuilder Create();
    }
}
