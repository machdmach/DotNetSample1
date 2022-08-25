namespace Libx;
public class HStyleBuilder : HChildBuilderBase
{
    public HStyleBuilder(HtmlBuilder parent, StringBuilder buf) : base(parent, buf) { }

    public HtmlBuilder Done()
    {
        buf.Append('\'');
        return FlushInsertToParent();
    }
    //===================================================================================================
    public HStyleBuilder Add(string name, string val)
    {
        buf.Append(name);
        buf.Append(':');
        buf.Append(val);
        buf.Append(';');
        return this;
    }
    public HStyleBuilder Add(string name, ValueType val)
    {
        buf.Append(name);
        buf.Append(':');
        buf.Append(val);
        buf.Append(';');
        return this;
    }

    //===================================================================================================
    public HStyleBuilder BackgroundColor(string bgColor) => Add("background-color", bgColor);
    public HStyleBuilder Color(string val) => Add("color", val);
    public HStyleBuilder Display(string val) => Add("display", val); //fixed, flex, block, inline-block
    public HStyleBuilder FontSize(string val) => Add("font-size", val);
    public HStyleBuilder Width(int val) => Add("width", val);
    public HStyleBuilder ZIndex(int val) => Add("z-index", val);

}