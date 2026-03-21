namespace TinyUrl.API.Helpers
{
    public static class ShortCodeGenerator
    {
        private static readonly Random _rng = new();
        private const string Chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate(int length = 6) =>
            new string(Enumerable.Range(0, length)
                .Select(_ => Chars[_rng.Next(Chars.Length)])
                .ToArray());
    }
}
