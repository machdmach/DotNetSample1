namespace Libx;
public class WebPageEntity
{
    public int PageNum { get; set; }
    public int ParentPageNum { get; set; }
    public WebPageEntity Parent { get; set; }
    public string ParentDirUrl { get; set; }

    public string URL { get; set; }
    public bool IsInvalidURL { get; set; }

    public string NormalizedURL { get; set; }

    public string AHrefLabel { get; set; }
    //public List<string> AHrefLabelList { get; init; } = new List<string>();
    public int ReferenceCount { get; set; }

    public float Score { get; set; }
    public double Latency { get; set; }

    //public string AHrefInnerHtml { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Level { get; set; }

    public string KeyWords { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public String ParsedErrorMessage { get; set; }
    public bool IsHtmlPage => !IsInvalidURL && UriX.IsHtmlPage(URL);

    public int ContentLength { get; set; } //{ return ContentPlainText == null ? -1 : ContentPlainText.Length; } }

    public string ContentPlainText { get; set; }
    public string HtmlContentOrginal { get; set; }
    //public string HtmlContentIndexed { get { return ContentPlainText; } }

    public string ContentType { get; set; }
    public bool IsContentTypeHtml()
    {
        bool ret = ContentType?.IndexOf("text/html", StringComparison.InvariantCultureIgnoreCase) >= 0;
        return ret;
    }
    public bool IsContentTypeText()
    {
        bool ret = ContentType?.IndexOf("text/", StringComparison.InvariantCultureIgnoreCase) >= 0;
        return ret;
    }
    public void SaveMemoryFootprint()
    {
        HtmlContentOrginal = null;
        ContentPlainText = null;
    }

    //===================================================================================================
    public WebPageEntity() { }
    public WebPageEntity(String url, WebPageEntity parent = null)
    {
        Parent = parent;
        if (parent != null)
        {
            Level = parent.Level + 1;
            ParentPageNum = parent.PageNum;

            Uri baseUri = UriX.CreateUri(parent.URL);
            try
            {
                Uri newUri = UriX.CreateUri(baseUri, url);
                URL = newUri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                URL = url;
                IsInvalidURL = true;
            }
        }
        else
        {
            Uri uri = UriX.CreateUri(url);
            URL = uri.AbsoluteUri;
        }
        NormalizedURL = IsInvalidURL ? url : UriX.NormalizeUrl(URL);
    }
    public List<WebPageEntity> GetAncestors(bool includeSelf = true)
    {
        List<WebPageEntity> list = new List<WebPageEntity>();
        if (includeSelf)
        {
            list.Add(this);
        }
        GetAncestors(this.Parent, list);
        return list;
    }
    public void GetAncestors(WebPageEntity wp, List<WebPageEntity> list)
    {
        list.Add(wp);
        if (wp.Parent != null)
        {
            GetAncestors(wp.Parent, list);
        }
    }

    public string GetWebPageInfoLink(string label = "@@")
    {
        var wp = this;
        string url = "/WebsiteTools/WebPageInfoUI?URL=" + Uri.EscapeDataString(wp.URL);

        string getPageInfoLink = string.Format("<a href='{0}' target='win2'> {1} </a>", url, label);
        return string.Format(" {0}", getPageInfoLink);
    }

    //===================================================================================================
    public String ToHtml(string caption = "WebPageEntity")
    {
        WebPageEntity o1 = this;
        var nvs = new NameObjectCollection();
        nvs.Add("URL", o1.URL);

        nvs.Add("Title", o1.Title);
        nvs.Add("Description", o1.Description);
        nvs.Add("KeyWords", o1.KeyWords);
        nvs.Add("ContentLength", o1.ContentLength);

        nvs.Add("Latency", o1.Latency / 1000.0); //Double: 0
        nvs.Add("AHrefLabel", o1.AHrefLabel);
        //nvs.Add("AHrefInnerHtml", o1.AHrefInnerHtml); 

        nvs.Add("StatusCode", o1.StatusCode); //HttpStatusCode: 0
        nvs.Add("ParsedErrorMessage", WebUtility.HtmlEncode(o1.ParsedErrorMessage));

        nvs.Add("Parent", Parent == null ? "" : Parent.URL); //WebPageEntity: 
        //string parentLink = (e.Parent == null) ? "" : string.Format("<a href='{0}'>{0}</a>", Parent.URL);
        //nvs.Add("Parent", parentLink); //WebPageEntity: 

        nvs.Add("ParentDirUrl", ParentDirUrl); //WebPageEntity: 
        nvs.Add("Level", o1.Level); //int: 0
        nvs.Add("ReferenceCount", o1.ReferenceCount); //int: 0

        String rval = nvs.ToHtml(new() { Caption = caption });
        return rval;
    }

    //===================================================================================================
    public static String zToAHrefSimple(String url)
    {
        string URLShort = url;
        //if (!string.IsNullOrEmpty(options.DomainToRemove))
        //{
        //    URLShort = URLShort.Replace(options.DomainToRemove, "");
        //}
        //if (URLShort == "")
        //{
        //    URLShort = url;
        //}
        if (URLShort.Length > 500)
        {
            URLShort = URLShort.Substring(0, 100) + "...";
        }
        string urlLink = string.Format("<a href='{0}' target='win2'>{1}</a>", url, URLShort);
        return urlLink;
    }
    //===================================================================================================
    //public string GetURLShort()
    //{
    //    String URLShort = URL;
    //    if (URLShort.Length > 100)
    //    {
    //        URLShort = URLShort.Substring(0, 100) + "...";
    //    }
    //    return URLShort;
    //}
    //public string GetUrlLink()
    //{
    //    string urlLink = string.Format("<a href='{0}' target='win2'>{1}</a>", URL, GetURLShort());
    //    return urlLink; ;
    //}
    //public string GetLabelLink()
    //{
    //    string ahrefLabel = AHrefLabel;
    //    if (IsHtmlPage)
    //    {
    //        ///Crawler/WebPageInfo?url=http://url123
    //        string crawlerUrl = string.Format("/Crawler/WebPageInfo?url={0}&method=Get%20AHrefs", Uri.EscapeDataString(URL));
    //        ahrefLabel = string.Format("<a href='{0}' ztarget='win2'>{1}</a>", crawlerUrl, ahrefLabel);
    //    }
    //    return ahrefLabel;
    //}
}
//*******************************************************************************************************
public class WebPageEntityFormatter
{
    //===================================================================================================
    public static String FormatHtmlSimple(IEnumerable<WebPageEntity> list, Options options, string title)
    {
        var hm = new HtmlBuilder();
        var buf = hm.buf;
        buf.AppendFormat("<h3>{0}</h3>", title);
        
        hm.TagStart_cs("table", "dataTabCrawlerRes", "border-collapse: collapse; border-spacing: 0");
        hm.InsertAttr().Attr("border", 1).Done();

        hm.TagStart("tr");
        hm.Th("#");
        hm.Th("Parent URL");
        hm.Th("Label");
        hm.Th(options.ShowBrokenUrls? "Broken URLs": "URL");
        hm.TagEnd("tr");

        int i = 1;
        hm.ThStyle = "padding: 5px;";
        hm.TdStyle = "padding: 5px;";
        foreach (var e in list)
        {
            if (i > options.MaxCount)
            {
                buf.Append("too many rows exceeding: " + options.MaxCount);
                break;
            }
            hm.TagStart("tr").InsertAttr("valign", "top").Done();
            hm.Td(i.ToString());
            hm.Td(ToAHrefSimple(e.Parent.URL));
            hm.Td(e.AHrefLabel);
            hm.Td(ToAHrefSimple(e.URL));
            hm.TagEnd("tr");
            i++;
        }
        hm.TagEnd("table");
        return hm.ToString();
    }

