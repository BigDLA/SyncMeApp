namespace SyncMeAppLibrary.Model
{
    public interface IFileItem
    {
        string FullName { get; set; }
        string RelativePath { get; set; }
        string RootDirPath { get; set; }
    }
}