namespace University.Project.Library.Models;

public class WordTokenModel
{
    public int Id { get; set; }
    public string Word { get; set; }
    public List<CustomFileModel> Files = new();
}
