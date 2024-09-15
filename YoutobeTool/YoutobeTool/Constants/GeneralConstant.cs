namespace YoutobeTool.Constants
{
    public static class GeneralConstant
    {
        public static string PathVoices => $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data\\voices\\en";
        public static string PathPiper => $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data\\piper\\piper.exe";
        public static string PathFfmpeg => $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data\\ffmpeg.exe";
        public static string PathLog => $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Data\\logs";
        public const string Domain = "https://www.googleapis.com/drive/v3";
        public const string Api = "AIzaSyAMTDK0NpJm-Oh0R8S18A80dzLlf3eUxq0";
        public const string IdFile = "1zQGXLpItrBnXJP98wnTfIBCY5TxrEng0";

    }
}
