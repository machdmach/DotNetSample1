using System.IO;

namespace System
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
        //===================================================================================================
        public static void GetCleanedFileNameFromPath_Tests()
        {
            string t;
            MOutput.WriteHtmlEncodedPre(string.Format("{0} = {1}", t = "abc  #$#--.jpg", GetCleanedFileNameFromPath(t)));
        }

        public static string GetCleanedFileNameFromPath(string pfname)
        {
            if (string.IsNullOrEmpty(pfname))
            {
                return pfname;
            }
            var buf = new StringBuilder();
            string fname = Path.GetFileName(pfname);
            char lastChar = 'z';
            foreach (char c in fname)
            {
                if (Char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.') //or +, &, , 
                {
                    buf.Append(c);
                    lastChar = c;
                }
                else
                {
                    if (lastChar != ' ')
                    {
                        buf.Append(' ');  //compress multiple spaces into 1
                        lastChar = ' ';
                    }
                }
            }
            string rval = buf.ToString();
            rval = rval.Trim();
            return rval;
        }
    }

}
