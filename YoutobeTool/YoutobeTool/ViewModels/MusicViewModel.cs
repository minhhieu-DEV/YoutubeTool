using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using YoutobeTool.Helpers;
using YoutobeTool.Interfaces;
using YoutobeTool.Models;

namespace YoutobeTool.ViewModels
{
    public partial class MusicViewModel : ObservableObject
    {

        #region Init
        [ObservableProperty]
        private string pathTexts;
        [ObservableProperty]
        private List<VoiceModel> voices;
        [ObservableProperty]
        private ObservableCollection<VoiceItemModel> voiceItemModels;
        [ObservableProperty]
        private int voiceIndex;
        partial void OnVoiceIndexChanged(int value)
        {
            ChangeVoice();
        }
        [ObservableProperty]
        private int voiceItemIndex;
        [ObservableProperty]
        private int rate = 1;
        [ObservableProperty]
        private int numberTask;
        [ObservableProperty]
        private ObservableCollection<MusicModel> musicModels;
        public IAsyncRelayCommand ChooseFolderClicked { get; }
        public IAsyncRelayCommand TestVoiceClicked { get; }
        public IAsyncRelayCommand CreateVoiceClicked { get; }

        private readonly IDialogService _dialogService;
        private readonly IDispatcherService _dispatcherService;

        public MusicViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            ChooseFolderClicked = new AsyncRelayCommand(ChooseFolderAsync);
            TestVoiceClicked = new AsyncRelayCommand(TestVoice);
            CreateVoiceClicked = new AsyncRelayCommand(CreateVoice);
            MusicModels = new ObservableCollection<MusicModel>();
            Voices = new List<VoiceModel>();
            VoiceItemModels = new ObservableCollection<VoiceItemModel>();
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
            GetDataVoices();


        }
        #endregion
        #region Event
        private async Task ChooseFolderAsync()
        {
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();
            var window = App.window;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                PathTexts = folder.Path;
                HandleFileFromPath(folder.Path);
            }
        }

        private async Task GetDataVoices()
        {
            var result = MusicHelper.GetDefaultVoiceAsync();
            if (result != null)
            {
                Voices.AddRange(result);
                VoiceIndex = 0;
            }
            await Task.Run(() =>
            {
                ChangeVoice();
            });
        }
        private void ChangeVoice()
        {
            if (Voices.Count == 0)
            {
                return;
            }
            var voiceTam = Voices[VoiceIndex];
            if (voiceTam != null)
            {
                VoiceItemModels.Clear();
                foreach (var item in voiceTam.VoiceItems)
                {
                    VoiceItemModels.Add(item);
                }
                VoiceItemIndex = 0;
            }
        }

        private async Task TestVoice()
        {
            MusicHelper.ListenVoiceDefault(VoiceItemModels[VoiceItemIndex]);
        }
        private async Task CreateVoice()
        {
            await HandleCreateVoices(Rate, VoiceItemModels[VoiceItemIndex].PathModel);
        }
        #endregion
        private void HandleFileFromPath(string pathFolder)
        {
            MusicModels.Clear();
            string[] txtFiles = Directory.GetFiles(pathFolder, "*.txt", SearchOption.AllDirectories);
            for (int i = 0; i < txtFiles.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(txtFiles[i]);
                var musicModel = new MusicModel((i + 1).ToString(), fileInfo.Name, txtFiles[i], 0, "");
                MusicModels.Add(musicModel);
            }
        }
        private async Task HandleCreateVoices(int rate, string voice)
        {
            if (MusicModels.Count == 0)
            {
                await _dialogService.ShowMessageAsync("Dữ liệu trống");
                return;
            }
            SemaphoreSlim semaphore = new SemaphoreSlim(NumberTask == default || NumberTask <= 0 ? Environment.ProcessorCount / 2 : NumberTask);
            List<Task> tasks = new List<Task>();
            foreach (var item in MusicModels.AsEnumerable())
            {
                Task task = Task.Run(() =>
                {
                    _dispatcherService.EnqueueAsync(async () =>
                    {
                        await Helpers.MusicHelper.HandleCreateVoiceAsync(item, rate, voice, semaphore);
                    });
                });
                tasks.Add(task);

            }
            await Task.WhenAll(tasks);

        }


    }
}
