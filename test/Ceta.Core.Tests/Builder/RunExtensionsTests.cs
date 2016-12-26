using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ceta.Core.Builder;
using Microsoft.Extensions.Internal;
using Xunit;
using _Ceta.TestingFramework.Fakes;

namespace Ceta.Core.Tests.Builder
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
                return TaskCache.CompletedTask;
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
    }
}
