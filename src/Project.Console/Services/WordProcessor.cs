using University.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using University.ConsoleApp.DataAccess;
using University.ConsoleApp.Dto;
using University.ConsoleApp.Extensions;

namespace University.ConsoleApp.Services;


public class WordProcessor
{
    private ApplicationDbContext _dbContext = new();

    public void Process(FileDto file)
    {
        var path = _dbContext.Files.FirstOrDefault(x => x.Path == file.CustomFile.Path);

        foreach (var item in file.Content.RemovePunctuations().Split().Where(x => !string.IsNullOrEmpty(x)))
        {

            var wordToken = _dbContext.Words
                .Where(x => x.Word == item)
                .Include(x => x.Tf)
                .ThenInclude(tf => tf.CustomFile)
                .FirstOrDefault();

            if (wordToken is null)
            {
                var word = new WordTokenModel()
                {
                    Word = item,
                    Tf = [new() { CustomFile = path, Repetation = 1 }]
                };
                _dbContext.Words.Add(word);
                _dbContext.SaveChanges();
                continue;
            }

            var tf = wordToken?.Tf.FirstOrDefault(x => x.CustomFile == path);

            if (tf is null)
            {
                wordToken?.Tf.Add(new() { CustomFile = path, Repetation = 1 });
                _dbContext.SaveChanges();
                continue;
            }

            tf.Repetation++;
            _dbContext.SaveChanges();
        }
        _dbContext.SaveChanges();
    }
}
