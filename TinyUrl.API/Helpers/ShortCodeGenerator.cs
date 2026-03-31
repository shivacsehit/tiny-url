using TinyUrl.API.Interfaces;

namespace TinyUrl.API.Helpers
{
    public  class ShortCodeGenerator : IShortCodeGenerator
    {
        private static readonly Random _rng = new();
        private const string Chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public  string Generate(int length = 6) =>
            new string(Enumerable.Range(0, length)
                .Select(_ => Chars[_rng.Next(Chars.Length)])
                .ToArray());
    }
}
