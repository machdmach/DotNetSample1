namespace Libx
{
    public class AltColor1
    {
        public bool IsAutoSetNextColor = false;
        public int RowIndex = 0;
        public String Color1 = "F0F0F0";
        public String Color2 = "EBFCFD";

        public bool IsEnabled { get; set; }

        public AltColor1()
        {
            //Color2 = new BaseColor(240, 250, 255);
            //Color2 = new BaseColor(230, 240, 245);
            //IsEnabled = isEnabled;
            IsEnabled = true;
        }
        //public AltColor(BaseColor color2)
        //{
        //    Color2 = color2;
        //    IsEnabled = true;
        //}

        //public AltColor(bool isAutoSetNextColor)
        //{
        //    IsAutoSetNextColor = isAutoSetNextColor;
        //    IsEnabled = true;
        //}

        //===================================================================================================
        public void NextColor()
        {
            RowIndex++;
        }
        //===================================================================================================
        public string TR(string css)
        {
            //altColor.NextColor_TR("vertical-align:top")

            //buf.Append("<tr style="background-color:#EBFCFD">");
            if (!IsEnabled)
            {
                return "<tr>";
            }
            string color = Color1;
            if (RowIndex % 2 == 0)
            {
                color = Color2;
            }
            if (IsAutoSetNextColor)
            {
                NextColor();
            }
            return string.Format("<tr style=\"background-color:#{0};{1}\">", color, css);
        }
    }
}
