using Microsoft.EntityFrameworkCore;

public static class Utilities
{
    public static void InitializeDbForTests(TodoAppDbContext db)
    {
        db.Database.EnsureCreated();
        db.Users.Add(new User(1, "admin", "admin", "admin@admin.adm", "Admin", "Admin"));
        db.SaveChanges();
    }
}