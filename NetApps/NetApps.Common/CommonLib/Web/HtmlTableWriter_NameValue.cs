using System.Xml.Linq;

namespace System
{
    public class HtmlTableWriter_NameValue : HtmlTableWriter
    {

        //===============================================================================
        public HtmlTableWriter AddRow_NameValue(String label, object val, string val2 = null)//, bool isRequired = false)
        {
            string strVal = null;
            if (val != null)
            {
                Type dataType = val.GetType();

                if (dataType == typeof(Nullable<DateTime>))
                {
                    DateTime? dt = (Nullable<DateTime>)val;
                    if (dt.HasValue) // != DateTime.MinValue)
                    {
                        strVal = dt.Value.ToString("MM/dd/yyyy");
                    }
                }
                else if (dataType == typeof(DateTime))
                {
                    DateTime dt = (DateTime)val;
                    if (dt != DateTime.MinValue)
                    {
                        strVal = dt.ToString("MM/dd/yyyy");
                    }
                }
                else if (dataType == typeof(Nullable<bool>))
                {
                    var v = (Nullable<bool>)val;
                    if (v.HasValue) // != DateTime.MinValue)
                    {
                        strVal = (v.Value == true) ? "Yes" : "No";
                    }
                }
                else if (dataType == typeof(bool))
                {
                    strVal = ((bool)val == true) ? "Yes" : "No";
                }
                else
                {
                    strVal = val.ToString();
                }
            }
            strVal += " &nbsp; " + val2;
            AddRow_NameValue(label, strVal); //, isRequired);
            return this;
        }
        //===============================================================================
        public HtmlTableWriter AddRow_NameValue(String label, String value, bool isRequired = false)
        {
            if (!string.IsNullOrEmpty(label))
            {
                label += ":";
            }
            buf.AppendFormat(TR_NameValue_Format, label, value);
            return this;
        }
        //===============================================================================
        public string TR_NameValue_Format =
         @"<tr valign='top'>
              <td style='padding:3px'> 
                  <strong class='fontNormal'>{0}</strong> &nbsp; &nbsp;
              </td>
              <td> 
                    {1}
              </td>
           </tr>
         ";

        //<span class='fontNormal'> {1} </span>
        //.fontNormal {
        //    font-size: 14px;
        //}



        //===============================================================================
        public HtmlTableWriter AddRow_NameValue_HtmlEncodePre(String label, String value)
        {
            if (!string.IsNullOrEmpty(label))
            {
                label += ":";
            }
            if (!string.IsNullOrEmpty(value))
            {
                var doc = XDocument.Parse(value);
                string valueFormatted = doc.ToString();
                //MOutput.WriteHtmlEncodedPre(valueFormatted);

                string valueEncoded = System.Net.WebUtility.HtmlEncode(valueFormatted);
                value = valueEncoded;

                value = "<pre>" + value + "</pre>";
            }
            buf.AppendFormat(TR_NameValue_Format, label, value);

            return this;
        }
    }
}