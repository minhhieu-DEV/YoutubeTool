namespace YoutobeTool.Models
{

    public class Datum
    {
        public string url { get; set; }
    }
    public class ResponseImageModel
    {
        public int created { get; set; }
        public Datum[] data { get; set; }
    }
}
