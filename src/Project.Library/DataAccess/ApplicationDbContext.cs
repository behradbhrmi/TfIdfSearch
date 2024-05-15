using Microsoft.EntityFrameworkCore;
using University.Project.Library.Models;

namespace University.Project.Library.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<CustomFileModel> Files => Set<CustomFileModel>();
    public DbSet<WordTokenModel> Words => Set<WordTokenModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<WordTokenModel>().HasMany(x => x.Files).WithMany();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source="+Path.Combine(AppContext.BaseDirectory, "db.sqlite"));
}