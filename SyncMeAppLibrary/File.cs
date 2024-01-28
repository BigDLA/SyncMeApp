using System.Security.Cryptography;

namespace SyncMeAppLibrary
{
    public class FolderItem
    {

        public  string Name { get; set; }
        public  string Path { get; set; }
        public  Stream Stream { get; set; }

        public FolderItem(FileInfo fileInfo) 
        {
            Name = fileInfo.Name;
            Path = fileInfo.FullName;
            Stream = File.OpenRead(Path);
        }


        // To-Do: optimalizace na množství souborů? ať nedržím v paměti tisíce souborů
        public static FolderItem[] LoadAllFiles(string folderPath)
        {
            var files = new FolderItem[] { };

            try
            {
                DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
                FileInfo[] fileInfo = folderInfo.GetFiles("*", SearchOption.AllDirectories);

                foreach (FileInfo info in fileInfo)
                {
                    files.Append(new FolderItem(info));
                }
            }
            catch (Exception ex) 
            {
                // TO-DO: zapiš do konzole
                throw;
            }

            return files;
        }

        // <nazev souboru, hash>
        public static Dictionary<string, byte[]> GetFilesHash(FolderItem[] files)
        {
            var hashes = new Dictionary<string, byte[]>();

            foreach (FolderItem file in files)
            {
                using (var md5 = MD5.Create()) 
                {
                    hashes[file.Name] = md5.ComputeHash(file.Stream);
                }
            }
            return hashes;
        }


        //metodu na výpočet zvlášť -> načítat soubory jen jednou 
        //var hashes = new Dictionary<string, string>();

    }
}
