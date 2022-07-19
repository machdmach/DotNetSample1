using System.Collections;

namespace System
{
    public class HtmlSelectX : JqHtmlElement
    {
        /*
         * <select id="Select1" onchange="window.location='http://www.google.com/a.b?a=1&b='+ this.value;">
           <option value="0">1111111111111</option>
           <option value="1">2</option>
        </select>
        *
         */
        //public string ID;
        public HtmlSelectX() : base("select")
        {
        }
        public string OnChange = null;

        public ArrayList Values = new ArrayList();
        public ArrayList Labels = new ArrayList();
        public void AddOption(object value)
        {
            AddOption(value, value);
        }
        public bool ContainsValue(object val)
        {
            return Values.Contains(val);
        }
        public void AddOption(object value, object label)
        {
            Values.Add(value);
            Labels.Add(label);
        }
        public void InsertOption(int idx, object value, object label)
        {
            Values.Insert(idx, value);
            Labels.Insert(idx, label);
        }
        public string SelectedValue = Guid.NewGuid().ToString();

        public string ToHtml()
        {
            var buf = new StringBuilder();

            //string onChangeStr = "";
            //if (OnChange != null)
            //{
            //    onChangeStr = string.Format("onchange=\"{0}\";", OnChange);
            //}
            //string addlAttribs = onChangeStr;
            //buf.AppendFormat("<select id='{0}' name='{0}' class='form-control' {1}>", ID, addlAttribs);

            //buf.AppendFormat("<select id='{0}' name='{0}' class='form-control' {1}>", ID, addlAttribs);
            BuildStartTag(buf);

            for (int i = 0; i < Values.Count; i++)
            {
                string val = Values[i].ToString();
                string label = Labels[i].ToString();
                string selected = "";
                if (val == SelectedValue)
                {
                    selected = " selected";
                }
                buf.Append(string.Format("<option value=\"{0}\"{1}>{2}</option>", val, selected, label));
            }
            buf.Append("</select>");
            return buf.ToString();
        }
        //public static string Build(string ID, string addlAttribs, ArrayList values, ArrayList labels)
        //{
        //   throw new Exception("abc");
        //}
    }
}
