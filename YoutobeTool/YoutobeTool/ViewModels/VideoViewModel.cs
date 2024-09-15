using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using YoutobeTool.Constants;
using YoutobeTool.Helpers;
using YoutobeTool.Interfaces;
using YoutobeTool.Models;

namespace YoutobeTool.ViewModels
{
    public partial class VideoViewModel : ObservableObject
    {
        #region Init
        [ObservableProperty]
        private string pathFolder;
        [ObservableProperty]
        private List<string> features;
        [ObservableProperty]
        private string feature;
        [ObservableProperty]
        private string status;
        [ObservableProperty]
        private ObservableCollection<FolderModel> folderModels;
        private DispatcherQueue _dispatcherQueue;
        public IAsyncRelayCommand ChooseFolderClicked { get; }
        public IAsyncRelayCommand CreateVideoByImageClicked { get; }
        public IAsyncRelayCommand CreateVideoTotalClicked { get; }

        private readonly IDialogService _dialogService;
        private readonly IDispatcherService _dispatcherService;

        public VideoViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            ChooseFolderClicked = new AsyncRelayCommand(ChooseFolderAsync);
            CreateVideoByImageClicked = new AsyncRelayCommand(CreateVideoByImage);
            FolderModels = new ObservableCollection<FolderModel>();
            Features = new List<string>()
            {
                "Scroll with black background",
                "Scroll bottom to top",
                "Zoom"
            };
            Feature = Features[0];
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
        }
        #endregion
        #region Event
        private async Task CreateVideoByImage()
        {
            if (FolderModels.Count == 0)
            {
                await _dialogService.ShowMessageAsync("Dữ liệu trống");
                return;
            }
            string filter = string.Empty;
            switch (Feature)
            {
                case "Scroll with black background":
                    filter = FilterConstant.BottomToTopBlack;
                    break;
                case "Zoom":
                    filter = FilterConstant.Zoom;
                    break;
                case "Scroll bottom to top":
                    filter = FilterConstant.BottomToTop;
                    break;
            }
            int numberThread = Environment.ProcessorCount / 2;
            await HandleCreateVideoFromImages(numberThread, filter);
        }
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
                PathFolder = folder.Path;
                HandleFolderFromPath(PathFolder);
            }


        }
        #endregion
        #region Handle Event
        private async Task HandleCreateVideoFromImages(int number, string filter)
        {
            var elements = TaskHelper<FolderModel>.SplitList(FolderModels.AsEnumerable(), number);
            foreach (var element in elements)
            {
                List<Task> tasks = new();
                foreach (var item in element)
                {
                    Task task = Task.Run(() =>
                    {
                        _dispatcherService.EnqueueAsync(async () =>
                        {
                            await VideoHelper.CreateVideoByImage(item, filter);
                            await VideoHelper.CreateVideoTotal(item);
                        });
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks.ToArray());
            }
        }
        private void HandleFolderFromPath(string path)
        {
            FolderModels.Clear();
            int index = 1;
            string[] subdirectories = Directory.GetDirectories(path);
            foreach (var subdirectorie in subdirectories)
            {
                var folder = new FolderModel();
                folder.Index = index;
                folder.Name = Path.GetFileName(subdirectorie);
                folder.Path = subdirectorie;
                List<ImageModel> images = new List<ImageModel>();
                string[] txtFiles = Directory.GetFiles(subdirectorie, "*.jpg");
                for (int i = 0; i < txtFiles.Length; i++)
                {
                    FileInfo fileInfo = new FileInfo(txtFiles[i]);
                    var image = new ImageModel(i + 1, fileInfo.Name, txtFiles[i]);
                    images.Add(image);
                }
                folder.Images = images;
                folder.Music = new MusicModel(1.ToString(), $"{folder.Name}.mp3", $"{subdirectorie}\\{folder.Name}.mp3", 0, "");
                folder.Video = new VideoModel(1.ToString(), $"{folder.Name}.mp4", $"{subdirectorie}\\{folder.Name}.mp4");
                FolderModels.Add(folder);
                index++;
            }
        }
        #endregion
    }
}
