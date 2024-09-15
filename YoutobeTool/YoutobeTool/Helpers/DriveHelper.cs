//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;

//namespace YoutobeTool.Helpers
//{

//    public class DriveHelper
//    {
//        //static string[] Scopes = { DriveService.Scope.Drive };
//        //private DriveService service { get; set; }
//        //public DriveHelper()
//        //{
//        //    try
//        //    {
//        //        UserCredential credential;
//        //        string pathKey = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json");
//        //        using (var stream = new FileStream(pathKey, FileMode.Open, FileAccess.Read))
//        //        {
//        //            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//        //                GoogleClientSecrets.FromStream(stream).Secrets,
//        //                Scopes,
//        //                "user",
//        //                CancellationToken.None,
//        //                new FileDataStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token.json"), true)).Result;
//        //        }
//        //        service = new DriveService(new BaseClientService.Initializer()
//        //        {
//        //            HttpClientInitializer = credential,
//        //            ApplicationName = "YoutubeTool"
//        //        });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Console.WriteLine(ex.Message);
//        //    }

//        //}
//        public void DownloadAndReadFile(string fileId)
//        {
//            //var request = service.Files.Get(fileId);
//            //var file = request.Execute();
//            //var fileName = file.Name;
//            //var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

//            //using (var stream = new FileStream(filePath, FileMode.Create))
//            //{
//            //    request.Download(stream);
//            //}

//            // Read the XLSX file using EPPlus or ClosedXML
//            // ...
//        }
//        public async Task<List<Google.Apis.Drive.v3.Data.File>> GetAllFilesAsync()
//        {
//            var files = new List<Google.Apis.Drive.v3.Data.File>();
//            string pageToken = null;

//            do
//            {
//                var request = service.Files.List();
//                request.PageSize = 1000; // Adjust page size as needed
//                request.PageToken = pageToken;

//                var response = await request.ExecuteAsync();
//                files.AddRange(response.Files);
//                pageToken = response.NextPageToken;
//            }
//            while (pageToken != null);

//            return files;
//        }
//    }
//}
