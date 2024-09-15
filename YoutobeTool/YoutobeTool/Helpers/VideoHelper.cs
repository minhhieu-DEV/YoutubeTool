using System;
using System.IO;
using System.Threading.Tasks;
using YoutobeTool.Constants;
using YoutobeTool.Models;

namespace YoutobeTool.Helpers
{
    public class VideoHelper
    {
        public async static Task CreateVideoTotal(FolderModel folder)
        {
            folder.Status = "Bắt đầu tạo video tổng";
            await Task.Delay(1000);
            if (File.Exists($"{folder.Path}\\video.mp4"))
            {
                File.Delete($"{folder.Path}\\video.mp4");
            }
            folder.Status = "Bắt đầu lấy file";
            await Task.Delay(1000);
            string[] videoFiles = Directory.GetFiles(folder.Path, "*.mp4");
            string filePath = $"{folder.Path}\\file.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in videoFiles)
                    {
                        writer.WriteLine($"file '{item}'");
                    }
                }
                folder.Status = "Đang tạo video tổng";
                await Task.Delay(1000);
                var command = $"-f concat -safe 0 -i \"{filePath}\" -c copy \"{folder.Path}\\video.mp4\"";
                bool isCreateVideo = Helpers.GeneralHelper.ProcessCmd(GeneralConstant.PathFfmpeg, command);
                if (!isCreateVideo)
                {
                    folder.Status = "Tạo video thất bại!";
                    await Task.Delay(1000);
                    return;
                }
                if (File.Exists($"{folder.Video.PathVideo}"))
                {
                    File.Delete($"{folder.Video.PathVideo}");
                }
                folder.Status = "Ghép nhạc vào video";
                await Task.Delay(1000);
                var loop = MusicHelper.GetDurationByAudio(folder.Music.PathFile) / (videoFiles.Length * 5);
                var commandMusic = $"-stream_loop {loop} -i \"{folder.Path}\\video.mp4\" -i \"{folder.Music.PathFile}\" -c:v copy -c:a aac -shortest \"{folder.Video.PathVideo}\"";
                bool isMusic = Helpers.GeneralHelper.ProcessCmd(GeneralConstant.PathFfmpeg, commandMusic);
                if (!isMusic)
                {
                    folder.Status = "Ghép nhạc thất bại!";
                    await Task.Delay(1000);
                }
                File.Delete(filePath);
                folder.Status = "Tạo video thành công!";
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                folder.Status = "Lỗi khi tạo tệp: " + ex.Message;
            }
        }
        public static async Task CreateVideoByImage(FolderModel folder, string filter)
        {
            try
            {
                folder.Status = "Bắt đầu";
                await Task.Delay(1000);
                foreach (var image in folder.Images)
                {
                    int lastIndexOfDot = image.Path.LastIndexOf('.');
                    string nameFile = image.Path.Substring(0, lastIndexOfDot);
                    folder.Status = "Kiểm tra file";
                    await Task.Delay(1000);
                    if (File.Exists($"{nameFile}.mp4"))
                    {
                        File.Delete($"{nameFile}.mp4");
                    }
                    folder.Status = "Đang tạo...";
                    await Task.Delay(1000);
                    var command = $"-loop 1 -i \"{image.Path}\" {filter} -c:v mpeg4 -t 5 -r 25 -crf 18 \"{nameFile}.mp4\"";
                    bool isSuccess = Helpers.GeneralHelper.ProcessCmd(GeneralConstant.PathFfmpeg, command);
                    if (isSuccess)
                    {
                        folder.Status = $"Tạo video {nameFile} thành công!";
                        await Task.Delay(1000);
                    }
                    else
                    {
                        folder.Status = $"Tạo video {nameFile} thất bại!";
                        await Task.Delay(1000);
                    }
                }
                folder.Status = "Hoàn thành!";
            }
            catch (Exception ex)
            {
                folder.Status = ex.Message;
            }
        }
    }
}
