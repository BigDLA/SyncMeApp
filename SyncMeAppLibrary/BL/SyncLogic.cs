using NLog;
using SyncMeAppLibrary.Model;
using System;
using System.Diagnostics;
using System.IO;

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
                if (sourceDirWithSubdirectories.Length == 0 ) 
                    throw new Exception($"Source directory {inputParameters.SourceDirectory} not found!");

                DirectoryInfo[] replicaDirWithSubdirectories = GetDirAndSubdirectories(inputParameters.ReplicaDirectory);

                // Case when source directory is empty
                if (sourceDirWithSubdirectories.Length == 1 && sourceDirWithSubdirectories[0].GetFiles("*", SearchOption.TopDirectoryOnly).Length == 0)
                {
                    log.Warn($"Source directory {inputParameters.SourceDirectory} is empty!");

                    // replicaDirWithSubdirectories.Length > 1 means there are subfolders. If there are files or subfolders we need to delete them.
                    if (replicaDirWithSubdirectories.Length > 1 || replicaDirWithSubdirectories[0].GetFiles("*", SearchOption.TopDirectoryOnly).Length > 0)
                    {
                        if (!Utils.ConfirmationDialog($"Target directory is not empty. Delete all files and directories in {inputParameters.ReplicaDirectory}? Y/N:", showDialog: inputParameters.AskDeleteConfirmation))
                            throw new Exception(Logging.msgActionCanceledByUser);

                        DeleteAllFilesAndFolders(replicaDirWithSubdirectories, log);
                        
                    }
                    log.Info("Both directories are now empty.");
                    return;
                }

                // Case when replica directory is empty and source is not.
                if (replicaDirWithSubdirectories.Length <= 1 && replicaDirWithSubdirectories[0].GetFiles("*", SearchOption.TopDirectoryOnly).Length == 0)
                {
                    log.Info($"Target directory {inputParameters.ReplicaDirectory} is empty. Proceeding to synchronize with directory {inputParameters.SourceDirectory}.");
                    foreach (DirectoryInfo sourceDirectory in sourceDirWithSubdirectories)
                    {
                        ReplicateDirectory(sourceDirectory, inputParameters, log);
                    }
                }

                //*

                /*
                foreach (DirectoryInfo sourceDirectory in sourceDirWithSubdirectories) 
                {
                    ReplicateDirectory(sourceDirectory, inputParameters, log);
                }
                
                */


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
            for (int i = directories.Length - 1; i>=0; i--)
            {
                if (!directories[i].Exists)
                    throw new DirectoryNotFoundException();

                log.Info($"Deleting files in {directories[i].FullName}");
                FileInfo[] files = directories[i].GetFiles("*", SearchOption.TopDirectoryOnly);
                DeleteFiles(files);
                log.Info($"Files deleted. Deleting directory {directories[i].FullName} itself.");
                
                // Delete subdirectories
                if (i > 0)
                    Directory.Delete(directories[i].FullName);
            }
        }

        private static void ReplicateDirectory(DirectoryInfo directory, InputParameters inputParameters, Logger log)
        {
            if (!directory.Exists)
                throw new Exception($"Source directory {directory.FullName} not found!");

            string targetDirectory = directory.FullName.Replace(inputParameters.SourceDirectory, inputParameters.ReplicaDirectory);
            if (!Directory.Exists(targetDirectory))
            {
                log.Info($"Creating target directory {targetDirectory} because it doesn´t exist.");
                Directory.CreateDirectory(targetDirectory);
            }
            FileInfo[] fileInfo = directory.GetFiles("*", SearchOption.TopDirectoryOnly);
            CopyFiles(fileInfo, targetDirectory, log);

        }

        private static DirectoryInfo[] GetDirAndSubdirectories(string directoryPath)
        {
            var directories =  new List<DirectoryInfo>() {new DirectoryInfo(directoryPath) };
            foreach (DirectoryInfo subdirectoryInfo in directories[0].GetDirectories("*", SearchOption.AllDirectories))
            {
                directories.Add(subdirectoryInfo);
            }
            return directories.ToArray();
        }

       /* private static void CheckReplicaDirectory(DirectoryItem[] sourceDirectoryFiles, InputParameters inputParameters)
        {
            DirectoryItem[] replicaDirectoryFiles = DirectoryItem.LoadAllFiles(inputParameters.SourceDirectory);
            foreach (DirectoryItem item in sourceDirectoryFiles)
            {
                if (CompareFiles(item, replicaDirectoryFiles))
                {
                    break;
                }
                else 
                {
                    throw new Exception($"File validation error! Item {item.Path} not found in replica directory ({inputParameters.ReplicaDirectory}).");
                }
            }
            
        }*/
       /*
        private static bool CompareFiles(DirectoryItem item, DirectoryItem[] replicaDirectoryFiles)
        {
            byte[] sourceItemHash = DirectoryItem.GetFilesHash([item])[0];
            List<byte[]> replicaItemsHashes = DirectoryItem.GetFilesHash(replicaDirectoryFiles);
            return replicaItemsHashes.Contains(sourceItemHash);
        }
       */
        private static void CopyFiles(FileInfo[] files, string targetDirectory, Logger log)
        {
            foreach (FileInfo file in files)
            {
                string targetFileFullName = Path.Combine(targetDirectory, file.Name);
                File.Copy(file.FullName, targetFileFullName);
                log.Info($"File {file.FullName} copied to {targetFileFullName}.");
            }
        }

        private static void DeleteFiles(FileInfo[] files)
        {
            foreach (FileInfo file in files)
            {
                File.Delete(file.FullName);
            }
        }
    }
}
