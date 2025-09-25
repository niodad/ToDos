using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ToDos.Api.Middleware;
using ToDos.Api.Services;
using FluentAssertions;
using Xunit;

namespace ToDos.Tests.Middleware
{
    public class ApiKeyAuthenticationMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<ApiKeyAuthenticationMiddleware>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ApiKeyAuthenticationService> _mockAuthService;
        private readonly ApiKeyAuthenticationMiddleware _middleware;

        public ApiKeyAuthenticationMiddlewareTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<ApiKeyAuthenticationMiddleware>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockAuthService = new Mock<ApiKeyAuthenticationService>(_mockConfiguration.Object, _mockLogger.Object);
            _middleware = new ApiKeyAuthenticationMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldCallNext_WhenValidApiKeyInHeader()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-Key", "valid-api-key");
            context.Request.Path = "/api/todos";
            
            _mockAuthService.Setup(a => a.ValidateApiKey("valid-api-key")).Returns(true);
            _mockAuthService.Setup(a => a.CreatePrincipal("valid-api-key")).Returns(new System.Security.Claims.ClaimsPrincipal());

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
            _mockAuthService.Verify(a => a.ValidateApiKey("valid-api-key"), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldCallNext_WhenValidApiKeyInQuery()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?apiKey=valid-api-key");
            context.Request.Path = "/api/todos";
            
            _mockAuthService.Setup(a => a.ValidateApiKey("valid-api-key")).Returns(true);
            _mockAuthService.Setup(a => a.CreatePrincipal("valid-api-key")).Returns(new System.Security.Claims.ClaimsPrincipal());

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
            _mockAuthService.Verify(a => a.ValidateApiKey("valid-api-key"), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn401_WhenNoApiKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/todos";

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            context.Response.StatusCode.Should().Be(401);
            _mockNext.Verify(n => n(context), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturn401_WhenInvalidApiKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-Key", "invalid-api-key");
            context.Request.Path = "/api/todos";
            
            _mockAuthService.Setup(a => a.ValidateApiKey("invalid-api-key")).Returns(false);

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            context.Response.StatusCode.Should().Be(401);
            _mockNext.Verify(n => n(context), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSkipAuthentication_ForSwaggerEndpoints()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/swagger/index.html";

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
            _mockAuthService.Verify(a => a.ValidateApiKey(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSkipAuthentication_ForApiDocsEndpoints()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api-docs/v1/swagger.json";

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
            _mockAuthService.Verify(a => a.ValidateApiKey(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSkipAuthentication_ForHealthEndpoints()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/health";

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
            _mockAuthService.Verify(a => a.ValidateApiKey(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task InvokeAsync_ShouldSetUserPrincipal_WhenValidApiKey()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("X-API-Key", "valid-api-key");
            context.Request.Path = "/api/todos";
            
            var principal = new System.Security.Claims.ClaimsPrincipal();
            _mockAuthService.Setup(a => a.ValidateApiKey("valid-api-key")).Returns(true);
            _mockAuthService.Setup(a => a.CreatePrincipal("valid-api-key")).Returns(principal);

            // Act
            await _middleware.InvokeAsync(context, _mockAuthService.Object);

            // Assert
            context.User.Should().Be(principal);
            _mockAuthService.Verify(a => a.CreatePrincipal("valid-api-key"), Times.Once);
        }
    }
}
