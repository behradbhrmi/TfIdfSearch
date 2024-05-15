using University.ConsoleApp.Models;

namespace University.ConsoleApp.Interfaces;

public interface IFileWorkflowManager
{
    public void ScanDirectories();
    public void IndexFiles();
    public List<CustomFileModel> FetchFiles();
}