    //===================================================================================================
    public class Options
    {
        public bool ShowBrokenUrls { get; set; }
        public bool ShowAHrefInnerHtml { get; set; }
        public string DomainToRemove { get; set; }
        public int MaxCount { get; set; } = 1000;
        public bool IsShowingScore { get; set; }
        public bool IsShowingExtensiveInfo { get; set; }
    }
    Options options;
    //===================================================================================================
    public String ToHtml(IEnumerable<WebPageEntity> list, Options options, string title)
    {
        this.options = options;

        var buf = new StringBuilder();
        buf.AppendFormat("<h3>{0}</h3>", title);
        buf.AppendLine("<table class='dataTabCrawlerRes' border='1' cellpadding='3px' style='padding: 3px; border-collapse: collapse; border-spacing: 0'>");
        buf.Append("<tr valign='top'>");
        buf.AppendLine("<td>#</td>");
        buf.AppendLine("<td>Link Label</td>");
        buf.AppendLine("<td>URL</td>");
        buf.AppendLine("<td>Status Code</td>");
        buf.AppendLine("<td>Latency</td>");
        buf.AppendLine("<td>Ref</td>");

        if (options.IsShowingScore) buf.AppendLine("<td>Score</td>");
        if (options.IsShowingExtensiveInfo)
        {
            buf.AppendLine("<td>Page Title</td>");
            buf.AppendLine("<td>Page Description</td>");
            buf.AppendLine("<td>Keywords</td>");
            buf.AppendLine("<td>length</td>");
        }

        buf.AppendLine("<td>Parent URL</td>");
        buf.AppendLine("<td>Level</td>");

        buf.AppendLine("</tr>");

        int i = 1;
        foreach (var wp in list)
        {
            if (i > options.MaxCount)
            {
                buf.Append("too many rows exceeding: " + options.MaxCount);
                break;
            }
            ToTR(wp, buf, i);
            i++;
        }
        buf.AppendLine("</table>");

        return buf.ToString();
    }

