using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using StructureMap;
using Xunit;

namespace MinimalTest
{
    public class LoggingStructureMapTests : IDisposable
    {
        private Mock<ILogger> _mockLogger;
        private Mock<ILoggerProvider> _mockLoggerProvider;
        private ServiceCollection _services;

        public LoggingStructureMapTests()
        {
            _mockLogger = new Mock<ILogger>(MockBehavior.Loose);
            _mockLoggerProvider = new Mock<ILoggerProvider>(MockBehavior.Loose);
            _mockLoggerProvider.Setup(p => p.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _services = new ServiceCollection();
            _services.AddLogging(b => { b.AddProvider(_mockLoggerProvider.Object); });
        }

        [Fact]
        public void WithoutStructureMap_LogCalled()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Foo>>();

            logger.LogCritical("Hello");

            _mockLogger.Verify(logExpressionToVerify, Times.Once);
        }

        [Fact]
        public void WithStructureMap_LogNeverCalled()
        {
            var container = new Container(c => { c.Populate(_services); });
            var serviceProvider = container.GetInstance<IServiceProvider>();
            var logger = serviceProvider.GetService<ILogger<Foo>>();

            logger.LogCritical("Hello");

            _mockLogger.Verify(logExpressionToVerify, Times.Never); /*SAD FACE*/
        }

        public void Dispose()
        {
            _mockLogger = null;
            _mockLoggerProvider = null;
            _services = null;
        }

        private Expression<Action<ILogger>> logExpressionToVerify = l => l.Log(
            LogLevel.Critical,
            0,
            It.IsAny<FormattedLogValues>(),
            null,
            It.IsAny<Func<object, Exception, string>>()
        );
    }

    public class Foo { }
}