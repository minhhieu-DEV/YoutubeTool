namespace YoutobeTool.Constants
{
    public static class FilterConstant
    {
        public const string BottomToTopBlack = "-filter_complex \"color=black:1920x1080[b];[b][0]overlay=y=-'t*h*0.02'\"";
        public const string RightToLeft = "-filter_complex \"color=black:1920x1080[b];[b][0]overlay=x=-'t*h*0.02'\"";
        public const string Zoom = "-vf \"zoompan=z='zoom+0.002':x='iw/2-(iw/zoom/2)':y='ih-(ih/zoom)':d=125\"";
        public const string BottomToTop = "-vf \"scale=1920:1024,scroll=vertical=0.001\"";
    }
}
