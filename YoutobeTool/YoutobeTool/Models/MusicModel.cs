using CommunityToolkit.Mvvm.ComponentModel;

namespace YoutobeTool.Models
{
    public partial class MusicModel : ObservableObject
    {
        [ObservableProperty]
        private string index;
        [ObservableProperty]
        private string nameFile;
        [ObservableProperty]
        private string pathFile;
        [ObservableProperty]
        private int duration;
        [ObservableProperty]
        private int rate;
        [ObservableProperty]
        private string status;

        public MusicModel(string index, string nameFile, string pathFile, int duration, string status)
        {
            this.Index = index;
            this.NameFile = nameFile;
            this.PathFile = pathFile;
            this.Duration = duration;
            this.Status = status;
        }
    }
}
