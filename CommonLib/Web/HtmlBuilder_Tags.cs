namespace Libx;

public partial class HtmlBuilder
{
    public HtmlBuilder Hr() => AppendLine("<hr>");

    public HtmlBuilder Br(int repeat = 1)
    {
        for (int i = 0; i < repeat; i++) buf.AppendLine("<br>");
        return this;
    }
    //===================================================================================================
    public HtmlBuilder Anchor(string url, string urlLabel = null)
    {
        if (urlLabel == null)
        {
            urlLabel = url;
        }

        int maxLabelLen = 50;
        if (urlLabel.Length > maxLabelLen)
        {
            //urlLabel = "..." + urlLabel.Substring(urlLabel.Length - 50);
        }
        if (string.IsNullOrWhiteSpace(urlLabel))
        {
            //urlLabel = "&nbsp;";
        }
        TagOpen("a").Href(url).TagClose().Append(urlLabel).TagEnd();
        return this;
    }
    //public void H3(string s)
    //{
    //    buf.Append("<h3>");
    //    buf.Append(s);
    //    buf.Append("</h3>");
    //}

    public HtmlBuilder Button(string text = null, string name = "action", string value = null, string accessKey = null, string className = "btnx btnx-primary", string style = null) =>
        TagOpen("button").Attr("name", name).Attr("value", value ?? text).AccessKey(accessKey).Class(className).Style(style).TagClose().Append(text).TagEnd();

    public HtmlBuilder Comment(string s) => Append("<!--").Append(s).AppendLine("-->");
    public HtmlBuilder Div(string text = null, string className = null, string style = null) => TagOpen("div").Class(className).Style(style).TagClose().Append(text).TagEnd();

    public HtmlBuilder Form(string method = "post", string encType = "application/x-www-form-urlencoded", string className = "formx", string style = null) =>
        TagOpen("form").Method(method).Enctype(encType).Class(className).Style(style).TagClose();

    public HtmlBuilder H1(string text, string className = null, string style = null) => TagOpen("h1").Class(className).Style(style).TagClose().Append(text).TagEnd();
    public HtmlBuilder H2(string text, string className = null, string style = null) => TagOpen("h2").Class(className).Style(style).TagClose().Append(text).TagEnd();
    public HtmlBuilder H3(string text, string className = null, string style = null) => TagOpen("h3").Class(className).Style(style).TagClose().Append(text).TagEnd();
    public HtmlBuilder Hn(int sectionLevel, string text, string className = null, string style = null) =>
        TagOpen("h" + sectionLevel).Class(className).Style(style).TagClose().Append(text).TagEnd();

    public HtmlBuilder Input(string name, string className = "ff-control", string style = null) => TagOpen("input").Name(name).Class(className).Style(style).TagClose();

    public HtmlBuilder Label(string text, string forElt = null, string className = null, string style = null) =>
        TagOpen("label").Attr("for", forElt).Class(className).Style(style).TagClose().Append(text).TagEnd();

    //<link href='https://hr.com/fsweb/assets/pubStatic/LocalDev1.css?ts={ConfigX.AppRunIdLong}' rel='stylesheet' />
    public HtmlBuilder Link_relStyleSheet(string href, string baseUrl = null) => TagOpen("link").Href(href, baseUrl).Rel("stylesheet").TagCloseAndEndSelf();

    //<script src='file1js?v=12' type='module'></script>
    public HtmlBuilder Script(string href, string baseUrl = null, string type = "module") => TagOpen("script").Src(href, baseUrl).Type(type).TagCloseAndEnd();

    public HtmlBuilder Span(string text = null, string className = null, string style = null)
        => TagOpen("span").Class(className).Style(style).TagClose().Append(text).TagEnd();

    //public HtmlBuilder Tr(string className = null, string style = null) => TagOpen("td").Class(className).Style(style).TagClose().TagEnd();
    public string TdStyle;
    public string ThStyle;
    public HtmlBuilder Td(string text = null, string className = null, string style = null) => TagOpen("td").Class(className).Style(style ?? TdStyle).TagClose().Append(text).TagEnd();
    public HtmlBuilder Th(string text = null, string className = null, string style = null) => TagOpen("th").Class(className).Style(style ?? ThStyle).TagClose().Append(text).TagEnd();
}
