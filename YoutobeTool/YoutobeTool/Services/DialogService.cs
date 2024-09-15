using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using YoutobeTool.Interfaces;

namespace YoutobeTool.Services
{
    public class DialogService : IDialogService
    {
        private readonly IDispatcherService _dispatcherService;

        public DialogService(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService ?? throw new ArgumentNullException(nameof(dispatcherService));
        }

        public async Task ShowMessageAsync(string message)
        {
            await _dispatcherService.EnqueueAsync(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = App.window.Content.XamlRoot,
                };
                await dialog.ShowAsync();
            });
        }

        public async Task<bool> ShowConfirmationAsync(string message)
        {
            bool result = false;
            await _dispatcherService.EnqueueAsync(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = message,
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    XamlRoot = App.window.Content.XamlRoot,
                };

                result = await dialog.ShowAsync() == ContentDialogResult.Primary;
            });

            return result;
        }
    }
}
