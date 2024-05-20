using University.ConsoleApp.Interfaces;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Models;
using University.ConsoleApp.Helper;
using University.ConsoleApp.Extensions;
using Microsoft.EntityFrameworkCore;

namespace University.ConsoleApp.Services;

public class ProgramApi
{
    private static UserSearch _userSearch;
    private static ContentExtractor _contentExtractor = new();
    private static List<DirectoryModel> _paths = new List<DirectoryModel>();
    private static List<string> _acceptableExtensions = [FileExtensions.Word, FileExtensions.Text, FileExtensions.Pdf, FileExtensions.Png, FileExtensions.Jpg, FileExtensions.Jpeg];
    private static IFileWorkflowManager _fileWorkflowManager;
    private static List<WordTokenModel> _wordTokens = new List<WordTokenModel>();

    public ProgramApi()
    {
        _userSearch = new();
        LoadAllPathForSearch();
        _fileWorkflowManager = new FileWorkflowManager(_paths, _acceptableExtensions);
    }


    private void SaveFileContentToDb()
    {
        using (var _dbContext = new ApplicationDbContext())
        {
            _dbContext.Words.AddRange(_wordTokens.Select(x =>
            {
                x.Tf = x.Tf.Select(tf => new TfidfModel()
                {
                    CustomFile = _dbContext.Files.FirstOrDefault(x => x.Path == tf.CustomFile.Path),
                    Repetation = tf.Repetation
                }).ToList();
                return x;
            })
            );
            _dbContext.SaveChanges();
        }
    }

    public void ProcessContentIntoMemory()
    {
        IEnumerable<CustomFileModel> files;
        using (var _context = new ApplicationDbContext())
        {
            files = _context.Files.AsNoTracking().ToList();
        }

        foreach (var file in files)
        {
            var content = _contentExtractor.ReadContent(file);

            if (string.IsNullOrEmpty(content))
                continue;

            var contentAsList = content.RemovePunctuations().Split().Where(x => !string.IsNullOrEmpty(x));

            content = null;

            foreach (var item in contentAsList)
            {
                var wordToken = _wordTokens
                    .Where(x => x.Word == item)
                    .FirstOrDefault();

                if (wordToken is null)
                {
                    var word = new WordTokenModel()
                    {
                        Word = item,
                        Tf = [new() { CustomFile = file, Repetation = 1 }]
                    };
                    _wordTokens.Add(word);
                    continue;
                }

                var tf = wordToken?.Tf.FirstOrDefault(x => x.CustomFile == file);

                if (tf is null)
                {
                    wordToken?.Tf.Add(new() { CustomFile = file, Repetation = 1 });
                    continue;
                }
                tf.Repetation++;
            }
        }
    }

    private void Process()
    {
        ProcessContentIntoMemory();
        SaveFileContentToDb();
        _wordTokens.Clear();
    }


    public void IndexFiles()
    {
        _fileWorkflowManager.ScanDirectories();
        Process();
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

