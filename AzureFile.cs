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

    public static void SetSharePermissions() {

      CloudFileShare share = GetShare(SHARE);

      if (share.Exists()) {
        string policyName = "SharePolicy";

        SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy() {
          SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
          Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
        };

        FileSharePermissions permissions = share.GetPermissions();
        permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
        share.SetPermissions(permissions);

      }
    }

    public static void GenerateSAS(string filePath) {

      CloudFileDirectory rootDir = GetRootDirectory();
      CloudFile file = rootDir.GetFileReference(filePath);

      string sasToken = file.GetSharedAccessSignature(null, "SharePolicy");
      Uri fileSASUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);
      CloudFile fileSAS = new CloudFile(fileSASUri);
      
    }

    public static void Copy(string sourceFilePath, string destPath) {
      
      CloudFileDirectory rootDir = GetRootDirectory();      
      CloudFile sourceFile = rootDir.GetFileReference(sourceFilePath);

      if (!sourceFile.Exists()) {
        throw new Exception("I cant find the sourceFile!!!");
      }

      CloudFileDirectory destDirectory = rootDir.GetDirectoryReference(destPath);
      destDirectory.CreateIfNotExists();

      string destFileName = Path.GetFileName(sourceFilePath);
      CloudFile destFile = destDirectory.GetFileReference(destFileName);

      destFile.StartCopy(sourceFile);

    }

    public static void Move(string sourceFilePath, string destPath) {

      Copy(sourceFilePath, destPath);
      DeleteFile(sourceFilePath);
    }


  }

  }
