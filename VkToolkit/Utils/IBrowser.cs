namespace VkToolkit.Utils
{
    public interface IBrowser
    {
        string Authorize(string url, string email, string password);

        string GetJson(string url);
    }
}