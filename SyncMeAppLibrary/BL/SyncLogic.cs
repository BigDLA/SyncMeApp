using SyncMeAppLibrary.Model;
using System;

namespace SyncMeAppLibrary.BL
{
    public class SyncLogic
    {
        public static void ReplicateSourceFolder(InputParameters inputParameters)
        {
            FolderItem[] sourceFolderFiles = FolderItem.LoadAllFiles(inputParameters.SourceFolder);
            FolderItem[] replicaFolderFiles = FolderItem.LoadAllFiles(inputParameters.SourceFolder);

            if (replicaFolderFiles.Length == 0)
            {
                CopyFiles(sourceFolderFiles, inputParameters.ReplicaFolder);
                CheckReplicaFolder(sourceFolderFiles, inputParameters);
                return;               
            }

        }

        private static void CheckReplicaFolder(FolderItem[] sourceFolderFiles, InputParameters inputParameters)
        {
            try 
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
            catch (Exception ex)
            {
                throw;
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
            try
            {
               /* if (replicaFolder.EndsWith("\\"))
                    {
                       replicaFolder = replicaFolder.Remove(replicaFolder.Length - 1);
                    }*/
                foreach (FolderItem item in sourceFiles)
                {
                    File.Copy(item.Path, $"{replicaFolder}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
