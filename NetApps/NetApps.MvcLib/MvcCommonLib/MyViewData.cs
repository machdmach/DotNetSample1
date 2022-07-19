using Microsoft.AspNetCore.Mvc.Razor;

namespace Libx.Mvc;
public class MyViewData
{
    public StringBuilder FooterDebugInfoBuf = new StringBuilder();
    public string publicFacingUrlRoot { get; set; }
    public DateTime ServerDateTime { get; set; }

    public string HeadTitle { get; set; }
    public string AppTitle { get; set; }
    public string MasterPage { get; set; }
    public string DbEnv { get; set; }

    public MvcHttpRequest reqData { get; set; }
    public ResponseData resData { get; set; }

    public Func<string> GetHeaderhtml;
    public Func<string> GetHtmlHeadExtraHtml;

    //===================================================================================================
    public MyViewData(MvcHttpRequest reqData)
    {
        this.reqData = reqData;

        GetHeaderhtml = () =>
        {
            var hm = new HtmlBuilder();
            hm.TagOpen("div").Style("text-align:center").TagClose()
            .Span(AppTitle, style: "font-size:18pt")
            .Span(DbEnv, style: "font-size:10pt")
            .TagEnd("div").Hr();

            return hm.ToString();
        };
        GetHtmlHeadExtraHtml = () =>
        {
            //Add client-side javascripts and CSS
            var hm = new HtmlBuilder();
            hm.AppendLine($@"
                <script>
                    window.global_bag = window.global_bag || new Object();
                   window.global_bag.appName = '{reqData.AppName}';
                </script>");

            string clientSideBaseUrl;
            if (reqData.IsLocalhost)
            {
                if (ConfigX.IsDevMachine)
                {
                    clientSideBaseUrl = "http://localhost:4141/NetApps/cs";
                }
                else
                {
                    clientSideBaseUrl = "https://www.harryreidairport.com/FSWeb/x/NetApps";
                }
            }
            else
            {
                clientSideBaseUrl = ConfigX.WebAppVirtualPath + "/cs";
            }
            var refreshParam = ConfigX.AppRunIdLong;
            if (ConfigX.IsIISExpress)
            {
                refreshParam = Environment.TickCount + "";
            }
            if (NotRunThis) //#todo
            {
                hm.Link_relStyleSheet("/WebUIx/NetApps/styleNetApps.css?ts=" + refreshParam, clientSideBaseUrl);
                hm.Script("/WebUIx/NetApps/mainNetApps.js?ts=" + refreshParam, clientSideBaseUrl);
            }
            return hm.ToString();
        };
    }

    //===================================================================================================
    public static MyViewData Prep(RazorPageBase page, bool isPageLayout = false)
    {
        var ret = page.ViewContext.ViewData["myViewData"] as MyViewData ?? new MyViewData(new());
        ret.prerender(page, isPageLayout);
        return ret;
    }
    private bool prerenderDone = false;
    private void prerender(RazorPageBase page, bool isPageLayout = false)
    {
        if (prerenderDone) return; //-------------

        if (reqData.IsDebug)
        {
            try { FooterDebugInfoBuf.AppendLine(DevLinks.Instance.BuildDevLinks(reqData)); }
            catch { } //ignore, for dev only
        }
        if (HeadTitle == null) HeadTitle = $"{reqData.AppName} - {DbEnv}";
        prerenderDone = true;
    }
}

