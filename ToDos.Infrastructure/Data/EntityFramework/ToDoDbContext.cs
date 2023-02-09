using Microsoft.EntityFrameworkCore;
using ToDos.Infrastructure.Data.Entities;

namespace ToDos.Infrastructure.Data.EntityFramework
{
    public class ToDosDbContext : DbContext
    {
        public ToDosDbContext(DbContextOptions<ToDosDbContext> options)
        : base(options) { }

        public DbSet<ToDo> Items { get; set; } = default!;

    }
}