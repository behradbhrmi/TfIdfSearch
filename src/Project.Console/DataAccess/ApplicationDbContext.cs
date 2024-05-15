using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<CustomFileModel> Files => Set<CustomFileModel>();
    public DbSet<WordTokenModel> Words => Set<WordTokenModel>();
    public DbSet<TfidfModel> Tfidfs => Set<TfidfModel>();
    public DbSet<DirectoryModel> Directpries => Set<DirectoryModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<WordTokenModel>().HasMany(x => x.Tf).WithMany();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=" + Path.Combine(AppContext.BaseDirectory, "db.sqlite"));
}