namespace TinyUrl.API.Models
{
    public class TinyUrl
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = "";
        public string ShortCode { get; set; } = "";
        public bool IsPrivate { get; set; }
        public int Clicks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
