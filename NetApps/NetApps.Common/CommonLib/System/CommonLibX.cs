using System.IO;
using System.Xml.Serialization;

namespace System
{
    //===================================================================================================
    public class CommonLibX
    {
        //===================================================================================================
        public static String XmlSerialize(Object obj)
        {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            var buf = new StringBuilder();

            StringWriter sw = new StringWriter(buf);
            xs.Serialize(sw, obj);
            sw.Flush();
            string s = buf.ToString();
            s = s.Replace("utf-16", "utf-8");
            return s;
        }

        //===================================================================================================
        public static String GetMimeType(String fname)
        {
            //--------------------------------------------------------------------------------------------
            string mimeType;
            if (fname.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                mimeType = "application/pdf";
            }
            else
            {
                mimeType = "application/force-download";
            }
            var fi = new FileInfo(fname);
            mimeType = "application/" + fi.Extension.Replace(".", "");
            //return this.View(MvcLib.PassThroughView, (object) mimeType);
            //return Content(mimeType);
            return mimeType;
        }
        //===================================================================================================
        public static string ToString_MM_dd_yyyy_hh_mm_ss(DateTime? dt)
        {
            string rval = "";
            if (dt.HasValue)
            {
                rval = dt.Value.ToString("MM/dd/yyyy HH:mm:ss");
            }
            return rval;
        }
        public static string ToString_dd_MMM_yyyy_at_hh_mm_tt(DateTime dt)
        {
            if (dt == default(DateTime)) { return ""; }

            string s = dt.ToString("dd-MMM-yyyy 'at' hh:mm tt");
            return s;
        }

    }
}
