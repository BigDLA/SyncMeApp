using NLog;
using SyncMeAppLibrary.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SyncMeAppLibrary.BL
{
    //logger.Info("Message with {myProperty}", "myValue");

    public class SyncLogic
    {
        public static void ReplicateSourceDirectory(InputParameters inputParameters, Logger log)
        {
            try
            {
                log.Info($"Starting to synchronize target directory {inputParameters.ReplicaDirectory} with source directory {inputParameters.SourceDirectory}.");
                DirectoryInfo[] sourceDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.SourceDirectory);
                if (sourceDirWithSubdirectories.Length == 0)
                    throw new Exception($"Source directory {inputParameters.SourceDirectory} not found!");

                DirectoryInfo[] replicaDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.ReplicaDirectory);

                FileInfo[] sourceParentDirFiles = sourceDirWithSubdirectories[0].GetFiles("*", SearchOption.TopDirectoryOnly); //TO-DO: vytáhnout podmnožinu ze všech souborů pomocí linq nebo vlastnosti directory?
                FileInfo[] replicaParentDirFiles = replicaDirWithSubdirectories[0].GetFiles("*", SearchOption.TopDirectoryOnly);

                // TO-DO: refaktoring -> rozsekat do menších metod ať to není tak velká metoda

                // Case when source directory is empty
                if (sourceDirWithSubdirectories.Length == 1 && sourceParentDirFiles.Length == 0)
                {
                    log.Warn($"Source directory {inputParameters.SourceDirectory} is empty!");

                    // replicaDirWithSubdirectories.Length > 1 means there are subfolders. If there are files or subfolders we need to delete them.
                    if (replicaDirWithSubdirectories.Length > 1 || replicaParentDirFiles.Length > 0)
                    {
                        if (!Utils.ConfirmationDialog($"Target directory is not empty. Delete all files and directories in {inputParameters.ReplicaDirectory}? Y/N:", showDialog: inputParameters.AskDeleteConfirmation))
                            throw new Exception(Logging.msgActionCanceledByUser);

                        DeleteAllFilesAndFolders(replicaDirWithSubdirectories, log);
                    }
                    log.Info("Both directories are empty.");
                    return;
                }

                // Case when replica directory is empty and source is not.
                if (replicaDirWithSubdirectories.Length <= 1 && replicaParentDirFiles.Length == 0)
                {
                    log.Info($"Target directory {inputParameters.ReplicaDirectory} is empty. Proceeding to synchronize with directory {inputParameters.SourceDirectory}.");
                    foreach (DirectoryInfo sourceDirectory in sourceDirWithSubdirectories)
                    {
                        ReplicateDirectory(sourceDirectory, inputParameters, log);
                    }
                }

                // Case when both directories contain files
                FileCompare myFileCompare = new FileCompare(inputParameters);

                // Need to compare directories without parent (parent name may differ)
                string[] sourceRelativePaths = Utils.RelativePaths(sourceDirWithSubdirectories, inputParameters.SourceDirectory);
                string[] replicaRelativePaths = Utils.RelativePaths(replicaDirWithSubdirectories, inputParameters.ReplicaDirectory);
                var subDirsAreSame = sourceRelativePaths.SequenceEqual(replicaRelativePaths);

                IEnumerable<FileInfo> allSourceDirFiles = sourceDirWithSubdirectories[0].GetFiles("*", SearchOption.AllDirectories);
                IEnumerable<FileInfo> allReplicaDirFiles = replicaDirWithSubdirectories[0].GetFiles("*", SearchOption.AllDirectories);
                bool filesAreSame = allSourceDirFiles.SequenceEqual(allReplicaDirFiles, myFileCompare); // TO-DO: otestovat pro různý počet souborů apod.

                // If both directories and their files are same, no further action is needed.
                if (filesAreSame && subDirsAreSame)
                {
                    log.Info($"Folders {inputParameters.SourceDirectory} and {inputParameters.ReplicaDirectory} are same. No action is needed");
                    return;
                }

                log.Info($"Target directory {inputParameters.ReplicaDirectory} contains files.");
                if (!Utils.ConfirmationDialog($"All files in target directory {inputParameters.ReplicaDirectory} that do not match files from source directory {inputParameters.SourceDirectory} will be deleted. Proceed ? Y/N:", showDialog: inputParameters.AskDeleteConfirmation))
                    throw new Exception(Logging.msgActionCanceledByUser);

                // Directories to be created or deleted in replica
                IEnumerable<string> dirsToCreate = [];
                IEnumerable<string> dirsToDelete = [];
                // Files to be created or deleted
                IEnumerable<FileInfo> filesToCopy = [];
                IEnumerable<FileInfo> filesToDelete = [];

                dirsToCreate = (from path in sourceRelativePaths select path).Except(replicaRelativePaths);
                filesToCopy = (from file in allSourceDirFiles select file).Except(allReplicaDirFiles, myFileCompare);

                // First create directories and copy files
                if (dirsToCreate.Any())
                {
                    foreach (string dir in dirsToCreate)
                    {
                        CreateDirectory(Path.Combine(inputParameters.SourceDirectory, dir), log);
                    }
                }

                if (filesToCopy.Any())
                {

                }

                dirsToDelete = (from path in replicaRelativePaths select path).Except(sourceRelativePaths);
                filesToDelete = (from file in allReplicaDirFiles select file).Except(allSourceDirFiles, myFileCompare);

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

        private static void DeleteAllFilesAndFolders(DirectoryInfo[] directories, Logger log)
        {
            // Iterating trough directories in reverse order to remove subdirectories first.
            for (int i = directories.Length - 1; i >= 0; i--)
            {
                if (!directories[i].Exists)
                    throw new DirectoryNotFoundException();

                log.Info($"Deleting files in {directories[i].FullName}");
                FileInfo[] files = directories[i].GetFiles("*", SearchOption.TopDirectoryOnly);
                DeleteFiles(files, log);
                log.Info($"Files deleted. Deleting directory {directories[i].FullName} itself.");

                // Delete subdirectories
                if (i > 0)
                    Directory.Delete(directories[i].FullName);
            }
        }

        private static void ReplicateDirectory(DirectoryInfo directory, InputParameters inputParameters, Logger log)
        {
            string targetDirectory = directory.FullName.Replace(inputParameters.SourceDirectory, inputParameters.ReplicaDirectory);
            if (!Directory.Exists(targetDirectory))
            {
                CreateDirectory(targetDirectory, log);
            }
            FileInfo[] fileInfo = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            CopyFiles(fileInfo, targetDirectory, log);

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
            var directories = new List<DirectoryInfo>() { new DirectoryInfo(directoryPath) };
            foreach (DirectoryInfo subdirectoryInfo in directories[0].GetDirectories("*", SearchOption.AllDirectories))
            {
                directories.Add(subdirectoryInfo);
            }
            return directories.ToArray();
        }

        private static void CopyFiles(FileInfo[] files, string targetDirectory, Logger log)
        {
            foreach (FileInfo file in files)
            {
                string targetFileFullName = Path.Combine(targetDirectory, file.Name);
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

        private static void DeleteFiles(FileInfo[] files, Logger log)
        {
            foreach (FileInfo file in files)
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
