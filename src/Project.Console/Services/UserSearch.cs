using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Models;

namespace University.ConsoleApp.Services;

public class UserSearch
{
    public List<CustomScoreModel> FindInterception(string[] words)
    {
        using (var _context = new ApplicationDbContext())
        {
            List<CustomScoreModel> allPaths = new();
            var queryResult = _context.Words
                .AsNoTracking()
                .Where(x => words.Contains(x.Word))
                .Include(x => x.Tf)
                .ThenInclude(tf => tf.CustomFile)
                .ToList();

            foreach (var word in queryResult)
            {
                AddPathsToListByTfidf(word, allPaths);
            }

            var result = MergeScores(allPaths);

            return result
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();
        }
    }

    public List<CustomScoreModel> MergeScores(List<CustomScoreModel> allPaths)
    {
        var result = new List<CustomScoreModel>();
        foreach (var item in allPaths)
        {
            var path = result.FirstOrDefault(x => x.Path == item.Path);
            if (path == null)
            {
                result.Add(item);
                continue;
            }
            path.Score += item.Score;
        }
        return result;
    }

    public void AddPathsToListByTfidf(WordTokenModel word, List<CustomScoreModel> allPaths)
    {
        using (var _context = new ApplicationDbContext())
        {
            int documentFrequency = word.Tf.Count;
            int documentsCount = _context.Files.Count();

            var idf = (decimal)Math.Log(documentsCount / documentFrequency);

            foreach (var tf in word.Tf)
            {
                allPaths.Add(new() { Path = tf.CustomFile.Path, Score = tf.Repetation * idf });
            }
        }
    }

    public string[] RefineQuery(string userInput)
    {
        return userInput
            .Split()
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => x.ToLower())
            .ToArray();
    }
}
