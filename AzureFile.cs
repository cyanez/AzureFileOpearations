using System;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using System.IO;

namespace AzureInitial
{
  class AzureFile {
    const string SHARE = "ontica";

    static private CloudFileClient GetConnection() {

      CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
         CloudConfigurationManager.GetSetting("StorageConnectionString"));

      return storageAccount.CreateCloudFileClient();
    }

    static private CloudFileShare GetShare(string share) {

      CloudFileClient fileClient = GetConnection();

      return fileClient.GetShareReference(share);
    }

    static private CloudFileDirectory GetRootDirectory() {
      CloudFileShare share = GetShare(SHARE);

      return share.GetRootDirectoryReference();
    }

    static public void UploadFile(string sourceFile, string path) {
      
      CloudFileDirectory rootDir = GetRootDirectory();
      CloudFileDirectory cloudFileDirectory = rootDir.GetDirectoryReference(path);
      cloudFileDirectory.CreateIfNotExists();

      string destFileName = Path.GetFileName(sourceFile);  

      CloudFile cloudFile = cloudFileDirectory.GetFileReference(destFileName);

      using (Stream fileStream = File.OpenRead(sourceFile)) {
        cloudFile.UploadFromStreamAsync(fileStream).Wait();
      }  

    }

    public static void DownloadFile(string sourceFilePath, string destinationPath) {

      string directory = Path.GetDirectoryName(sourceFilePath);
      string fileName = Path.GetFileName(sourceFilePath);

      CloudFileDirectory rootDir = GetRootDirectory();    
      CloudFile file = rootDir.GetFileReference(sourceFilePath);
      
      if (!file.Exists()) {
        throw new Exception("I cant find the file in the directory!!!");
      }

      file.DownloadToFileAsync(destinationPath + "\\" + fileName, System.IO.FileMode.OpenOrCreate).Wait();           
    }

    public static void DeleteFile(string filePath) {

      CloudFileDirectory rootDir = GetRootDirectory();
      CloudFile file = rootDir.GetFileReference(filePath);

      file.DeleteIfExists();
    }

  }
}
