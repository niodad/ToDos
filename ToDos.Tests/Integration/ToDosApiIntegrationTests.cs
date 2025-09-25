using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using ToDos.Api;
using ToDos.Domain.Interfaces;
using ToDos.Infrastructure.Data.Entities;
using FluentAssertions;
using Xunit;
using ToDos.Tests.Helpers;

namespace ToDos.Tests.Integration
{
    public class ToDosApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;

        public ToDosApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real repository and add mock
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IRepository<ToDo, Guid>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    
                    _mockRepository = new Mock<IRepository<ToDo, Guid>>();
                    services.AddScoped<IRepository<ToDo, Guid>>(_ => _mockRepository.Object);
                });
            });
        }

        [Fact]
        public async Task CreateToDo_ShouldReturn201_WhenValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            var todo = new
            {
                Name = "Test Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var savedToDo = TestDataHelper.CreateTestToDo();
            _mockRepository.Setup(r => r.SaveAsync(It.IsAny<ToDo>())).ReturnsAsync(savedToDo);

            var json = JsonSerializer.Serialize(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/todos/", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateToDo_ShouldReturn400_WhenInvalidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidTodo = new
            {
                Name = "", // Invalid: empty name
                Email = "invalid-email", // Invalid: not a valid email
                Date = DateTimeOffset.UtcNow,
                Done = false
            };

            var json = JsonSerializer.Serialize(invalidTodo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/todos/", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetToDoById_ShouldReturn200_WhenFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var id = Guid.NewGuid();
            var todo = TestDataHelper.CreateTestToDo(id);
            var todos = new List<ToDo> { todo };

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(todos);

            // Act
            var response = await client.GetAsync($"/api/todos/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetToDoById_ShouldReturn404_WhenNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var id = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(new List<ToDo>());

            // Act
            var response = await client.GetAsync($"/api/todos/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetToDosByEmail_ShouldReturn200_WhenFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var email = "test@example.com";
            var todos = TestDataHelper.CreateTestToDoList(2);

            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(todos);

            // Act
            var response = await client.GetAsync($"/api/todos/user/{email}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateToDo_ShouldReturn200_WhenValidData()
        {
            // Arrange
            var client = _factory.CreateClient();
            var id = Guid.NewGuid();
            var todo = new
            {
                Id = id,
                Name = "Updated Todo",
                Email = "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = true
            };

            var updatedToDo = TestDataHelper.CreateTestToDo(id, "Updated Todo");
            _mockRepository.Setup(r => r.SaveAsync(It.IsAny<ToDo>())).ReturnsAsync(updatedToDo);

            var json = JsonSerializer.Serialize(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"/api/todos/{id}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteToDo_ShouldReturn200_WhenFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var id = Guid.NewGuid();
            var todo = TestDataHelper.CreateTestToDo(id);

            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(todo);

            // Act
            var response = await client.DeleteAsync($"/api/todos/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteToDo_ShouldReturn404_WhenNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var id = Guid.NewGuid();

            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync((ToDo?)null);

            // Act
            var response = await client.DeleteAsync($"/api/todos/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ApiEndpoints_ShouldReturn401_WhenNoApiKey()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act & Assert
            var createResponse = await client.PostAsync("/api/todos/", new StringContent("{}", Encoding.UTF8, "application/json"));
            createResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var getResponse = await client.GetAsync("/api/todos/test-id");
            getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var updateResponse = await client.PutAsync("/api/todos/test-id", new StringContent("{}", Encoding.UTF8, "application/json"));
            updateResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var deleteResponse = await client.DeleteAsync("/api/todos/test-id");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task SwaggerEndpoints_ShouldBeAccessible_WithoutApiKey()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var swaggerResponse = await client.GetAsync("/swagger/index.html");
            var apiDocsResponse = await client.GetAsync("/api-docs/v1/swagger.json");

            // Assert
            // These might return 404 if not configured, but should not return 401
            swaggerResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            apiDocsResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }
    }
}
