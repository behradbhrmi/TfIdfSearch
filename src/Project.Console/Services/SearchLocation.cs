using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public static class SearchLocation
{
    private static string _pathForLocationFile = Path.Combine(AppContext.BaseDirectory, "locations.txt");
    private static CustomFileModel _file = new() { Path = _pathForLocationFile };
    private static ContentExtractor _contentExtractor = new ContentExtractor();
    private static ApplicationDbContext _context = new();

    public static List<DirectoryModel> FetchSearchLocations()
    {
        return _context.Directpries.ToList();
    }

    public static void AddSearchLocations(string dir)
    {
        var existingAddress = _context.Directpries.FirstOrDefault(x => x.Address == dir.ToLower());
        if (existingAddress == null)
        {
            _context.Directpries.Add(new() { Address = dir.ToLower() });
            _context.SaveChanges();
        }
    }

    public static void RemoveSearchLocations(DirectoryModel dir)
    {
        _context.Remove(dir);
        _context.SaveChanges();
    }

}
