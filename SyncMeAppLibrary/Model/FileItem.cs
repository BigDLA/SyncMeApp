using System.Security.Cryptography;

namespace SyncMeAppLibrary.Model
{
    public class FileItem(FileInfo file, string rootPath) : IEquatable<FileItem?>, IFileItem
    {
        public string FullName { get; set; } = file.FullName;
        public string RootDirPath { get; set; } = rootPath;
        public string RelativePath { get; set; } = file.FullName.Replace(rootPath, string.Empty);


        public static FileItem[] GetAllFiles(DirectoryInfo directory)
        {
            FileInfo[] filesInfo = directory.GetFiles("*", SearchOption.AllDirectories);
            List<FileItem> fileItems = [];
            foreach (FileInfo fileInfo in filesInfo)
            {
                fileItems.Add(new FileItem(fileInfo, directory.FullName));
            }
            return fileItems.ToArray();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FileItem);
        }

        public bool Equals(FileItem? file)
        {
            if (file == null)
                return false;

            byte[] hash1 = GetMD5Hash(FullName);
            byte[] hash2 = GetMD5Hash(file.FullName);
            return hash1.SequenceEqual(hash2) && RelativePath == file.RelativePath;
        }

        public override int GetHashCode()
        {
            byte[] hash = GetMD5Hash(FullName);
            return HashCode.Combine(RelativePath, hash);
        }

        private static byte[] GetMD5Hash(string path)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                return md5.ComputeHash(stream);
            }
        }
    }


}
    

