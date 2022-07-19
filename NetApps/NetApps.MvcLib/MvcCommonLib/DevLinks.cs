namespace Libx.Mvc;

public class DevLinkEntity
{
    public String Url;
    public String Label;
}

public abstract class DevLinks
{
    private readonly List<DevLinkEntity> devLinks = new List<DevLinkEntity>();
    public static DevLinks Instance => (DevLinks) Activator.CreateInstance(ConcreteClass);

    static Type ConcreteClass;
    public static void InjectDependency(Type concreteClass)
    {
        ConcreteClass = concreteClass;
    }
    public abstract string BuildDevLinks(MvcHttpRequest req);

    //===================================================================================================
    public string AddLink(string url, string urlLabel = null)
    {
        if (urlLabel == null)
        {
            urlLabel = url;
        }
        int maxLabelLen = 50;
        if (urlLabel.Length > maxLabelLen)
        {
            urlLabel = "..." + urlLabel.Substring(urlLabel.Length - 50);
        }
        if (string.IsNullOrWhiteSpace(urlLabel))
        {
            //urlLabel = "&nbsp;";
        }
        devLinks.Add(new DevLinkEntity { Url = url, Label = urlLabel });
        return url;
    }
    //===================================================================================================
    public string ToHtml()
    {
        //#devlinks
        var buf = new StringBuilder();
        buf.AppendLine("<ul class='ul-plain'>");
        devLinks.ForEach(e =>
        {
            if (string.IsNullOrWhiteSpace(e.Label)) 
            {
                buf.AppendLine("<li> &nbsp; </li>");
            }
            else
            {
                string url = MvcLib.ResolveToAbsolutePath(e.Url);
                buf.AppendFormat("<li> <a href='{0}'> {1} </a></li>\n", url, e.Label);
            }
        });
        buf.AppendLine("</ul>");
        return buf.ToString();
    }
}
