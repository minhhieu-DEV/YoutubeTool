using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using YoutobeTool.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YoutobeTool.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KeyPage : Page
    {
        public KeyPage()
        {
            this.InitializeComponent();
            this.DataContext = App.Services.GetRequiredService<KeyViewModel>();
        }
    }
}
