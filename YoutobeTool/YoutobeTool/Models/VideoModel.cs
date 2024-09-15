using CommunityToolkit.Mvvm.ComponentModel;

namespace YoutobeTool.Models
{
    public partial class VideoModel : ObservableObject
    {
        [ObservableProperty]
        private string index;
        [ObservableProperty]
        private string nameVideo;
        [ObservableProperty]
        private string pathVideo;
        [ObservableProperty]
        private string status;

        public VideoModel(string index, string nameVideo, string pathVideo)
        {
            this.index = index;
            this.nameVideo = nameVideo;
            this.pathVideo = pathVideo;
        }
    }
}
