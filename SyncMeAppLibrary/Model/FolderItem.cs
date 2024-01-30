using NLog;
using System.Security.Cryptography;

namespace SyncMeAppLibrary.Model
{
    public class FolderItem
    {

        public string Name { get; set; }
        public string Path { get; set; }
        public Type ItemType { get; set; }
        public Stream Stream { get; set; }

        public FolderItem(FileInfo fileInfo)
        {
            Name = fileInfo.Name;
            Path = fileInfo.FullName;
            ItemType = fileInfo.Attributes.GetType();
            Stream = File.OpenRead(Path);
        }


        // To-Do: optimalizace na množství souborů? ať nedržím v paměti tisíce souborů


        public static FolderItem[] LoadAllFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new Exception($"Folder {folderPath} not found!");

            var files = new FolderItem[] { };
            DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfo = folderInfo.GetFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo info in fileInfo)
            {
                files.Append(new FolderItem(info));
            }
            return files;
        }

    public static List<byte[]> GetFilesHash(FolderItem[] files)
    {
        var hashes = new List<byte[]>();

        foreach (FolderItem file in files)
        {
            using (var md5 = MD5.Create())
            {
                hashes.Add(md5.ComputeHash(file.Stream));
            }
        }
        return hashes;
    }

    public static bool CompareHashes(byte[] sourceHash, byte[] replicaHash)
    {
        return sourceHash.SequenceEqual(replicaHash);
    }
}
}
