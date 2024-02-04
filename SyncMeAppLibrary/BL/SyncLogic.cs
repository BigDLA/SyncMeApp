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
                if (!sourceDirWithSubdirectories[0].Exists || sourceDirWithSubdirectories.Length == 0) // Bude fungovat?
                    throw new Exception($"Source directory {inputParameters.SourceDirectory} not found!");

                DirectoryInfo[] replicaDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.ReplicaDirectory);

                // Need to compare directories without parent (parent name may differ)
                string[] sourceRelativePaths = Utils.RelativePaths(sourceDirWithSubdirectories, inputParameters.SourceDirectory);
                string[] replicaRelativePaths = Utils.RelativePaths(replicaDirWithSubdirectories, inputParameters.ReplicaDirectory);
                //var subDirsAreSame = sourceRelativePaths.SequenceEqual(replicaRelativePaths);

                FileItem[] allSourceDirFiles = FileItem.GetAllFiles(sourceDirWithSubdirectories[0]);
                FileItem[] allReplicaDirFiles = FileItem.GetAllFiles(replicaDirWithSubdirectories[0]);
                //bool filesAreSame = allSourceDirFiles.SequenceEqual(allReplicaDirFiles, myFileCompare); // TO-DO: otestovat pro různý počet souborů apod.

                if (allReplicaDirFiles.Any())
                {
                    log.Warn($"Target directory {inputParameters.ReplicaDirectory} contains files.");
                    if (!Utils.ConfirmationDialog($"All files in target directory {inputParameters.ReplicaDirectory} that do not match files from source directory {inputParameters.SourceDirectory} will be overwritten or deleted. Proceed ? Y/N:", showDialog: inputParameters.AskDeleteConfirmation))
                        throw new Exception(Logging.msgActionCanceledByUser);
                }

                // Directories to be created or deleted in replica
                IEnumerable<string> dirsToCreate = [];
                IEnumerable<string> dirsToDelete = [];
                // Files to be created or deleted
                IEnumerable<FileItem> filesToCopy = [];
                IEnumerable<FileItem> filesToDelete = [];

                dirsToCreate = (from path in sourceRelativePaths select path).Except(replicaRelativePaths);
                filesToCopy = FileItem.FindDifferentFiles(allSourceDirFiles, allReplicaDirFiles);


                // First create directories and copy files
                if (dirsToCreate.Any())
                {
                    foreach (string dir in dirsToCreate)
                    {
                        CreateDirectory(Path.Combine(inputParameters.ReplicaDirectory, dir), log); //TO-DO: Je zajištěno pořadí podsložek?
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
                filesToDelete = FileItem.FindDifferentFiles(allReplicaDirFiles, allSourceDirFiles);

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

        public static void CreateDirectory(string path, Logger log)
        {
            if (Directory.Exists(path))
            {
                log.Error($"Directory {path} already exists!");
                return;
            }
            Directory.CreateDirectory(path);
            log.Info($"Directory {path} created.");
        }

        public static DirectoryInfo[] GetDirs(string[] Paths, string rootPath)
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

        private static void CopyFiles(FileItem[] files, string targetRootDirectory, Logger log)
        {
            foreach (FileItem file in files)
            {
                string targetFileFullName = file.FullName.Replace(file.RootDirPath, targetRootDirectory);
                if (File.Exists(targetFileFullName))
                    log.Warn($"File {targetFileFullName} already exists and will be overwritten");

                File.Copy(file.FullName, targetFileFullName, true);
                log.Info($"File {file.FullName} copied to {targetFileFullName}.");
            }
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

        private static void DeleteFiles(FileItem[] files, Logger log)
        {
            foreach (FileItem file in files)
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
