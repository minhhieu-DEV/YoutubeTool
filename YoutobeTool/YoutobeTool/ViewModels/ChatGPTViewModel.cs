using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using YoutobeTool.Interfaces;
using YoutobeTool.Models;

namespace YoutobeTool.ViewModels
{
    public partial class ChatGPTViewModel : ObservableObject
    {
        [ObservableProperty]
        private string data;
        [ObservableProperty]
        private string pathFolder;
        [ObservableProperty]
        private ObservableCollection<PromtModel> promtModels;
        public IAsyncRelayCommand ChooseFolderClicked { get; }
        public IAsyncRelayCommand CreateImageClicked { get; }

        private readonly IDispatcherService dispatcherService;
        private readonly IDialogService dialogService;

        public ChatGPTViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            ChooseFolderClicked = new AsyncRelayCommand(ChooseFolderAsync);
            CreateImageClicked = new AsyncRelayCommand(CreateImageAsync);
            PromtModels = new ObservableCollection<PromtModel>();
            this.dialogService = dialogService;
            this.dispatcherService = dispatcherService;
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
            }
        }
        private async Task CreateImageAsync()
        {
            HandleDataFromText();
        }
        private void HandleDataFromText()
        {
            PromtModels.Clear();
            string[] txtFiles = Data.Split('\r');
            for (int i = 0; i < txtFiles.Length; i++)
            {
                PromtModels.Add(new PromtModel(i + 1, txtFiles[i]));
            }
        }
    }
}
