using University.Project.Library.DataAccess;
using University.Project.Library.Interfaces;
using University.Project.Library.Models;

namespace University.Project.Library.Services;

public class FileWorkflowManager : IFileWorkflowManager
{
    public static List<string> docs = [];
    readonly private ApplicationDbContext _context;
    readonly List<string> _paths = [];
    readonly List<string> _acceptableExtensions = [];

    public FileWorkflowManager(ApplicationDbContext context, List<string> paths, List<string> ext)
    {
        _acceptableExtensions = ext;
        _context = context;
        _paths = paths;
    }

    private void ExtractContentRecursively(string path, CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested) return;

            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            docs.AddRange(files.Where(x => _acceptableExtensions.Contains(Path.GetExtension(x))));

            if (dirs.Length != 0) foreach (var dir in dirs) ExtractContentRecursively(dir, cancellationToken);
        }
        catch { }
    }

    public void ScanDirectories(CancellationToken cancellationToken)
    {
        foreach (var dir in _paths) ExtractContentRecursively(dir, cancellationToken);
    }

    public void IndexFiles()
    {
        _context.Files.AddRange(docs.Where(x =>
        {
            var file = _context.Files.FirstOrDefault(y => y.Path == x);
            if (file == null)
                return true;
            return false;
        }).Select(x => new CustomFileModel() { Path = x }));
        _context.SaveChanges();
    }

    public List<CustomFileModel> FetchFiles()
    {
        return [.. _context.Files];
    }
}
