namespace OzonPriceChecker
{
    public interface IWebPageParser
    {
        Product GetOzonProductInfoByLink(string productLink);
    }
}