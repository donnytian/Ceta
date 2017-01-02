using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ceta.Core.Tests.Engine
{
    public class BuilderTests
    {
        [Fact]
        public void GetEnvironmentVariable()
        {
            // Arrange
            var engineKey = "test1";
            var envKey = "ceta_" + engineKey;
            var variable = "my string";
            Environment.SetEnvironmentVariable(envKey, variable);
            var builder = new CetaEngineBuilder();

            // Act
            var result = builder.GetSetting(engineKey);

            // Assert
            Assert.Equal(variable, result);
        }

        [Fact]
        public void UseSetting()
        {
            // Arrange
            var engineKey = "test1";
            var variable = "my string1";
            var builder = new CetaEngineBuilder();

            // Act
            builder.UseSetting(engineKey, variable);
            var result = builder.GetSetting(engineKey);

            // Assert
            Assert.Equal(variable, result);
        }

        [Fact]
        public void LoggerFactory()
        {
            // Arrange
            var factory = new LoggerFactory();
            var builder = new CetaEngineBuilder();

            // Act
            builder.UseLoggerFactory(factory);
            var result = builder.Build().Services.GetService(typeof(ILoggerFactory));

            // Assert
            Assert.Equal(factory, result);
        }

        [Fact]
        public void ConfigureLogging()
        {
            // Arrange
            var test = 1;
            var builder = new CetaEngineBuilder();

            // Act
            builder.ConfigureLogging(factory => test++).Build();

            // Assert
            Assert.Equal(2, test);
        }

        [Fact]
        public void ConfigureServices()
        {
            // Arrange
            var test = 1;
            var builder = new CetaEngineBuilder();

            // Act
            builder.ConfigureServices(services => test++).Build();

            // Assert
            Assert.Equal(2, test);
        }
    }
}
