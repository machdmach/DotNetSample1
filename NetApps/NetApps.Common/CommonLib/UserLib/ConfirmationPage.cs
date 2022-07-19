using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    //public static class MiscExtentions
    //{
    //    public static string NameOf<TModel, TProperty>(this object @object, Expression<Func<TModel, TProperty>> propertyExpression)
    //    {
    //        var expression = propertyExpression.Body as MemberExpression;
    //        if (expression == null)
    //        {
    //            throw new ArgumentException("Expression is not a property.");
    //        }

    //        return expression.Member.Name;
    //    }
    //    //public string NameOf<TProperty>(Expression<Func<BACEReservationModel, TProperty>> propertyExpression)
    //    ////public string NameOf<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
    //    //{
    //    //    var expression = propertyExpression.Body as MemberExpression;
    //    //    if (expression == null)
    //    //    {
    //    //        throw new ArgumentException("Expression is not a property.");
    //    //    }

    //    //    return expression.Member.Name;
    //    //}
    //}


    public class ConfirmationPage<TModel>
    {
        private readonly Type type;
        private readonly TModel model;
        public HtmlTableWriterHNDVGT tab;
        public StringBuilder hiddenFieldsBuf = new StringBuilder();
        //public bool isDisplay = false;

        public string LabelSuffix = ":";

        public bool isOutputDisplay = true;
        public bool isOutputHidden = true;

        public string DateTimeFormat { get; set; }


        public ConfirmationPage(TModel model)
        {
            type = typeof(TModel);
            this.model = model;

            tab = new HtmlTableWriterHNDVGT();
            //htmlWriter.StartTable();
        }
        //===================================================================================================
        public string GetHtmlTable()
        {
            var ret = tab.EndTable();
            return ret;
        }
        public string GetHiddenFieldTags()
        {
            var ret = hiddenFieldsBuf.ToString();
            return ret;
        }

        //===============================================================================
        private bool sectionStarted = false;
        public void NewSection(string sectionHeader)
        {
            EndSection();

            var buf = tab.buf;

            if (sectionHeader != null)
            {
                //buf.AppendFormat("<h3 style='margin-bottom:0px;width: 100%;background-color:lightgray;'>{0}</h3><hr style='margin-top:0px' />\n", sectionHeader);
                buf.AppendFormat("<h3 style='margin-bottom:0px;width: 100%;background-color:lightgray;'>{0}</h3>\n", sectionHeader);
            }
            else
            {
                //buf.AppendLine("<br>");
                //buf.AppendLine("<br>");
            }
            buf.Append("<table border='1' cellpadding=3px style='padding:3px; border-collapse:collapse;border-spacing:0'>");

            tab.TableStarted = true;
            sectionStarted = true;
        }
        //===============================================================================
        public void EndSection()
        {
            tab.EndTable();
            if (sectionStarted)
            {
                //string sectionEndHtml = "</table>\n";
                //htmlWriter.buf.Append(sectionEndHtml);
            }
            sectionStarted = false;
        }

        //===================================================================================================
        public static void Test1()
        {
            Uri e = new Uri("");
            var p = new ConfirmationPage<Uri>(e);

            p.AddField(o => o.LocalPath, "x");
            p.AddField("Local Path", o => o.LocalPath);

            //Expression<Func<Uri>> exp =  (Uri o) => o.AbsolutePath;
            //var n = p.NameOf((T o) => o.AbsolutePath);
        }
        //===================================================================================================
        public void AddHidden<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            bool temp = isOutputDisplay;
            isOutputDisplay = false;

            AddField(propertyExpression, null);
            isOutputDisplay = temp;
        }
        //===================================================================================================
        public void AddField2<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, Expression<Func<TModel, TProperty>> propertyExpression2)
        {
            //tab.endTR = false;
            //c.AddField(o => o.ArrivalDate);
            //tab.startTR = false;
            //c.AddField(o => o.ETA_HHMM);
            //tab.Reset();

        }
        //===================================================================================================
        public void AddField<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var name = GetMemberName(propertyExpression);
            object value = GetPropertyValue(name);
            string label = GetPropertyDisplayName(propertyExpression);
            if (string.IsNullOrEmpty(label))
            {
                label = DataFieldDefHelper.FieldNameToLabel(name);
                //label = name;
            }
            AddField(name, label, value);
        }
        //===================================================================================================
        public void AddField<TProperty>(string fieldLabel, Expression<Func<TModel, TProperty>> propertyExpression)
        {
            string label = GetPropertyDisplayName(propertyExpression);
            AddField(propertyExpression, fieldLabel);
        }
        //===================================================================================================
        public void AddField<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, string fieldLabel)
        {
            var name = GetMemberName(propertyExpression);
            object value = GetPropertyValue(name);

            AddField(name, fieldLabel, value);
            //ObjectX
        }
        //===================================================================================================
        public void AddField<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, string fieldLabel, object value)
        {
            var name = GetMemberName(propertyExpression);
            AddField(name, fieldLabel, value);
        }
        //===================================================================================================
        public void AddField(string name, string fieldLabel, object value)
        {
            if (isOutputHidden)
            {
                hiddenFieldsBuf.AppendFormat("<input type='hidden' name='{0}' value='{1}'/>\n", name, value);
            }

            if (isOutputDisplay)
            {
                string valueStr = FormatValue(value);

                fieldLabel = fieldLabel + LabelSuffix;
                fieldLabel = "<b>" + fieldLabel + "</b>";
                tab.AddRow_NValues(fieldLabel, valueStr);
            }
            //GetName(exp);
        }
        //===================================================================================================
        public static String FormatValue(object val)
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
            //strVal += " &nbsp; " + val2;
            return strVal;
        }
        //===================================================================================================
        //===================================================================================================
        public string GetMemberName<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var expression = propertyExpression.Body as MemberExpression;
            if (expression == null)
            {
                throw new ArgumentException("Expression is not a property.");
            }
            var name = expression.Member.Name;
            return name;
        }
        //===================================================================================================
        public object GetPropertyValue(string name)
        {
            var prop = type.GetProperty(name);
            if (prop == null)
            {
                throw new Exception("no property found for name: " + name);
            }
            object value = prop.GetValue(model);
            return value;
        }

        //===================================================================================================
        //public string GetPropertyDisplayName(Expression<Func<TModel, object>> propertyExpression)
        public string GetPropertyDisplayName<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException("No property reference expression was found.", "propertyExpression");
            }

            var attr = GetAttribute<DisplayAttribute>(memberInfo, false);
            if (attr == null)
            {
                //return memberInfo.Name;
                //throw new Exception("No DisplayName Attribute found for field: " + memberInfo.Name);
                return null;
            }
            else
            {
                return attr.Name;
            }
        }

        //===================================================================================================
        public static T GetAttribute<T>(MemberInfo member, bool isRequired) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }
        //===================================================================================================
        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            //Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }

}
