using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Interfaces;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public class FileWorkflowManager : IFileWorkflowManager
{
    public static List<string> docs = [];
    readonly private ApplicationDbContext _context;
    static List<DirectoryModel> _paths;
    static List<string> _acceptableExtensions;

    public FileWorkflowManager(ApplicationDbContext context, List<DirectoryModel> paths, List<string> ext)
    {
        _acceptableExtensions = ext;
        _context = context;
        _paths = paths;
    }

    private void ExtractContentRecursively(string path)
    {
        try
        {
            if (File.Exists(path) 
                && _acceptableExtensions.Contains(Path.GetExtension(path)) 
                && !docs.Contains(path))
                docs.Add(path);

            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            docs.AddRange(files.Select(x => x.ToLower()).Where(x => _acceptableExtensions.Contains(Path.GetExtension(x))).Where(x => !docs.Contains(x)));

            if (dirs.Length != 0) foreach (var dir in dirs) ExtractContentRecursively(dir);
        }
        catch { }
    }

    public void ClearDBRows()
    {
        _context.RemoveRange(_context.Tfidfs.ToList());
        _context.RemoveRange(_context.Words.ToList());
        _context.RemoveRange(_context.Files.ToList());
        _context.SaveChanges();
    }

    public void ScanDirectories()
    {
        ClearDBRows();
        foreach (var dir in _paths) ExtractContentRecursively(dir.Address);
        IndexFiles();
    }

    public void IndexFiles()
    {
        _context.Files.AddRange(docs.Where(x =>
        {
            var existingFile = _context.Files.FirstOrDefault(y => y.Path.ToLower() == x.ToLower());

            if (existingFile == null) return true;
            return false;

        }).Select(x => new CustomFileModel() { Path = x }));

        _context.SaveChanges();
    }

    public List<CustomFileModel> FetchFiles()
    {
        return [.. _context.Files];
    }
}
