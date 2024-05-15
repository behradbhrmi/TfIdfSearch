using University.ConsoleApp.Dto;
using University.ConsoleApp.Interfaces;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Models;
using University.ConsoleApp.Helper;

namespace University.ConsoleApp.Services;

public class ProgramApi
{
    private static UserSearch _userSearch = new();
    private static WordProcessor _wordProcessor = new();
    private static ContentExtractor _contentExtractor = new();
    private static ApplicationDbContext _context = new();
    private static List<DirectoryModel> _paths = new List<DirectoryModel>();
    private static List<string> _acceptableExtensions = [FileExtensions.Word, FileExtensions.Text, FileExtensions.Pdf, FileExtensions.Png, FileExtensions.Jpg, FileExtensions.Jpeg];
    private static IFileWorkflowManager _fileWorkflowManager;

    public ProgramApi()
    {
        LoadAllPathForSearch();
        _fileWorkflowManager = new FileWorkflowManager(_context, _paths, _acceptableExtensions);
    }

    private void SaveFileContentToDb()
    {
        var files = _context.Files.ToList();

        foreach (var file in files)
        {
            var content = _contentExtractor.ReadContent(file);
            var newFile = new FileDto()
            {
                CustomFile = file,
                Content = content
            };

            _wordProcessor.Process(newFile);
        }
    }

    public void IndexFiles()
    {
        _fileWorkflowManager.ScanDirectories();
        SaveFileContentToDb();
    }

    public List<CustomScoreModel> Search(string searchText)
    {
        var keyWords = _userSearch.RefineQuery(searchText);
        return _userSearch.FindInterception(keyWords);
    }

    public void LoadAllPathForSearch()
    {
        _paths = SearchLocation.FetchSearchLocations();
    }

    public List<DirectoryModel> ReadAllPathForSearch()
    {
        return _paths;
    }

    public void AddToPathForSearch(string dir)
    {
        SearchLocation.AddSearchLocations(dir);
        LoadAllPathForSearch();
    }

    public void RemoveFromPathForSearch(int dirIndex)
    {
        if (dirIndex < 1 || dirIndex > _paths.Count)
            return;

        var dir = _paths[dirIndex - 1];
        SearchLocation.RemoveSearchLocations(dir);
        LoadAllPathForSearch();
    }
}

