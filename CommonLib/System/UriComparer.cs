namespace Libx;
public class UriComparer
{
    string baseUrl;
    public UriComparer(string baseUrl)
    {
        this.baseUrl = UriX.NormalizeUrl(baseUrl, true);
    }
    public bool IsChildUrl(string childUrl)
    {
        string url = UriX.NormalizeUrl(childUrl);
        return url.StartsWith(baseUrl);
    }
    public bool IsChildUrl_Normalized(string childUrl)
    {
        return childUrl.StartsWith(baseUrl);
    }
}