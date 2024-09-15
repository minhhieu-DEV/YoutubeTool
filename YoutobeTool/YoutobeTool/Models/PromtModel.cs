using CommunityToolkit.Mvvm.ComponentModel;

namespace YoutobeTool.Models
{
    public partial class PromtModel : ObservableObject
    {
        [ObservableProperty]
        private int index;
        [ObservableProperty]
        private string promt;
        [ObservableProperty]
        private string status;

        public PromtModel(int index, string promt)
        {
            Index = index;
            Promt = promt;
        }
    }
}
