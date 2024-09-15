using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using YoutobeTool.Interfaces;
using YoutobeTool.Services;
using YoutobeTool.ViewModels;
using YoutobeTool.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YoutobeTool
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window window { get; private set; }
        public static IServiceProvider Services { get; private set; }
        public App()
        {
            this.InitializeComponent();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            window = new MainView();
            window.Activate();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDispatcherService, DispatcherService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<MusicViewModel>();
            services.AddSingleton<VideoViewModel>();
            services.AddSingleton<KeyViewModel>();
            services.AddSingleton<ChatGPTViewModel>();
        }
    }
}
