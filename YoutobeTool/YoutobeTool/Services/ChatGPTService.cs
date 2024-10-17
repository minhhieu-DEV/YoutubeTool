using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YoutobeTool.Helpers;
using YoutobeTool.Interfaces;
using YoutobeTool.Models;

namespace YoutobeTool.Services
{
    public class ChatGPTService : IChatGPTService
    {
        public string Domain { get; set; }
        public string ApiKey { get; set; }
        public ChatGPTService(string domain, string api)
        {
            Domain = domain;
            ApiKey = api;
        }
        public async Task<bool> IsCheckApiChatGPT()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Set up the client with the authorization header
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                    // Send a GET request to the API
                    HttpResponseMessage response = await client.GetAsync($"{Domain}/models");

                    // Check the response status code
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task CreatePromtFromFileText(ImageModel imageModel, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            imageModel.Status = "Xử lý text....";
            await Task.Delay(1000);
            string contentFile = GeneralHelper.RemoveSpecialCharacters(System.IO.File.ReadAllText(imageModel.Path).Replace('\n', ' ').Replace('\r', ' '));
            int divContent = contentFile.Length / 36500 + (contentFile.Length % 36500 != 0 ? 1 : 0);
            string[] contents = GeneralHelper.SplitStringIntoChunks(contentFile, contentFile.Length / divContent);
            int numberPromt = divContent >= 6 ? 1 : 2;
            for (int i = 0; i < contents.Length; i++)
            {
                var item = contents[i];
                imageModel.Status = $"Tạo danh sách promts từ đoạn text {i + 1}...";
                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{Domain}/chat/completions");
                    request.Headers.Add("Authorization", $"Bearer {ApiKey}");
                    string contentString = "{\r\n  \"model\": \"gpt-4\",\r\n  \"messages\": [\r\n    {\r\n      \"role\": \"user\",\r\n      \"content\": \"Tôi gửi text truyện, hãy tạo giúp tôi " + numberPromt + " promt tạo ảnh bằng tiếng anh loại bỏ các từ nhạy cảm, bạo lực và gợi dục: '" + item + "'\"\r\n    }\r\n  ],\r\n  \"temperature\": 0.7\r\n}";
                    var content = new StringContent(contentString, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ResponseSummaryModel>(responseContent);
                        foreach (var choice in result.choices)
                        {
                            string[] promts = choice.message.content.Split('\n');
                            for (int j = 0; j < promts.Length; j++)
                            {
                                imageModel.Status = $"Tạo image từ promt {i + 1}...";
                                bool isCreateImage = await CreateImageFromPromt(promts[i], imageModel.Path);
                                if (isCreateImage)
                                {
                                    imageModel.Status = $"Tạo image từ promt {i + 1} thành công!";
                                }
                                imageModel.Status = $"Tạo image từ promt {i + 1} thất bại!";
                            }
                        }
                    }
                }
            }
        }
        public async Task<bool> CreateImageFromPromt(string promt, string path)
        {
            string pathDirectory = path.Substring(0, path.LastIndexOf('.'));
            if (!Directory.Exists(pathDirectory))
            {
                Directory.CreateDirectory(pathDirectory);
            }
            int nameFile = GeneralHelper.GetLastNameImage(Directory.GetFiles(pathDirectory));


            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{Domain}/images/generations");
                request.Headers.Add("Authorization", $"Bearer {ApiKey}");
                var content = new StringContent("{\r\n  \"prompt\": \"" + promt + "\",\r\n  \"n\": 1,\r\n  \"size\": \"1024x1024\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ResponseImageModel>(responseContent);
                    foreach (var choice in result.data)
                    {
                        return await GeneralHelper.DownloadFileAsync(choice.url, $"{pathDirectory}\\{nameFile + 1}.jpg");
                    }
                }
                return false;
            }
        }
    }
}
