using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutobeTool.Constants;
using YoutobeTool.Models;
namespace YoutobeTool.Helpers
{
    public class MusicHelper
    {
        public static void ListenVoiceDefault(VoiceItemModel voiceItemModel)
        {
            using (var audioFile = new AudioFileReader(voiceItemModel.PathVoiceExample))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        public static int GetDurationByAudio(string path)
        {
            using (var reader = new AudioFileReader(path))
            {
                return (int)reader.TotalTime.TotalSeconds;
            }
        }
        public static bool ConvertWavToMp3(string pathFileWav)
        {
            try
            {
                int index = pathFileWav.LastIndexOf('.');
                string pathFile = pathFileWav.Substring(0, index);
                var command = $"-i \"{pathFileWav}\" \"{pathFile}.mp3\"";
                return Helpers.GeneralHelper.ProcessCmd(GeneralConstant.PathFfmpeg, command);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task HandleCreateVoiceAsync(MusicModel musicModel, int rate, string voice, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                musicModel.Status = "Xử lý dữ liệu đầu vào...";
                await Task.Delay(1000);
                await File.WriteAllTextAsync(musicModel.PathFile, GeneralHelper.RemoveSpecialCharacters(await File.ReadAllTextAsync(musicModel.PathFile)));
                int lengthFile = musicModel.PathFile.LastIndexOf(".");
                string nameFileSpeech = musicModel.PathFile.Substring(0, lengthFile);
                musicModel.Status = "Bắt đầu tạo file .wav";
                await Task.Delay(1000);
                var content = new StringBuilder($"/c type \"{musicModel.PathFile}\"");
                content.Append($" | \"{GeneralConstant.PathPiper}\" -m \"{voice}\"  --length_scale {rate} -f \"{nameFileSpeech}.wav\"");
                if (!await GeneralHelper.ProcessCmdAsync("cmd.exe", content.ToString()))
                {
                    musicModel.Status = "Tạo file .wav thất bại!";
                    return;
                }
                musicModel.Status = "Tạo file .wav thành công!";
                await Task.Delay(1000);
                musicModel.Status = "Kiểm tra file .mp3!";
                await Task.Delay(1000);
                if (File.Exists($"{nameFileSpeech}.mp3"))
                {
                    File.Delete($"{nameFileSpeech}.mp3");
                }
                musicModel.Status = "Chuyển wav sang mp3";
                await Task.Delay(1000);
                if (ConvertWavToMp3($"{nameFileSpeech}.wav"))
                {
                    musicModel.Status = "Tạo file .mp3 thành công!";
                    File.Delete($"{nameFileSpeech}.wav");
                    musicModel.Duration = GetDurationByAudio($"{nameFileSpeech}.mp3");
                }
                else
                {
                    musicModel.Status = "Tạo file .mp3 thất bại!";
                }

            }
            catch (Exception ex)
            {
                musicModel.Status = ex.Message;
            }
            finally
            {
                // Giải phóng semaphore để task khác có thể chạy
                semaphore.Release();
            }
        }
        public static List<VoiceModel> GetDefaultVoiceAsync()
        {
            List<VoiceModel> voices = new List<VoiceModel>();
            if (!Directory.Exists(GeneralConstant.PathVoices))
            {
                return null;
            }
            string[] directories = Directory.GetDirectories(GeneralConstant.PathVoices);
            foreach (var item in directories)
            {
                string nameFolder = System.IO.Path.GetFileName(item);
                if (string.IsNullOrEmpty(nameFolder))
                {
                    continue;
                }
                VoiceModel voiceModel = new VoiceModel();
                voiceModel.VoiceName = GeneralHelper.CapitalizeFirstLetter(nameFolder);
                voiceModel.PathVoice = item;
                List<VoiceItemModel> voiceItemModels = new List<VoiceItemModel>();
                string[] voiceTypes = Directory.GetDirectories(item.ToString());
                if (voiceTypes.Length == 0 || voiceTypes == null)
                {
                    continue;
                }
                foreach (string voiceType in voiceTypes)
                {
                    string nameType = System.IO.Path.GetFileName(voiceType);
                    if (string.IsNullOrEmpty(nameType))
                    {
                        continue;
                    }
                    VoiceItemModel voiceItemModel = new VoiceItemModel();
                    voiceItemModel.VoiceType = GeneralHelper.CapitalizeFirstLetter(nameType);
                    voiceItemModel.PathModel = Directory.GetFiles(voiceType, "*.onnx")[0];
                    voiceItemModel.PathVoiceExample = Directory.GetFiles($"{voiceType}\\samples", "*.mp3")[0];
                    voiceItemModels.Add(voiceItemModel);
                }
                voiceModel.VoiceItems = voiceItemModels;
                voices.Add(voiceModel);
            }
            return voices;
        }
    }
}
