namespace TinyUrl.API.Interfaces
{
    public interface IShortCodeGenerator
    {
        string Generate(int length = 6);
    }
}