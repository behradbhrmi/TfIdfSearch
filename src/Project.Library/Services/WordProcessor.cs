using University.Project.Library.Models;
using Microsoft.EntityFrameworkCore;
using University.Project.Library.Dto;
using University.Project.Library.DataAccess;

namespace University.Project.Library.Services;


public class WordProcessor
{
    private ApplicationDbContext _dbContext = new();

    public async Task ProcessAsync(FileDto file)
    {
        var result = PythonInterPreter.ProcessContent(file.Content);

        // TODO: check errors
        if (result[0] == "end")
            return;

        foreach (var item in result)
        {
            var path = await _dbContext.Files.FirstOrDefaultAsync(x => x.Path == file.CustomFile.Path);

            var wordToken = await _dbContext.Words.FirstOrDefaultAsync(x => x.Word == item);

            if (wordToken is null)
            {
                var word = new WordTokenModel()
                {
                    Word = item,
                    Files = [path]
                };
                await _dbContext.Words.AddAsync(word);
                await _dbContext.SaveChangesAsync();
                continue;
            }

            if (!wordToken.Files.Contains(path))
                wordToken.Files.Add(path);
            await _dbContext.SaveChangesAsync();
        }
    }

}
