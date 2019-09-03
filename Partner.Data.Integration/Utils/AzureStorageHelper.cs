using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage type
using System.Configuration;

using System.IO;

namespace Partner.Data.Integration.Utils
{
    public class AzureStorageHelper
    {
        static string containerName = AppSettings.AzureStorageContainerName;
        static string connectionName = AppSettings.AzureStorageConnectionName;

        static CloudStorageAccount storageAccount = null;
        static CloudBlobClient blobClient = null;
        static CloudBlobContainer container = null;
        static AzureStorageHelper()
        {
            storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(connectionName));

            blobClient = storageAccount.CreateCloudBlobClient();

            container = blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
            //container.SetPermissions(
            //    new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }


        /// <summary>
        /// upload blob to window azure container folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="azureItemKey"></param>
        /// <param name="azureBolb"></param>
        public static void UploadBlobToAzure(string folderPath, string azureItemKey, Stream azureBolb)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(azureItemKey);
                blockBlob.UploadFromStream(azureBolb);
            }
            else
            {
                CreateFolder(folderPath);

                CloudBlobDirectory folderBlob = GetFolderBlobObject(folderPath);
                CloudBlockBlob blockBlob = folderBlob.GetBlockBlobReference(azureItemKey);
                blockBlob.UploadFromStream(azureBolb);
            }
        }

        /// <summary>
        /// get file content from azure storage 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> ListAllBlobFromAzure(string folderPath, string fileExtension)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (string.IsNullOrEmpty(folderPath))
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        if (blob.Name.ToLower().EndsWith(fileExtension.ToLower()))
                        {
                            byte[] blobItemByte = DownloadBlobFromAzure(folderPath, blob.Name);

                            result.Add(new KeyValuePair<string, string>(blob.Name, Encoding.UTF8.GetString(blobItemByte)));
                        }
                    }
                }
            }
            else
            {
                CloudBlobDirectory folderBlob = GetFolderBlobObject(folderPath);
                foreach (IListBlobItem item in folderBlob.ListBlobs(false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        if (blob.Name.ToLower().EndsWith(fileExtension.ToLower()))
                        {
                            byte[] blobItemByte = DownloadBlobFromAzure(folderPath, blob.Name);

                            result.Add(new KeyValuePair<string, string>(blob.Name, Encoding.UTF8.GetString(blobItemByte)));
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// download blob from window azure
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="azureItemKey"></param>
        /// <returns></returns>
        public static byte[] DownloadBlobFromAzure(string folderPath, string azureItemKey)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                CloudBlockBlob blockBlob = null;
                if (string.IsNullOrEmpty(folderPath))
                {
                    blockBlob = container.GetBlockBlobReference(azureItemKey);
                }
                else
                {
                    CloudBlobDirectory folderBlob = GetFolderBlobObject(folderPath);

                    blockBlob = folderBlob.GetBlockBlobReference(azureItemKey);
                }
                blockBlob.DownloadToStream(ms);
                ms.Position = 0;
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        /// <summary>
        /// delete blod from window azure
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="azureItemKey"></param>
        public static void DeleteBlob(string folderPath, string azureItemKey)
        {
            CloudBlockBlob blockBlob = null;
            if (string.IsNullOrEmpty(folderPath))
            {
                blockBlob = container.GetBlockBlobReference(azureItemKey);
            }
            else
            {
                CloudBlobDirectory folderBlob = GetFolderBlobObject(folderPath);
                blockBlob = folderBlob.GetBlockBlobReference(azureItemKey);
            }
            blockBlob.DeleteIfExists();
        }

        /// <summary>
        /// Get the object for Azure storage folder/Subfolder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns>if folder/subfolder existing, return the the object for Azure storage folder/Subfolder, else return null</returns>
        private static CloudBlobDirectory GetFolderBlobObject(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/");
            string[] allFolders = folderPath.Split('/');

            CloudBlobDirectory rootFolderObject = null;
            if (container.GetDirectoryReference(allFolders[0]).ListBlobs().Count() > 0)
            {
                rootFolderObject = container.GetDirectoryReference(allFolders[0]);
            }
            if (rootFolderObject != null && allFolders.Length > 1)
            {
                for (int i = 1; i < allFolders.Length; i++)
                {
                    string folderName = allFolders[i];
                    if (rootFolderObject.GetDirectoryReference(folderName).ListBlobs().Count() > 0)
                    {
                        rootFolderObject = rootFolderObject.GetDirectoryReference(folderName);
                    }
                    else
                    {
                        rootFolderObject = null;
                        break;
                    }
                }
            }

            return rootFolderObject;
        }
        /// <summary>
        /// create folder/subfolder in azure storage containner
        /// </summary>
        /// <param name="folder"></param>
        public static void CreateFolder(string folder)
        {
            folder = folder.Replace("\\", "/");
            string[] allFolders = folder.Split('/');


            CloudBlobDirectory rootFolderBlob = null;
            //create the root folder
            if (container.GetDirectoryReference(allFolders[0]).ListBlobs().Count() == 0)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(allFolders[0]);
                blockBlob.UploadFromByteArray(new byte[0], 0, 0);
            }
            rootFolderBlob = container.GetDirectoryReference(allFolders[0]);


            //create all sub folder if it is required
            for (int i = 1; i < allFolders.Length; i++)
            {
                string folderName = allFolders[i];
                if (rootFolderBlob.GetDirectoryReference(folderName).ListBlobs().Count() == 0)
                {
                    CloudBlockBlob blockBlob = rootFolderBlob.GetBlockBlobReference(folderName);
                    blockBlob.UploadFromByteArray(new byte[0], 0, 0);
                }
                rootFolderBlob = rootFolderBlob.GetDirectoryReference(folderName);
            }
        }

        /// <summary>
        /// Delete folder in azure storage containner
        /// </summary>
        /// <param name="folder"></param>
        public static void DeleteFolder(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/");
            if (folderPath.EndsWith("/"))
                folderPath += "/";


            CloudBlobDirectory folder = GetFolderBlobObject(folderPath);
            if (folder == null)
            {
                throw new Exception("The folder is not existing.");
            }
            else
            {
                foreach (IListBlobItem blob in container.GetDirectoryReference(folderPath).ListBlobs(true))
                {
                    if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                    {
                        ((CloudBlob)blob).DeleteIfExists();
                    }
                }
            }
        }
    }
}