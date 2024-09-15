using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace YoutobeTool.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Window
    {
        public VideoPage VideoPage { get; set; }
        public ImagePage ImagePage { get; set; }
        public MusicPage MusicPage { get; set; }
        public KeyPage KeyPage { get; set; }
        public ChatGPTPage ChatGPTPage { get; set; }

        public MainView()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            KeyPage = new KeyPage();
            VideoPage = new VideoPage();
            ImagePage = new ImagePage();
            MusicPage = new MusicPage();
            ChatGPTPage = new ChatGPTPage();
            ContentFrame.Navigate(typeof(KeyPage));
        }

        private void myNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            switch ((string)args.InvokedItemContainer.Tag)
            {
                case "Video":
                    ContentFrame.Navigate(typeof(VideoPage));
                    break;
                case "Music":
                    ContentFrame.Navigate(typeof(MusicPage));
                    break;
                case "Image":
                    ContentFrame.Navigate(typeof(ImagePage));
                    break;
                case "Settings":
                    ContentFrame.Navigate(typeof(KeyPage));
                    break;
                case "ChatGPT":
                    ContentFrame.Navigate(typeof(ChatGPTPage));
                    break;
            }
        }


    }
}
