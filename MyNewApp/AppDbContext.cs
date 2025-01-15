using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // This represents the table in the database
    public virtual DbSet<TodoItem> Todos { get; set; }
    public virtual DbSet<User> Users { get; set; }
}