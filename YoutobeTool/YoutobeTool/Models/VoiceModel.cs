using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace YoutobeTool.Models
{
    public partial class VoiceModel : ObservableObject
    {
        [ObservableProperty]
        private string pathVoice;
        [ObservableProperty]
        private string voiceName;
        [ObservableProperty]
        private List<VoiceItemModel> voiceItems;
    }
    public partial class VoiceItemModel : ObservableObject
    {
        [ObservableProperty]
        private string pathModel;
        [ObservableProperty]
        private string pathVoiceExample;
        [ObservableProperty]
        private string voiceType;
    }
}
