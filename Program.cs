using System;

namespace AzureInitial
{
  class Program
  {
    static void Main(string[] args) {

      //AzureFile.UploadFile(@"D:\varios\REPORTE.doc", @"images/marian/img");
      try {
        AzureFile.DownloadFile(@"images/marian/img/adry.JPG", "D:");
        Console.WriteLine("Downloaded File!!!");
      } catch (Exception e) {
        Console.WriteLine(e);
      }
     
     
      Console.ReadLine();

    }
  
  }
}
