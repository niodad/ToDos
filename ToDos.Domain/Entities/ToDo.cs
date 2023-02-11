namespace ToDos.Domain.Entities
{
    public class ToDo
    {
        public string? Name { get; set; }
        public string Email { get; set; } = default!;
        public DateTimeOffset Date { get; set; } = default!;
        public bool Done { get; set; }
    }
}
