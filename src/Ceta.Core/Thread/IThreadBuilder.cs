using System;
using System.Collections.Generic;

namespace Ceta.Core
{
    /// <summary>
    /// Provides the mechanisms to configure an test thread's pipeline.
    /// </summary>
    public interface IThreadBuilder
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider Services { get; set; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data between middleware.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Adds a middleware delegate to the thread's test pipeline.
        /// </summary>
        /// <param name="middleware">The middleware delegate.</param>
        /// <returns>
        /// The <see cref="IThreadBuilder"/> itself is returned. This enables you to chain your use statements together.
        /// </returns>
        IThreadBuilder Use(Func<TestDelegate, TestDelegate> middleware);

        /// <summary>
        /// Builds the delegate used by this thread to process testing.
        /// </summary>
        /// <returns>The entry point delegate of the test processing.</returns>
        TestDelegate Build();

        /// <summary>
        /// Creates a new <see cref="IThreadBuilder"/> that shares the <see cref="Properties"/> of this
        /// <see cref="IThreadBuilder"/>.
        /// </summary>
        /// <returns>The new <see cref="IThreadBuilder"/>.</returns>
        IThreadBuilder New();
    }
}
