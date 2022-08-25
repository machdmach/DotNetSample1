namespace Libx;

public class HAttributeBuilder : HChildBuilderBase
{
    public HAttributeBuilder(HtmlBuilder parent, StringBuilder buf) : base(parent, buf) { }

    public HtmlBuilder Done()
    {
        return FlushInsertToParent();
    }

    //===================================================================================================
    public HtmlBuilder TagClose()
    {
        buf.Append('>');
        buf.AppendLine();
        return FlushInsertToParent();
    }
    public HtmlBuilder TagCloseAndEnd()
    {
        buf.Append(">");
        FlushInsertToParent();
        parent.TagEnd();
        return parent;
    }
    public HtmlBuilder TagCloseAndEndSelf()
    {
        buf.AppendLine("/>");
        return FlushInsertToParent();
    }

    //===================================================================================================
    public HAttributeBuilder Attr(string name, string val)
    {
        if (val != null)
        {
            buf.Append(' ');
            buf.Append(name);
            buf.Append("='");
            buf.Append(val);
            buf.Append('\'');
        }
        return this;
    }
    //Attr("href", baseUrl, url);
    public HAttributeBuilder Attr(string name, string val, string val2)
    {
        if (val == null)
        {
            buf.Append(' ');
            buf.Append(name);
            buf.Append("='");
            buf.Append(val2);
            buf.Append('\'');
        }
        else
        {
            buf.Append(' ');
            buf.Append(name);
            buf.Append("='");
            buf.Append(val);
            if (val2 != null)
            {
                buf.Append(val2);
            }
            buf.Append('\'');
        }
        return this;
    }
    /// <summary>
    /// Set attribute for the html tag
    /// </summary>
    /// <param name="name"></param>
    /// <param name="val">ValueType: e.g. int, decimal</param>
    /// <returns></returns>
    public HAttributeBuilder Attr(string name, ValueType val)
    {
        //this.Attr("dd", 3);
        buf.Append(' ');
        buf.Append(name);
        buf.Append('=');
        buf.Append(val);
        //buf.Append(' ');
        return this;
    }
    public HAttributeBuilder Attr(string name, bool val)
    {
        if (val)
        {
            buf.Append(' ');
            buf.Append(name);
        }
        return this;
    }

    public HAttributeBuilder AccessKey(string key) => Attr("accesskey", key);
    public HAttributeBuilder Action(string action) => Attr("action", action);
    public HAttributeBuilder AutoFocus(bool flag = true) => Attr("autofocus", flag);
    public HAttributeBuilder Class(string className) => Attr("class", className);
    public HAttributeBuilder Cols(int numOfChars) => Attr("cols", numOfChars); //default=20
    public HAttributeBuilder Disabled(bool flag = true) => Attr("disabled", flag); //inherit from fieldset

    /*
    Content-Type: application/x-www-form-urlencoded
    Content-Type: multipart/form-data; boundary=----formdata-polyfill-0.3122345992898219
    */
    public HAttributeBuilder Enctype(string enctype) => Attr("enctype", enctype);

    //const url = new URL('../cats', 'http://www.example.com/dogs');
    //console.log(url.hostname); // "www.example.com"
    //console.log(url.pathname); // "/cats"
    //C# new Uri(baseUrl, relUrl)
    public HAttributeBuilder Href(string url, string baseUrl = null) => Attr("href", baseUrl, url);
    public HAttributeBuilder Id(string id) => Attr("id", id);
    public HAttributeBuilder MaxLength(int numOfChars) => Attr("maxlength", numOfChars); //default=unlimited
    /// <summary>
    /// </summary>
    /// <param name="httpMethod"> post, get, dialog</param>
    public HAttributeBuilder Method(string httpMethod) => Attr("method", httpMethod);
    public HAttributeBuilder MinLength(int numOfChars) => Attr("minlength", numOfChars);
    public HAttributeBuilder Name(string name) => Attr("name", name);
    public HAttributeBuilder NameAndId(string name, string id) => Attr("name", name).Attr("id", id);

    public HAttributeBuilder Onclick(string name) => Attr("onclick", name);
    
    public HAttributeBuilder PlaceHolder(string text) => Attr("placeholder", text);
    public HAttributeBuilder ReadOnly(bool flag = true) => Attr("readonly", flag); //submitted with the form
    public HAttributeBuilder Required(bool flag = true) => Attr("required", flag);
    public HAttributeBuilder Rel(string relation) => Attr("rel", relation);
    public HAttributeBuilder Rows(int numOfLines) => Attr("rows", numOfLines); //default=2

    public HAttributeBuilder Src(string url, string baseUrl = null) => Attr("src", baseUrl, url);
    public HAttributeBuilder Style(string style) => Attr("style", style);

    /// <summary>
    /// </summary>
    /// <param name="target">_self, _parent, _top, _blank</param>
    public HAttributeBuilder Target(string target) => Attr("target", target);

    /// <summary>
    /// [button type='submit(default)/reset/button'
    /// </summary>
    /// <param name="type">submit(default)/reset/button</param>
    public HAttributeBuilder Type(string type) => Attr("type", type);
    public HAttributeBuilder Value(string val) => Attr("value", val);

    /// <summary>
    /// default=soft(not auto insert CRLF), 
    /// (hard=browser auto insert (CFLF), cols attr is required for this to take effect)
    /// </summary>
    /// <param name="softOrHard"></param>
    public HAttributeBuilder Wrap(string softOrHard) => Attr("wrap", softOrHard);
}
