using System.IO;

namespace Libx
{
    public partial class FileNodeEntity
    {
        public FileNodeEntity node => this;
        //===================================================================================================
        public string GetFileNameFromTitle()
        {
            var buf = new StringBuilder();
            foreach (char c in node.Title)
            {
                if (Char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.')
                {
                    buf.Append(c);
                }
                else
                {
                    buf.Append(' ');
                }
            }
            string fname = buf.ToString();
            if (!fname.EndsWith("." + node.FileExt, StringComparison.InvariantCultureIgnoreCase))
            {
                fname += "." + node.FileExt;
            }
            return fname;
        }
    }

}
