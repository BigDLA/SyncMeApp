using NLog;
using SyncMeAppLibrary.Model;
using System;
using System.Diagnostics;

namespace SyncMeAppLibrary.BL
{
    //logger.Info("Message with {myProperty}", "myValue");

    public class SyncLogic
    {
        public static void ReplicateSourceFolder(InputParameters inputParameters, Logger log)
        {
            try
            {
                log.Info($"Loading source files from {inputParameters.SourceFolder}");
                FolderItem[] sourceFolderFiles = FolderItem.LoadAllFiles(inputParameters.SourceFolder);
                
                log.Info($"Loading files in target folder {inputParameters.ReplicaFolder}");
                FolderItem[] replicaFolderFiles = FolderItem.LoadAllFiles(inputParameters.ReplicaFolder);

                if (sourceFolderFiles.Length == 0)
                {
                    log.Warn($"Source folder {inputParameters.SourceFolder} is empty!");

                    if (replicaFolderFiles.Length > 0)
                    {
                        DeleteFiles(replicaFolderFiles, inputParameters.ReplicaFolder);
                        log.Info(Logging.msgSyncCheck);
                        CheckReplicaFolder(sourceFolderFiles, inputParameters);
                        return;
                    }
                }

                if (replicaFolderFiles.Length == 0)
                {
                    log.Info("No files have been found in target folder. Proceeding with copying files from source folder.");
                    CopyFiles(sourceFolderFiles, inputParameters.ReplicaFolder);
                    log.Info(Logging.msgSyncCheck);
                    CheckReplicaFolder(sourceFolderFiles, inputParameters);
                    return;               
                }
            }
            catch (Exception ex) 
            {
                log.Error(ex);
                throw;
            }
        }

        private static void CheckReplicaFolder(FolderItem[] sourceFolderFiles, InputParameters inputParameters)
        {
            FolderItem[] replicaFolderFiles = FolderItem.LoadAllFiles(inputParameters.SourceFolder);
            foreach (FolderItem item in sourceFolderFiles)
            {
                if (CompareFiles(item, replicaFolderFiles))
                {
                    break;
                }
                else 
                {
                    throw new Exception($"File validation error! Item {item.Path} not found in replica folder ({inputParameters.ReplicaFolder}).");
                }
            }
            
        }

        private static bool CompareFiles(FolderItem item, FolderItem[] replicaFolderFiles)
        {
            byte[] sourceItemHash = FolderItem.GetFilesHash([item])[0];
            List<byte[]> replicaItemsHashes = FolderItem.GetFilesHash(replicaFolderFiles);
            return replicaItemsHashes.Contains(sourceItemHash);
        }

        private static void CopyFiles(FolderItem[] sourceFiles, string replicaFolder)
        {
            if (!Directory.Exists(replicaFolder))
                throw new Exception($"Target folder {replicaFolder} not found!");  // TO-DO?:Udělat custom exeptiony? abych neopakoval

            foreach (FolderItem item in sourceFiles)
            {
                File.Copy(item.Path, $"{replicaFolder}");

                //TO-DO?: možná bude potřeba zvlášť copy pro složky

            }
        }

        private static void DeleteFiles(FolderItem[] replicaFolderFiles, string replicaFolder)
        {
            if (Utils.ConfirmationDialog(Logging.msgDeleteConfirmation))
            {
                if (Directory.Exists(replicaFolder))
                {
                    foreach (FolderItem item in replicaFolderFiles)
                    {
                        File.Delete(item.Path);

                        //TO-DO?: možná bude potřeba zvlášť delete pro složky
                    }
                }
                else
                {
                    throw new Exception($"Target folder {replicaFolder} not found!");
                }
            }
            throw new Exception(Logging.msgActionCanceledByUser);
        }
    }
}
