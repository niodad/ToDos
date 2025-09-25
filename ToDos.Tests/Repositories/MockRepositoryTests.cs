using Microsoft.Extensions.Logging;
using Moq;
using ToDos.Domain.Interfaces;
using ToDos.Infrastructure.Data.Entities;
using FluentAssertions;
using Xunit;
using ToDos.Tests.Helpers;

namespace ToDos.Tests.Repositories
{
    public class MockRepositoryTests
    {
        private readonly Mock<IRepository<ToDo, Guid>> _mockRepository;

        public MockRepositoryTests()
        {
            _mockRepository = new Mock<IRepository<ToDo, Guid>>();
        }

        [Fact]
        public async Task SaveAsync_ShouldReturnSavedToDo()
        {
            // Arrange
            var todo = TestDataHelper.CreateTestToDo();
            _mockRepository.Setup(r => r.SaveAsync(todo)).ReturnsAsync(todo);

            // Act
            var result = await _mockRepository.Object.SaveAsync(todo);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(todo);
            _mockRepository.Verify(r => r.SaveAsync(todo), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnDeletedToDo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todo = TestDataHelper.CreateTestToDo(id);
            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(todo);

            // Act
            var result = await _mockRepository.Object.DeleteAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(todo);
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnMatchingToDos()
        {
            // Arrange
            var todos = TestDataHelper.CreateTestToDoList(3);
            _mockRepository.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()))
                .ReturnsAsync(todos);

            // Act
            var result = await _mockRepository.Object.GetAsync(t => t.Done == false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(todos);
            _mockRepository.Verify(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ToDo, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var todo = TestDataHelper.CreateTestToDo();
            _mockRepository.Setup(r => r.SaveAsync(todo))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _mockRepository.Object.SaveAsync(todo));
            _mockRepository.Verify(r => r.SaveAsync(todo), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenToDoNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync((ToDo?)null);

            // Act
            var result = await _mockRepository.Object.DeleteAsync(id);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
