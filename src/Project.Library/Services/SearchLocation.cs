using University.Project.Library.Models;

namespace University.Project.Library.Services;

public static class SearchLocation
{
    private static string _pathForLocationFile = Path.Combine(AppContext.BaseDirectory, "locations.txt");
    private static CustomFileModel _file = new() { Path = _pathForLocationFile };
    private static ContentExtractor _contentExtractor = new ContentExtractor();

    public static string[] FetchSearchLocations()
    {
        if (!File.Exists(_pathForLocationFile))
        {
            File.Create(_pathForLocationFile);
            return [];
        }
        return _contentExtractor.ReadContent(_file).Split();
    }

    public static void AddSearchLocations(string dir)
    {
        File.AppendAllText(_pathForLocationFile, $"\n{dir}");
    }
}
