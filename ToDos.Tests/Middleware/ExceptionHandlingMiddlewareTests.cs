using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using ToDos.Api.Middleware;
using ToDos.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ToDos.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
        private readonly ExceptionHandlingMiddleware _middleware;

        public ExceptionHandlingMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Invoke_ShouldCallNext_WhenNoException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockNext.Setup(n => n(context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_ShouldHandleNotFoundException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new NotFoundException();
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("NotFound");
            responseBody.Should().Contain("The requested resource could not be found");
        }

        [Fact]
        public async Task Invoke_ShouldHandleArgumentException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new ArgumentException("Invalid argument");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("BadRequest");
            responseBody.Should().Contain("Invalid argument");
        }

        [Fact]
        public async Task Invoke_ShouldHandleArgumentNullException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new ArgumentNullException("parameter");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("BadRequest");
            responseBody.Should().Contain("Required parameter is missing or null");
        }

        [Fact]
        public async Task Invoke_ShouldHandleUnauthorizedAccessException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new UnauthorizedAccessException("Access denied");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("Unauthorized");
            responseBody.Should().Contain("Access denied");
        }

        [Fact]
        public async Task Invoke_ShouldHandleTimeoutException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new TimeoutException("Request timed out");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.RequestTimeout);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("RequestTimeout");
            responseBody.Should().Contain("The request timed out");
        }

        [Fact]
        public async Task Invoke_ShouldHandleGenericException_WhenThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new Exception("Unexpected error");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Be("application/json");
            
            var responseBody = await GetResponseBody(context.Response.Body);
            responseBody.Should().Contain("InternalServerError");
            responseBody.Should().Contain("An unexpected error occurred");
        }

        [Fact]
        public async Task Invoke_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test exception");
            _mockNext.Setup(n => n(context)).ThrowsAsync(exception);

            // Act
            await _middleware.Invoke(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unhandled exception occurred")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private async Task<string> GetResponseBody(Stream body)
        {
            body.Position = 0;
            using var reader = new StreamReader(body);
            return await reader.ReadToEndAsync();
        }
    }
}
