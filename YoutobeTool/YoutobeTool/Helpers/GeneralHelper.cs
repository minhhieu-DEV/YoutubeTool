using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using YoutobeTool.Constants;
using YoutobeTool.Models;

namespace YoutobeTool.Helpers
{
    public class GeneralHelper
    {
        public static string RemoveSpecialCharacters(string str)
        {
            // Regex để giữ lại chỉ chữ cái (a-z, A-Z), số (0-9) và khoảng trắng
            return Regex.Replace(str, "[^a-zA-Z0-9\\s.,]", "");
        }
        public static string[] SplitStringIntoChunks(string str, int chunkSize)
        {
            List<string> chunks = new List<string>();
            int strLength = str.Length;
            for (int i = 0; i < strLength; i += chunkSize)
            {
                // Lấy một phần của chuỗi bắt đầu từ vị trí i, kích thước tối đa là chunkSize
                if (i + chunkSize > strLength)
                    chunkSize = strLength - i;  // Điều chỉnh nếu phần cuối chuỗi nhỏ hơn chunkSize

                chunks.Add(str.Substring(i, chunkSize));
            }
            return chunks.ToArray();
        }
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Chuyển chữ cái đầu thành chữ hoa và ghép với phần còn lại của chuỗi
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        public static bool ProcessCmd(string fileName, string command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = command;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<string> InstallVoices(ApplicationDataContainer localSettings)
        {
            var resultVersionVoice = await IsCheckVersionVoices();
            var versionSave = localSettings.Values.ContainsKey("versionVoice") ? DateTime.Parse((string)localSettings.Values["versionVoice"], null, System.Globalization.DateTimeStyles.RoundtripKind) : default;
            if (versionSave != resultVersionVoice)
            {
                if (Directory.Exists($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data"))
                {
                    Directory.Delete($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data", recursive: true);
                }
            }
            if (!Directory.Exists(GeneralConstant.PathVoices))
            {
                var isDownload = await GeneralHelper.DownloadFileAsync($"{GeneralConstant.Domain}/files/{GeneralConstant.IdFile}?alt=media&key={GeneralConstant.Api}", $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data.zip");
                if (!isDownload)
                {
                    return "Tải voice thất bại";
                }
                try
                {
                    ZipFile.ExtractToDirectory($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data.zip", $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data");
                    File.Delete($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data.zip");
                    localSettings.Values["versionVoice"] = resultVersionVoice.ToString("o");
                    return "Giải nén file voices thành công!";
                }
                catch
                {
                    return "Giải nén file voices thất bại!";
                }
            }
            return "Voices đã được cài đặt";
        }
        public static async Task<bool> DownloadFileAsync(string url, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        // Lấy Stream từ phản hồi
                        using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            // Mở file stream để ghi dữ liệu
                            using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                            {
                                byte[] buffer = new byte[8192]; // 8KB buffer
                                int bytesRead;

                                // Đọc dữ liệu từ response stream và ghi vào file stream theo từng phần
                                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public static async Task<bool> ProcessCmdAsync(string fileName, string arguments)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        LoggerHelper.Log(e.Data); // Xử lý đầu ra của lệnh
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        LoggerHelper.Log("Error: " + e.Data); // Xử lý lỗi (nếu có)
                    }
                };

                process.Exited += (sender, e) =>
                {
                    tcs.SetResult(true); // Khi tiến trình kết thúc, hoàn thành Task
                    process.Dispose();
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Đợi quá trình kết thúc (không chặn luồng)
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Lỗi khi chạy tiến trình: {ex.Message}");
                tcs.SetResult(false); // Hoàn thành với kết quả thất bại nếu có lỗi
                return false;
            }
        }

        public static async Task<DateTime> IsCheckVersionVoices()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{GeneralConstant.Domain}/files/{GeneralConstant.IdFile}?fields=modifiedTime&key={GeneralConstant.Api}";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<VoiceVersionModel>(responseBody);
                    return DateTime.Parse(result.modifiedTime);
                }
                catch (HttpRequestException e)
                {
                    return default;
                }
            }
        }
    }
}
