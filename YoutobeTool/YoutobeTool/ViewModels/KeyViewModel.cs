using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using YoutobeTool.Helpers;
using YoutobeTool.Interfaces;
using YoutobeTool.Services;
using YoutobeTool.Views;

namespace YoutobeTool.ViewModels
{
    public partial class KeyViewModel : ObservableObject
    {
        [ObservableProperty]
        private string key;
        [ObservableProperty]
        private string domain;
        [ObservableProperty]
        private string apiKey;
        [ObservableProperty]
        private string status;
        [ObservableProperty]
        private bool isActive;
        public IAsyncRelayCommand SaveDataClicked { get; set; }
        public IAsyncRelayCommand CheckKeyClicked { get; set; }
        public IAsyncRelayCommand ResetModelClicked { get; set; }
        public IAsyncRelayCommand IsCheckChatGPTClicked { get; set; }
        public IAsyncRelayCommand GetKeyClicked { get; set; }
        //private readonly DriveHelper driveHelper;
        private ApplicationDataContainer localSettings;

        private readonly IDialogService dialogService;
        private readonly IDispatcherService _dispatcherService;
        private ChatGPTService chatGPTService;

        public KeyViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            //driveHelper = new DriveHelper();
            localSettings = ApplicationData.Current.LocalSettings;
            SaveDataClicked = new AsyncRelayCommand(SetData);
            CheckKeyClicked = new AsyncRelayCommand(SetData);
            ResetModelClicked = new AsyncRelayCommand(ResetModel);
            IsCheckChatGPTClicked = new AsyncRelayCommand(IsCheckChatGPT);
            GetKeyClicked = new AsyncRelayCommand(GetKey);
            this.dialogService = dialogService;
            Status = "Đang kiểm tra các package...";
            IsActive = true;
            _dispatcherService = dispatcherService;
            _dispatcherService.Enqueue(() =>
            {
                GetData();

            });
        }

        private async Task ResetModel()
        {
            try
            {
                if (Directory.Exists($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data"))
                {
                    Directory.Delete($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data", true);
                    GetData();
                }
                else
                {
                    await dialogService.ShowMessageAsync("Folder voice không tồn tại.");
                }
            }
            catch
            {
                await dialogService.ShowMessageAsync("Folder voice đang được sử dụng không thể xóa.");
            }
        }

        private async Task GetKey()
        {
            try
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.SetText(GeneralHelper.GetKey());
                Clipboard.SetContent(dataPackage);
                await dialogService.ShowMessageAsync("Copy key thành công");
            }
            catch
            {
                await dialogService.ShowMessageAsync("Copy key thất bại");
            }
        }

        private async Task IsCheckChatGPT()
        {
            IsActive = true;
            Status = "Đang kiểm tra...";
            await Task.Delay(1000);
            var result = await chatGPTService.IsCheckApiChatGPT();
            if (result)
            {
                Status = "Kết nối thành công!";
            }
            else
            {
                Status = "Kết nối thất bại!";
            }
            IsActive = false;
        }
        private async void GetData()
        {
            Key = (string)localSettings.Values["Key"];
            Domain = (string)localSettings.Values["Domain"];
            ApiKey = (string)localSettings.Values["ApiKey"];
            Status = "Đang cài đặt voices . . .";
            await Task.Delay(1000);
            Status = await GeneralHelper.InstallVoices(localSettings);
            await Task.Delay(1000);
            Status = "Kiểm tra hoàn tất!";
            chatGPTService = new ChatGPTService(Domain, ApiKey);
            Status = "Kiểm tra key . . .";
            bool isKey = await GeneralHelper.IsCheckKey(Key);
            if (isKey)
            {
                Status = "Key hợp lệ!";
            }
            else
            {
                Status = "Key không hợp lệ!";
            }
            var mainWindow = App.window as MainView;
            mainWindow.EnableNavigate(isKey);
            IsActive = false;
        }
        private async Task SetData()
        {
            try
            {
                localSettings.Values["Key"] = Key;
                localSettings.Values["Domain"] = Domain;
                localSettings.Values["ApiKey"] = ApiKey;
                await dialogService.ShowMessageAsync("Lưu data thành công");
                GetData();
            }
            catch
            {
                await dialogService.ShowMessageAsync("Lưu data thất bại");
            }

        }
        private bool IsCheckKey()
        {
            if (string.IsNullOrEmpty(ApiKey))
            {
                return false;
            }
            return true;


        }
    }
}
