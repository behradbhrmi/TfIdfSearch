using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Extensions;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public static class SearchLocation
{
    public static List<DirectoryModel> FetchSearchLocations()
    {
        List<DirectoryModel> directories;
        using (var _context = new ApplicationDbContext())
        {
            directories = _context.Directpries.ToList();
        }
        return directories;
    }

    public static void AddSearchLocations(string dir)
    {
        using (var _context = new ApplicationDbContext())
        {
            dir = dir.StringToPath();
            var existingAddress = _context.Directpries.FirstOrDefault(x => x.Address == dir.ToLower());
            if (existingAddress == null)
            {
                _context.Directpries.Add(new() { Address = dir.ToLower() });
                _context.SaveChanges();
            }
        }
    }

    public static void RemoveSearchLocations(DirectoryModel dir)
    {
        using (var _context = new ApplicationDbContext())
        {
            _context.Remove(dir);
            _context.SaveChanges();
        }
    }
}
