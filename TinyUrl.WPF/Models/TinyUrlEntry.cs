using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.WPF.Models
{
    public class TinyUrlEntry
    {
        public int Id { get; set; }
        public string ShortCode { get; set; } = "";
        public string ShortUrl { get; set; } = "";
        public string OriginalUrl { get; set; } = "";
        public bool IsPrivate { get; set; }
        public int Clicks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
