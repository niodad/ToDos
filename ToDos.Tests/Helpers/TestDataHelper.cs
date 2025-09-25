using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static ToDo CreateTestToDo(Guid? id = null, string? name = null, string? email = null)
        {
            return new ToDo
            {
                Id = id ?? Guid.NewGuid(),
                Name = name ?? "Test Todo",
                Email = email ?? "test@example.com",
                Date = DateTimeOffset.UtcNow,
                Done = false
            };
        }

        public static List<ToDo> CreateTestToDoList(int count = 3)
        {
            var todos = new List<ToDo>();
            for (int i = 0; i < count; i++)
            {
                todos.Add(CreateTestToDo(name: $"Test Todo {i + 1}"));
            }
            return todos;
        }
    }
}
