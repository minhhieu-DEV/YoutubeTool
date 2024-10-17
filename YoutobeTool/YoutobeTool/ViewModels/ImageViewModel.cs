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
using YoutobeTool.Interfaces;
using YoutobeTool.Models;
using YoutobeTool.Services;

namespace YoutobeTool.ViewModels
{
    public partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ImageModel> imageModels;
        [ObservableProperty]
        private string pathFolderText;
        [ObservableProperty]
        private int numberTask;
        public IAsyncRelayCommand OnClickedChooseFolder { get; }
        public IAsyncRelayCommand OnClickedCreateImages { get; }
        private readonly IDialogService _dialogService;
        private readonly IDispatcherService _dispatcherService;
        private ApplicationDataContainer localSettings;

        public ImageViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            OnClickedChooseFolder = new AsyncRelayCommand(ClickedChooseFolder);
            OnClickedCreateImages = new AsyncRelayCommand(ClickedCreateImages);
            ImageModels = new ObservableCollection<ImageModel>();
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
            localSettings = ApplicationData.Current.LocalSettings;
        }
        private async Task ClickedChooseFolder()
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
                PathFolderText = folder.Path;
                HandleFileFromPath(folder.Path);
            }
        }
        private async Task ClickedCreateImages()
        {
            if (ImageModels.Count == 0)
            {
                await _dialogService.ShowMessageAsync("Vui lòng nhập dữ liệu!");
                return;
            }
            string domain = (string)localSettings.Values["Domain"];
            string apiKey = (string)localSettings.Values["ApiKey"];
            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(apiKey))
            {
                await _dialogService.ShowMessageAsync("Vui lòng nhập domain và api!");
                return;
            }
            var services = new ChatGPTService(domain, apiKey);
            SemaphoreSlim semaphore = new SemaphoreSlim(NumberTask == default || NumberTask <= 0 ? Environment.ProcessorCount / 2 : NumberTask);
            List<Task> tasks = new List<Task>();
            foreach (var item in ImageModels.AsEnumerable())
            {
                Task task = Task.Run(() =>
                {
                    _dispatcherService.EnqueueAsync(async () =>
                    {
                        await services.CreatePromtFromFileText(item, semaphore);
                    });
                });
                tasks.Add(task);

            }
            await Task.WhenAll(tasks);

        }
        private void HandleFileFromPath(string pathFolder)
        {
            ImageModels.Clear();
            string[] txtFiles = Directory.GetFiles(pathFolder, "*.txt", SearchOption.AllDirectories);
            for (int i = 0; i < txtFiles.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(txtFiles[i]);
                var imageModel = new ImageModel(i + 1, fileInfo.Name, txtFiles[i]);
                ImageModels.Add(imageModel);
            }
        }

    }
}
