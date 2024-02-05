using NLog;
using SyncMeAppLibrary.Model;

namespace SyncMeAppLibrary.BL
{
    public class SyncLogic
    {
        public static void ReplicateSourceDirectory(InputParameters inputParameters, Logger log)
        {
            try
            {
                log.Info($"Starting to synchronize target directory {inputParameters.ReplicaDirectory} with source directory {inputParameters.SourceDirectory}.");
                
                DirectoryInfo[] sourceDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.SourceDirectory);
                if (!sourceDirWithSubdirectories[0].Exists || sourceDirWithSubdirectories.Length == 0) 
                    throw new Exception($"Source directory {inputParameters.SourceDirectory} not found!");

                DirectoryInfo[] replicaDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.ReplicaDirectory);

                // Need to compare relative paths of directories without their parent
                string[] sourceRelativePaths = GetRelativePaths(sourceDirWithSubdirectories, inputParameters.SourceDirectory);
                string[] replicaRelativePaths = GetRelativePaths(replicaDirWithSubdirectories, inputParameters.ReplicaDirectory);

                IFileItem[] allSourceDirFiles = FileItem.GetAllFiles(sourceDirWithSubdirectories[0]);
                IFileItem[] allReplicaDirFiles = FileItem.GetAllFiles(replicaDirWithSubdirectories[0]);

                if (allReplicaDirFiles.Any())
                {
                    log.Warn($"Target directory {inputParameters.ReplicaDirectory} contains files that may be overwritten or deleted.");
                }

                // Directories to be created or deleted in replica
                IEnumerable<string> dirsToCreate = [];
                IEnumerable<string> dirsToDelete = [];
                // Files to be copied or deleted
                IFileItem[] filesToCopy = [];
                IFileItem[] filesToDelete = [];

                dirsToCreate = (from path in sourceRelativePaths select path).Except(replicaRelativePaths);
                filesToCopy = FindDifferentFiles(allSourceDirFiles, allReplicaDirFiles);

                // First create directories and copy files
                if (dirsToCreate.Any())
                {
                    foreach (string dir in dirsToCreate)
                    {
                        CreateDirectory(Path.Combine(inputParameters.ReplicaDirectory, dir), log); 
                    }
                }
                if (filesToCopy.Any())
                {
                    CopyFiles(filesToCopy.ToArray(), inputParameters.ReplicaDirectory, log);
                }

                // Need to refresh replica directories and files to know, what we need to delete
                replicaDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.ReplicaDirectory);
                allReplicaDirFiles = FileItem.GetAllFiles(replicaDirWithSubdirectories[0]);

                dirsToDelete = (from path in replicaRelativePaths select path).Except(sourceRelativePaths);
                filesToDelete = FindDifferentFiles(allReplicaDirFiles, allSourceDirFiles);

                // Then delete files and directories that are not in source directory.
                if (filesToDelete.Any())
                    DeleteFiles(filesToDelete.ToArray(), log);

                if (dirsToDelete.Any())
                    DeleteDirectories(GetDirs(dirsToDelete.ToArray(), inputParameters.ReplicaDirectory), log);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        private static void CopyFiles(IFileItem[] files, string targetRootDirectory, Logger log)
        {
            foreach (IFileItem file in files)
            {
                string targetFileFullName = file.FullName.Replace(file.RootDirPath, targetRootDirectory);
                if (File.Exists(targetFileFullName))
                    log.Warn($"File {targetFileFullName} already exists and will be overwritten");

                File.Copy(file.FullName, targetFileFullName, true);
                log.Info($"File {file.FullName} copied to {targetFileFullName}.");
            }
        }

        private static void CreateDirectory(string path, Logger log)
        {
            if (Directory.Exists(path))
            {
                log.Error($"Directory {path} already exists!");
                return;
            }
            Directory.CreateDirectory(path);
            log.Info($"Directory {path} created.");
        }

        public static IFileItem[] FindDifferentFiles(IFileItem[] files, IFileItem[] filesToCompareTo)
        {
            List<IFileItem> diffFiles = [];
            foreach (IFileItem file in files)
            {
                if (!filesToCompareTo.Contains(file))
                    diffFiles.Add(file);
            }
            return diffFiles.ToArray();
        }

        private static DirectoryInfo[] GetDirs(string[] Paths, string rootPath)
        {
            var directories = new List<DirectoryInfo>() { };
            foreach (string path in Paths)
            {
                directories.Add(new DirectoryInfo(Path.Combine(rootPath, path)));
            }
            return directories.ToArray();
        }

        private static DirectoryInfo[] GetDirAndSubdirectories(string directoryPath)
        {
            var directories = new List<DirectoryInfo>() { };
            var rootDir = new DirectoryInfo(directoryPath);
            directories.Add(rootDir);
            try
            {
                foreach (DirectoryInfo subdirectoryInfo in directories[0].GetDirectories("*", SearchOption.AllDirectories))
                {
                    directories.Add(subdirectoryInfo);
                }
            }
            catch (DirectoryNotFoundException)
            {
                return directories.ToArray();
            }
            return directories.ToArray();
        }

        private static string[] GetRelativePaths(DirectoryInfo[] directories, string rootDirectory)
        {
            List<string> relativePaths = new List<string> { };
            foreach (DirectoryInfo dir in directories)
            {
                if (!dir.FullName.Equals(rootDirectory, StringComparison.OrdinalIgnoreCase)) // Root directory will be skiped.
                    relativePaths.Add(dir.FullName.Replace(rootDirectory, string.Empty));
            }
            return relativePaths.ToArray();
        }

        private static void DeleteDirectories(DirectoryInfo[] dirs, Logger log)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                if (Path.Exists(dir.FullName))
                {
                    Directory.Delete(dir.FullName);
                    log.Info($"Directory {dir.FullName} deleted");
                }
                else
                {
                    log.Error($"Directory {dir.FullName} not found!");
                }
            }
        }

        private static void DeleteFiles(IFileItem[] files, Logger log)
        {
            foreach (IFileItem file in files)
            {
                if (Path.Exists(file.FullName))
                {
                    File.Delete(file.FullName);
                    log.Info($"File {file.FullName} deleted");
                }
                else
                {
                    log.Error($"File {file.FullName} not found!");
                }
            }
        }

        


    }
}
