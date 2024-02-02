using System.Security.Cryptography;

namespace SyncMeAppLibrary.Model
{
    public class FileCompare : IEqualityComparer<FileInfo>
    {
        public string RootPath1 { get; set; }
        public string RootPath2 { get; set; }

        public FileCompare(InputParameters inputParameters) 
        {

            RootPath1 = inputParameters.SourceDirectory;
            RootPath2 = inputParameters.ReplicaDirectory;
        }

        public bool Equals(FileInfo file1, FileInfo file2)
        {
            string relativePath1 = file1.FullName.Replace(RootPath1, string.Empty);
            string relativePath2 = file2.FullName.Replace(RootPath2, string.Empty);
            byte[] hash1 = GetMD5Hash(file1.FullName);
            byte[] hash2 = GetMD5Hash(file2.FullName);
            return hash1.SequenceEqual(hash2) && relativePath1 == relativePath2;
        }

        public int GetHashCode(FileInfo file)  
        {
            string relativePath = file.FullName.Replace(RootPath1, string.Empty);
            byte[] hash = GetMD5Hash(file.FullName);
            return HashCode.Combine(relativePath, hash);
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




// pozn.: možná problém s .netCore

/*
        public bool Equals(FileInfo file1, FileInfo file2, InputParameters inputParameters)
        {
            string relativePath1 = file1.FullName.Replace(inputParameters.SourceDirectory, string.Empty);
            string relativePath2 = file2.FullName.Replace(inputParameters.ReplicaDirectory, string.Empty);
            byte[] hash1 = GetMD5Hash(file1.FullName);
            byte[] hash2 = GetMD5Hash(file2.FullName);
            return hash1.SequenceEqual(hash2) && relativePath1 == relativePath2;
        }*/