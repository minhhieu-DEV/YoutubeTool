using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace YoutobeTool.Models
{
    public partial class FolderModel : ObservableObject
    {
        [ObservableProperty]
        private int index;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string path;
        [ObservableProperty]
        private List<ImageModel> images;
        [ObservableProperty]
        private MusicModel music;
        [ObservableProperty]
        private VideoModel video;
        [ObservableProperty]
        private string status;
    }
}
