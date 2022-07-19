namespace Libx;

public class HChildBuilderBase
{
    public StringBuilder buf;
    public int parentInsertLoc;
    public HtmlBuilder parent;

    public HChildBuilderBase(HtmlBuilder parent, StringBuilder buf)
    {
        this.parent = parent;
        this.buf = buf;
        parentInsertLoc = parent.currentInsertLoc;
    }

    public HtmlBuilder FlushInsertToParent()
    {
        if (buf != parent.buf)
        {
            if (parentInsertLoc == 0) throw new InvalidOperationException("htmlBuilderInsertLoc is 0");
            parent.buf.Insert(parentInsertLoc, buf);
        }
        return parent;
    }


}
