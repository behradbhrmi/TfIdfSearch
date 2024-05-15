using University.Project.Library.Dto;
using University.Project.Library.Interfaces;
using University.Project.Library.DataAccess;
using University.Project.Library.Models;
using University.Project.Library.Helper;

namespace University.Project.Library.Services;

public class ProgramApi
{
    private static UserSearch _userSearch = new();
    private static WordProcessor _wordProcessor = new();
    private static ContentExtractor _contentExtractor = new();
    private static ApplicationDbContext _context = new();
    private static List<string> _paths = new List<string>();
    private static List<string> _acceptableExtensions = [FileExtensions.Word, FileExtensions.Text, FileExtensions.Pdf, FileExtensions.Png, FileExtensions.Jpg, FileExtensions.Jpeg];
    private static IFileWorkflowManager _fileWorkflowManager = new FileWorkflowManager(_context, _paths, _acceptableExtensions);

    private void SaveFileContentToDb(bool allDocs, CancellationToken cancellationToken)
    {
        var thread = new Thread(() =>
        {
            IQueryable<CustomFileModel> filesQuery = _context.Files;

            if (!allDocs)
                filesQuery = filesQuery.Where(x => !x.IsChecked);

            var files = filesQuery.ToList();

            files.Select(async x =>
            {
                var content = _contentExtractor.ReadContent(x);

                var file = new FileDto()
                {
                    CustomFile = x,
                    Content = content
                };

                await _wordProcessor.ProcessAsync(file);
            });
        });

        thread.Start();
    }

    public void IndexFileContent(CancellationToken cancellationToken)
    {
        SaveFileContentToDb(false, cancellationToken);
    }

    public void ReSyncFileContent(CancellationToken cancellationToken)
    {
        SaveFileContentToDb(true, cancellationToken);
    }

    public async Task<string[]> SearchAsync(string searchText)
    {
        var keyWords = _userSearch.RefineQuery(searchText);
        return await _userSearch.FindInterceptionAsync(keyWords);
    }

    public void LoadAllPathForSearch()
    {
        _paths = SearchLocation.FetchSearchLocations().ToList();
    }

    public string[] ReadAllPathForSearch()
    {
        return _paths.ToArray();
    }

    public void AddToPathForSearch(string dir)
    {
        SearchLocation.AddSearchLocations(dir);
        LoadAllPathForSearch();
    }

    public void ScanFiles(CancellationToken cancellationToken)
    {
        _fileWorkflowManager.ScanDirectories(cancellationToken);
    }

    public void IndexFiles()
    {
        _fileWorkflowManager.IndexFiles();
    }

}

