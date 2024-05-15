namespace University.ConsoleApp.Models;

public class WordTokenModel
{
    public int Id { get; set; }
    public string Word { get; set; }
    public List<TfidfModel> Tf = new();
}
