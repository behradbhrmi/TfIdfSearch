using University.Project.Library.Models;

namespace University.Project.Library.Interfaces;

public interface IFileWorkflowManager
{
    public void ScanDirectories(CancellationToken cancellationToken);
    public void IndexFiles();
    public List<CustomFileModel> FetchFiles();
}
