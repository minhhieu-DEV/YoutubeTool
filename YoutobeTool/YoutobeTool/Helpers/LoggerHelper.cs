using System;
using System.IO;
using YoutobeTool.Constants;

namespace YoutobeTool.Helpers
{
    public class LoggerHelper
    {
        public static void Log(string message)
        {
            if (!Directory.Exists(GeneralConstant.PathLog))
            {
                Directory.CreateDirectory(GeneralConstant.PathLog);
            }
            string logFilePath = $"{GeneralConstant.PathLog}\\log_{DateTime.Now:yyyyMMdd}.txt";
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}";
            using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(logMessage); // Thêm nội dung vào cuối tệp
                }
            }
            File.AppendAllText(logFilePath, logMessage);  // Ghi log vào file theo ngày
        }
    }
}
