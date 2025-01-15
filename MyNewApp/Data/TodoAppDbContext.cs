using Microsoft.EntityFrameworkCore;

public class TodoAppDbContext : DbContext
{
    public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Todo>? Todos { get; set; }

    public virtual DbSet<User>? Users { get; set; }
}
