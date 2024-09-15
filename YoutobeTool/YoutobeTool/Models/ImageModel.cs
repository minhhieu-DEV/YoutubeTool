using CommunityToolkit.Mvvm.ComponentModel;

namespace YoutobeTool.Models
{
    public partial class ImageModel : ObservableObject
    {
        [ObservableProperty]
        private int index;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string path;
        [ObservableProperty]
        private string status;

        public ImageModel(int index, string name, string path)
        {
            Index = index;
            Name = name;
            Path = path;
        }
    }
}
