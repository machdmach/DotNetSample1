namespace System
{
    public class DataFieldDefHelper
    {
        //===================================================================================================
        public static string FieldNameToLabel(String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            var buf = new StringBuilder();
            char prevChar = Char.ToUpper(s[0]);
            buf.Append(prevChar);

            for (int i = 1; i < s.Length; i++)
            {
                char c = s[i];
                if (Char.IsUpper(c))
                {
                    if (char.IsLower(prevChar))
                    {
                        buf.Append(' ');
                    }
                    buf.Append(c);
                }
                else if (c == '_')
                {
                    buf.Append(' ');
                }
                else
                {
                    buf.Append(c);
                }
                prevChar = c;
            }
            string label = buf.ToString();
            //label = StringX.ToTitleCase(label);
            return label;
        }
        //===================================================================================================
        public static string ColumnNameToLabel(String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            var buf = new StringBuilder();
            char prevChar = Char.ToUpper(s[0]);
            buf.Append(prevChar);

            for (int i = 1; i < s.Length; i++)
            {
                char c = s[i];
                if (Char.IsUpper(c))
                {
                    if (char.IsLower(prevChar))
                    {
                        buf.Append(' ');
                    }
                    buf.Append(c);
                }
                else if (c == '_')
                {
                    buf.Append(' ');
                }
                else
                {
                    buf.Append(c);
                }
                prevChar = c;
            }
            string label = buf.ToString();
            label = StringX.ToTitleCase(label);
            return label;
        }
        //===================================================================================================
        public static string ColumnNameToFieldName(String s)
        {
            string label = ColumnNameToLabel(s);
            string fieldName = label.Replace(" ", "");
            return fieldName;
        }
        //===================================================================================================
        public static string getSampleValueForDef(DataFieldDef prop, string language = "C#")
        {
            Type propType = prop.PropertyType;
            var propName = prop.Name;
            string s;


            if (propType == typeof(String))
            {
                string sval = GetSampleValueForFieldName(propName);
                s = string.Format("\"{0}\"", sval);
            }
            else if (propType == typeof(Guid))
            {
                s = "Guid.NewGuid()";
            }
            else if (propType == typeof(bool))
            {
                s = "true";
            }
            else if (propType == typeof(Nullable<bool>))
            {
                s = "true";
            }
            else if (propType == typeof(DateTime))
            {
                s = "DateTime.Now";
            }
            else if (propType == typeof(DateTime?))
            {
                s = "DateTime.Now";
            }
            else if (propType.IsPrimitive)
            {
                s = "99";
            }
            else if (propType == typeof(Nullable<int>))
            {
                s = "99";
            }
            else
            {
                s = "null";
            }
            language = language.ToLower();
            if (language == "json")
            {
                if (s == "DateTime.Now")
                {
                    s = "new Date()";
                }
                else if (s == "Guid.NewGuid()")
                {
                    s = Guid.NewGuid().ToString();
                    s = string.Format("\"{0}\"", s);
                }
            }
            return s;
        }

        //===================================================================================================
        public static string GetSampleValueForFieldName(String fieldName)
        {
            string rval;
            var s = fieldName.ToLower();
            if (s.Contains("phone"))
            {
                rval = "702-123-4567";
            }
            else if (s.Contains("fax"))
            {
                rval = "(702) 123-7654";
            }
            else if (s.Contains("city"))
            {
                rval = "Las Vegas";
            }
            else if (s.Contains("state"))
            {
                rval = "NV";
            }
            else if (s.Contains("zip") || s.Contains("postal"))
            {
                rval = "89119";
            }
            else if (s.Contains("email"))
            {
                rval = "test1@abc.com";
            }
            else if (s.Contains("address"))
            {
                rval = "5757 Newton Wayne Blvd.";
            }
            else if (s.Contains("full")) //fullName
            {
                rval = "John Smiths";
            }
            else if (s.Contains("comment")
                  || s.Contains("adminnote"))
            {
                rval = "Testing only, please ignore this record";
            }
            else if (s == "createdby")
            {
                rval = "user1c";
            }
            else if (s == "updatedby")
            {
                rval = "user1u";
            }


            else
            {
                rval = FieldNameToLabel(fieldName) + " z";
            }

            return rval;
        }


    }
}
