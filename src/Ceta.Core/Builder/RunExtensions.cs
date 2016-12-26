using System;

namespace Ceta.Core.Builder
{
    /// <summary>
    /// Extension methods for adding terminal middleware.
    /// </summary>
    public static class RunExtensions
    {
        /// <summary>
        /// Adds a terminal middleware delegate to the thread's pipeline.
        /// </summary>
        /// <param name="thread">The <see cref="IThreadBuilder"/> instance.</param>
        /// <param name="handler">A delegate that handles the request.</param>
        public static void Run(this IThreadBuilder thread, TestDelegate handler)
        {
            if (thread == null) { throw new ArgumentNullException(nameof(thread)); }

            if (handler == null) { throw new ArgumentNullException(nameof(handler)); }

            thread.Use(ignoredDelegate => handler);
        }
    }
}
