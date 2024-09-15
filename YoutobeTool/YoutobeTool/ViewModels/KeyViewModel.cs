using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using YoutobeTool.Constants;
using YoutobeTool.Helpers;
using YoutobeTool.Interfaces;

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
        public IRelayCommand SaveDataClicked { get; set; }
        public IRelayCommand CheckKeyClicked { get; set; }
        public IRelayCommand CopyPathClicked { get; set; }
        public IRelayCommand InstallVoicesClicked { get; set; }

        //private readonly DriveHelper driveHelper;
        private ApplicationDataContainer localSettings;

        private readonly IDialogService dialogService;
        private readonly IDispatcherService _dispatcherService;

        public KeyViewModel(IDialogService dialogService, IDispatcherService dispatcherService)
        {
            //driveHelper = new DriveHelper();
            localSettings = ApplicationData.Current.LocalSettings;
            SaveDataClicked = new RelayCommand(SetData);
            CheckKeyClicked = new RelayCommand(SetData);
            InstallVoicesClicked = new RelayCommand(CopyPathInstall);
            CopyPathClicked = new RelayCommand(CopyPath);
            this.dialogService = dialogService;
            Status = "Đang kiểm tra các package...";
            IsActive = true;
            _dispatcherService = dispatcherService;
            _dispatcherService.Enqueue(() =>
            {
                GetData();
            });

        }
        private async void GetData()
        {
            Key = (string)localSettings.Values["Key"];
            Domain = (string)localSettings.Values["Domain"];
            ApiKey = (string)localSettings.Values["ApiKey"];
            Status = "Cài đặt voices...";
            await Task.Delay(1000);
            Status = await GeneralHelper.InstallVoices(localSettings);
            await Task.Delay(1000);
            IsActive = false;
            Status = "Kiểm tra hoàn tất!";
        }
        private void CopyPathInstall()
        {
            DataPackage dataPackage = new DataPackage();

            // Set the text as the content of the DataPackage
            dataPackage.SetText(Package.Current.InstalledLocation.Path);

            // Place the DataPackage on the clipboard
            Clipboard.SetContent(dataPackage);
        }
        private void CopyPath()
        {
            DataPackage dataPackage = new DataPackage();

            // Set the text as the content of the DataPackage
            dataPackage.SetText(GeneralConstant.PathVoices);

            // Place the DataPackage on the clipboard
            Clipboard.SetContent(dataPackage);
        }
        private void SetData()
        {
            localSettings.Values["Key"] = Key;
            localSettings.Values["Domain"] = Domain;
            localSettings.Values["ApiKey"] = ApiKey;
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
