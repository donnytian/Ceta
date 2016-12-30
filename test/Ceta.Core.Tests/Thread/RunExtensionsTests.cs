using System.Threading.Tasks;
using Xunit;
using _Ceta.TestingFramework.Fakes;

namespace Ceta.Core.Tests.Thread
{
    public class RunExtensionsTests
    {
        [Fact]
        public void RunAsTerminal()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());
            var result = "1";

            // Act
            builder.Use(next =>
            {
                return context =>
                {
                    result += "2";
                    return next(context);
                };
            });
            builder.Run(context =>
            {
                result += "3";
                return Task.CompletedTask;
            });
            builder.Use(next =>
            {
                return context =>
                {
                    result += "4";
                    return next(context);
                };
            });

            builder.Build().Invoke(new DefaultTestContext()).Wait();

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public void SecondRunShouldNotBeCalled()
        {
            // Arrange
            var builder = new ThreadBuilder(new FakeServiceProvider());
            var result = "1";

            // Act
            builder.Use(next =>
            {
                return context =>
                {
                    result += "2";
                    return next(context);
                };
            });
            builder.Run(context =>
            {
                result += "3";
                return Task.CompletedTask;
            });
            builder.Run(context =>
            {
                result += "4";
                return Task.CompletedTask;
            });

            builder.Build().Invoke(new DefaultTestContext()).Wait();

            // Assert
            Assert.Equal("123", result);
        }
    }
}
