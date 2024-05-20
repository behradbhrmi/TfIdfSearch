using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<CustomFileModel> Files => Set<CustomFileModel>();
    public DbSet<WordTokenModel> Words => Set<WordTokenModel>();
    public DbSet<TfidfModel> Tfidfs => Set<TfidfModel>();
    public DbSet<DirectoryModel> Directpries => Set<DirectoryModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) { 
        modelBuilder.Entity<WordTokenModel>().HasMany(x => x.Tf).WithMany();
        modelBuilder.Entity<CustomFileModel>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=" + Path.Combine(AppContext.BaseDirectory, "db.sqlite"));
}

//optionsBuilder.AddInterceptors(new DisableWalInterceptor());
//public void SetJournalModeToDelete()
//{
//    this.Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE;");
//}
//}

//public class DisableWalInterceptor : SaveChangesInterceptor
//{
//    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
//    {
//        var context = (DbContext)eventData.Context;
//        context.Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE;");
//        return base.SavingChanges(eventData, result);
//    }
//}