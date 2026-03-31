using TinyUrl.API.Interfaces;

namespace TinyUrl.API.Helpers
{
    public class GuidShortCodeGenerator : IShortCodeGenerator
    {
        public string Generate(int length = 6) =>
            Guid.NewGuid().ToString("N")[..length];
    }
}