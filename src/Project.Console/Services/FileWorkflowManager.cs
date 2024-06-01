using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Interfaces;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public class FileWorkflowManager : IFileWorkflowManager
{
    public static List<string> docs = [];
    static List<DirectoryModel> _paths;
    static List<string> _acceptableExtensions;

    public FileWorkflowManager(List<DirectoryModel> paths, List<string> ext)
    {
        _acceptableExtensions = ext;
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



    public void ScanDirectories()
    {
        Console.WriteLine("> Indexing...");
        foreach (var dir in _paths) ExtractContentRecursively(dir.Address);
        IndexFiles();
        ShowIndexedFilesInfo();
    }

    private void ShowIndexedFilesInfo()
    {
        var result = FetchFiles().Select(x => x.Path);
        var count = result.Count();
        Console.WriteLine($"Indexed Files Count: {count}");
        if (count == 0)
            return;
        int n = 1;
        foreach (var file in result) { Console.WriteLine($"{n}. {file}"); n++; }
        Console.WriteLine("> Indexing Finished");
    }

    public void IndexFiles()
    {
        using (var _context = new ApplicationDbContext())
        {
            _context.Tfidfs.RemoveRange(_context.Tfidfs.ToList());
            _context.Words.RemoveRange(_context.Words.ToList());
            _context.Files.RemoveRange(_context.Files.ToList());
            _context.SaveChanges();

            _context.Files.AddRange(docs.Select(x => new CustomFileModel() { Path = x }));
            _context.SaveChanges();
            docs.Clear();
        }
    }

    public List<CustomFileModel> FetchFiles()
    {
        List<CustomFileModel> files;
        using (var _context = new ApplicationDbContext())
        {
            files = _context.Files.ToList();
        }
        return files;
    }
}
