using System;

namespace Ceta.Core
{
    using Predicate = Func<ITestContext, bool>;

    /// <summary>
    /// Extension methods for <see cref="IThreadBuilder"/>.
    /// </summary>
    public static class ThreadBuilderUseWhenExtensions
    {
        /// <summary>
        /// Conditionally creates a branch in the thread pipeline that is rejoined to the main pipeline.
        /// </summary>
        /// <param name="thread">The <see cref="IThreadBuilder"/> instance.</param>
        /// <param name="predicate">Invoked with the environment to determine if the branch should be taken.</param>
        /// <param name="configuration">Configures a branch to take.</param>
        /// <returns>The <see cref="IThreadBuilder"/> itself.</returns>
        public static IThreadBuilder UseWhen(this IThreadBuilder thread, Predicate predicate, Action<IThreadBuilder> configuration)
        {
            if (thread == null){throw new ArgumentNullException(nameof(thread));}
            if (predicate == null){throw new ArgumentNullException(nameof(predicate));}
            if (configuration == null){throw new ArgumentNullException(nameof(configuration));}

            // Create and configure the branch builder right away; otherwise,
            // we would end up running our branch after all the components
            // that were subsequently added to the main builder.
            var branchBuilder = thread.New();
            configuration(branchBuilder);

            return thread.Use(main =>
            {
                // This is called only when the main application builder 
                // is built, not per request.
                branchBuilder.Run(main);
                var branch = branchBuilder.Build();

                return context =>
                {
                    if (predicate(context))
                    {
                        return branch(context);
                    }
                    else
                    {
                        return main(context);
                    }
                };
            });
        }
    }
}
