using Microsoft.EntityFrameworkCore;
using System.Linq;
using University.Project.Library.DataAccess;

namespace University.Project.Library.Services;

public class UserSearch
{
    private ApplicationDbContext _context = new();

    public async Task<string[]> FindInterceptionAsync(string[] words)
    {
        var queryResult = await _context.Words.Where(x => words.Contains(x.Word)).ToListAsync();

        //if (cancellationToken.IsCancellationRequested)
        //    return [];

        var allPaths = new List<string>();
        queryResult.ForEach(x => { allPaths.AddRange(x.Files.Select(x => x.Path).ToList()); });

        var interceptions = allPaths
        .GroupBy(file => file)
        .Where(group => group.Count() > 1)
        .Select(group => group.Key).ToArray();

        return interceptions;
    }

    public string[] RefineQuery(string userInput)
    {
        return PythonInterPreter.ProcessContent(userInput);
    }
}
