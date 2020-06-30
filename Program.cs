using System;

namespace AzureInitial
{
  class Program
  {
    static void Main(string[] args) {

      AzureFile.UploadFile(@"D:\varios\REPORTE.doc", @"images/marian/img");
      Console.WriteLine("Upload File!!!");   
      Console.ReadLine();

    }
  
  }
}
