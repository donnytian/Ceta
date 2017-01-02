using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ceta.Core.Tests.Engine
{
    public class BuilderExtensionTests
    {
        [Fact]
        public void ConfigureThread()
        {
            // Arrange
            var result = 0;
            var builder = new CetaEngineBuilder();

            // Act
            builder.Configure(thread => { result++; }).Build();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async void UseStartupType()
        {
            // Arrange
            var builder = new CetaEngineBuilder();

            // Act
            var engine = builder.UseStartup(typeof(MyStartup)).Build();
            await engine.StartAsync();

            // Assert
            Assert.Equal(MySingletonService, engine.Services.GetService(typeof(MyService)));
            Assert.Equal("MyStarup", builder.GetSetting("MyMiddleware"));
        }

        private class MyMiddleware
        {
            private TestDelegate _next;
            private string _parameter;
            public MyMiddleware(TestDelegate next, string parameter)
            {
                _next = next;
                _parameter = parameter;
            }

            public Task Invoke(ITestContext context, ILoggerFactory loggerFactory)
            {
                context.Configuration["MyMiddleware"] = _parameter;
                loggerFactory.CreateLogger("ceta").LogInformation("Invoked in MyMiddleware.");
                return _next(context);
            }
        }

        private static readonly MyService MySingletonService = new MyService();
        private class MyService
        {
            
        }

        private class MyStartup : IStartup
        {
            /// <inheritdoc />
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(MySingletonService);
            }

            /// <inheritdoc />
            public void Configure(IThreadBuilder thread)
            {
                thread.Use<MyMiddleware>("MyStarup");
            }
        }
    }
}
