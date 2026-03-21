namespace TinyUrl.Blazor.Models
{
    public class TinyUrlEntry
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = "";
        public string ShortCode { get; set; } = "";
        public bool IsPrivate { get; set; }
        public int Clicks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
