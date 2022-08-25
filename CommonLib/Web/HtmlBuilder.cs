namespace Libx;

public partial class HtmlBuilder
{
    public StringBuilder buf;
    public HAttributeBuilder Attr;

    public HtmlBuilder()
    {
        buf = new StringBuilder();
        Attr = new HAttributeBuilder(this, buf);
    }
    public HtmlBuilder(StringBuilder buf)
    {
        this.buf = buf;
    }

    //===================================================================================================
    public HtmlBuilder Append(String s)
    {
        buf.Append(s);
        return this;
    }
    public HtmlBuilder AppendLine(String s)
    {
        buf.AppendLine(s);
        return this;
    }
    public HtmlBuilder AppendFormat(string format, params object?[] args)
    {
        buf.AppendFormat(format, args);
        return this;
    }

    public override string ToString()
    {
        string ret = buf.ToString();
        MOutput.Write("HtmlBuilder.ToString: buf.len=" + buf.Length);
        //MOutput.WriteHtmlEncodedPre(ret);
        return ret;
    }
    //===================================================================================================
    public string currentTag;
    public int currentInsertLoc;
    public HAttributeBuilder TagOpen_cs(string tagName, string className = null, string style = null)
    {
        buf.Append('<');
        buf.Append(tagName);
        currentTag = tagName;
        if (className != null)
        {
            buf.Append(" class='");
            buf.Append(className);
            buf.Append('\'');
        }
        if (style != null)
        {
            buf.Append(" style='");
            buf.Append(style);
            buf.Append('\'');
        }
        currentInsertLoc = buf.Length;
        return Attr;
    }

    public HAttributeBuilder TagOpen(string tagName)
    {
        buf.Append('<');
        buf.Append(tagName);
        currentTag = tagName;
        currentInsertLoc = buf.Length;
        return Attr;
    }

    public HtmlBuilder TagStart_cs(string tagName, string className = null, string style = null)
    {
        TagOpen_cs(tagName, className, style);
        buf.Append('>');
        return this;
    }
    public HtmlBuilder TagStart(string tagName)
    {
        TagOpen(tagName);
        buf.Append('>');
        return this;
    }

    //===================================================================================================
    public HtmlBuilder TagClose()
    {
        buf.Append('>');
        return this;
    }
    public HtmlBuilder TagCloseAndEnd(string tagName = null)
    {
        buf.Append('>');
        TagEnd(tagName);
        return this;
    }
    public HtmlBuilder TagCloseAndEndSelf()
    {
        buf.Append("/>");
        return this;
    }

    //===================================================================================================
    public HtmlBuilder TagEnd(string tagName = null)
    {
        buf.Append("</");
        if (tagName == null)
        {
            buf.Append(currentTag);
        }
        else
        {
            buf.Append(tagName);
        }
        buf.AppendLine(">");
        return this;
    }

    //===================================================================================================
    /*
        public HtmlBuilder z()
        {
            buf.Append("");
            return this;
        }
    */
    public HStyleBuilder OpenStyle()
    {
        ///<see cref="Style"/>
        buf.Append(" style='");
        var ret = new HStyleBuilder(this, buf);
        return ret;
    }
    public HStyleBuilder InsertStyle()
    {
        HStyleBuilder ret;
        if (currentInsertLoc == buf.Length)
        {
            buf.Append(" style='");
            ret = new HStyleBuilder(this, buf);
        }
        else
        {
            var styleBuf = new StringBuilder();
            styleBuf.Append(" style='");
            ret = new HStyleBuilder(this, styleBuf);
        }
        return ret;
    }

    //===================================================================================================
    public HAttributeBuilder InsertAttr(string name = null, string val = null)
    {
        HAttributeBuilder ret;
        if (currentInsertLoc == buf.Length)
        {
            ret = new HAttributeBuilder(this, buf);
        }
        else
        {
            var newBuf = new StringBuilder();
            ret = new HAttributeBuilder(this, newBuf);
        }
        if (name != null)
        {
            ret.Attr(name, val);
        }
        return ret;
    }
}