    //===================================================================================================
    public string ToTR(WebPageEntity e, StringBuilder buf, int i = 0)
    {
        //401 Unauthorized
        buf.Append("<tr valign='top'>");
        buf.AppendFormat("<td>{0}</td>", i);

        string ahrefLabel = e.AHrefLabel;
        //if (!string.IsNullOrWhiteSpace(e.AHrefLabel))
        //{
        //    ahrefLabel = e.AHrefLabel;
        //}
        /////Crawler/WebPageInfo?url=http://doadocs.mccarran.com/dsweb/View/Collection-36931
        //string crawlerUrl = string.Format("/Crawler/WebPageInfo?url={0}&method=Get%20AHrefs", Uri.EscapeDataString(e.URL));
        //ahrefLabel = string.Format("<a href='{0}' ztarget='win2'>{1}</a>", crawlerUrl, ahrefLabel);

        buf.AppendFormat("<td>{0}</td>", ahrefLabel);
        buf.AppendFormat("<td>{0}</td>", ToAHref(e.URL));

        string statusCode = (int)e.StatusCode + "";
        //string statusCode = string.Format("{0} {1} {2}", (int)e.StatusCode, e.StatusCode.ToString(), WebUtility.HtmlEncode(e.ParsedErrorMessage));
        //if (statusCode.Length > 100) { statusCode = statusCode.Substring(0, 100) + "..."; }
        //statusCode = e.GetWebPageInfoLink(statusCode + "@@");        

        buf.AppendFormat("<td>{0}</td>", statusCode);
        buf.AppendFormat("<td>{0:#.#}</td>", e.Latency / 1000.0);
        buf.AppendFormat("<td>{0}</td>", e.ReferenceCount == 0 ? "" : e.ReferenceCount.ToString());

        if (options.IsShowingScore) buf.AppendFormat("<td>{0:#.##}</td>", e.Score);

        if (options.IsShowingExtensiveInfo)
        {
            buf.AppendFormat("<td>{0}</td>", e.Title);
            buf.AppendFormat("<td>{0}</td>", e.Description);
            buf.AppendFormat("<td>{0}</td>", e.KeyWords);
            buf.AppendFormat("<td>{0}</td>", e.ContentLength);
        }

        string parentLink = (e.Parent == null) ? "" : ToAHref(e.Parent.URL);
        buf.AppendFormat("<td>{0}</td>", parentLink);

        buf.AppendFormat("<td>{0}</td>", e.Level);

        buf.AppendLine("</tr>");
        return buf.ToString();
    }
    //===================================================================================================
    public String ToAHref(String url)
    {
        string URLShort = url;
        if (!string.IsNullOrEmpty(options.DomainToRemove))
        {
            URLShort = URLShort.Replace(options.DomainToRemove, "");
        }
        if (URLShort == "")
        {
            URLShort = url;
        }

        if (URLShort.Length > 500)
        {
            URLShort = URLShort.Substring(0, 100) + "...";
        }
        string urlLink = string.Format("<a href='{0}' target='win2'>{1}</a>", url, URLShort);
        return urlLink;
    }
    //===================================================================================================
    public static String ToAHrefSimple(String url)
    {
        string URLShort = url;
        //if (!string.IsNullOrEmpty(options.DomainToRemove))
        //{
        //    URLShort = URLShort.Replace(options.DomainToRemove, "");
        //}
        //if (URLShort == "")
        //{
        //    URLShort = url;
        //}
        if (URLShort.Length > 500)
        {
            URLShort = URLShort.Substring(0, 100) + "...";
        }
        string urlLink = string.Format("<a href='{0}' target='win2'>{1}</a>", url, URLShort);
        return urlLink;
    }
}
