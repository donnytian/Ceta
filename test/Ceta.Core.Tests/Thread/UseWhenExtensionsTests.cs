using System;
using Xunit;
using _Ceta.TestingFramework.Fakes;

namespace Ceta.Core.Tests.Thread
{
    public class UseWhenExtensionsTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BranchPredicate(bool branchCondition)
        {
            // Arrange
            var key = "result";
            var builder = new ThreadBuilder(new FakeServiceProvider());
            var context = new DefaultTestContext { Items = { [key] = "1" } };
            Func<ITestContext, bool> predicate = _ => branchCondition;
            Action<IThreadBuilder> branchConfig = branch =>
            {
                branch.Use((ctx, next) =>
                {
                    ctx.Items[key] += "a";
                    return next();
                })
                .Use((ctx, next) =>
                {
                    ctx.Items[key] += "b";
                    return next();
                });
            };

            // Act
            builder.Use((ctx, next) =>
                {
                    ctx.Items[key] += "2";
                    return next();
                })
                .UseWhen(predicate, branchConfig)
                .Use((ctx, next) =>
                {
                    ctx.Items[key] += "3";
                    return next();
                });

            builder.Build().Invoke(context).Wait();

            // Assert
            Assert.Equal(branchCondition ? "12ab3" : "123", context.Items[key]);
        }
    }
}
