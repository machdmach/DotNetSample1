using System.Collections;

namespace System.Data
{
    public class SqlWhereClauseBuilder : SqlWhereClauseBuilder_Base
    {
        public static string AddCondition(String wherec, string cond)
        {
            string rval;
            if (string.IsNullOrWhiteSpace(wherec))
            {
                rval = cond;
            }
            else
            {
                rval = wherec + " and " + cond;
            }
            return rval;
        }
        //===================================================================================================
        public bool HasConditions => _dbConds.Count > 0;
        //===================================================================================================
        public String ToStringEnclosedByParentheses()
        {
            return ToString("(", ")");
        }
        //===================================================================================================
        public String ToString(string prefix, string suffix)
        {
            string wc = ToString();
            string ret = "";
            if (!string.IsNullOrEmpty(wc))
            {
                //ret = " " + prefix + wc + suffix + " ";
                ret = prefix + wc + suffix;
            }
            return ret;
        }
        public override String ToString()
        {
            return ToStr("");
        }
        public String ToStr(string defaultVal)
        {
            if (_dbConds.Count == 0) { return defaultVal; }

            string s = IEnumerableX.ToString(_dbConds, "\n");
            if (s.StartsWith("and ")) { s = s.Substring(4); }
            if (s.StartsWith("or ")) { s = s.Substring(3); }
            s = " " + s + " ";
            //log.Debug("SqlWhereClauseBuilder.ToString()=" + s);
            return s;
        }

        //===================================================================================================
        public void And(String dbCond)
        {
            IsOr = false;
            AddCond(dbCond);
        }
        //===================================================================================================
        public void Or(String dbCond)
        {
            IsOr = true;
            AddCond(dbCond);
        }

        //===================================================================================================
        public void Or(DbSearchCriterion v)
        {
            AddCond(v, false);
        }
        //===================================================================================================
        public void And(DbSearchCriterion v)
        {
            AddCond(v, true);
        }
        //===================================================================================================
        protected void AddCond(DbSearchCriterion v, bool isAndCond = true)
        {
            IsOr = !isAndCond;

            if (v.IsEmpty)
            {
                //nothing;
            }
            else
            {
                string dbColumnName = v.DbColumnName;
                if (v.IsNull)
                {
                    AddCond(dbColumnName + " is null");
                }
                else if (v.IsNotNull)
                {
                    AddCond(dbColumnName + " is not null");
                }
                else
                {
                    object dbVal;
                    if (v.DataType == typeof(String))
                    {
                        dbVal = SqlLiteral.EscapeEncloseSQuote(v.StringValue);
                    }
                    else if (v.DataType == typeof(DateTime))
                    {
                        dbVal = "'" + v.DateTimeValue + "'";
                    }
                    else
                    {
                        dbVal = v.IntValue;
                    }
                    if (v.IsLike)
                    {
                        AddCond(dbColumnName + " like " + dbVal);
                    }
                    else if (v.IsNotLike)
                    {
                        AddCond(dbColumnName + " not like " + dbVal);
                    }
                    else if (v.IsNot)
                    {
                        AddCond(dbColumnName + " != " + dbVal);
                    }
                    else
                    {
                        AddCond(dbColumnName + " = " + dbVal);
                    }
                }
            }
            //return v;
        }

        //===================================================================================================
        private static void Addzz(StringBuilder buf, string label, string value, bool isWildcard = false, string operation = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            var v = new SearchFilterValue(value, false, isWildcard);
            string adjustedValue = v.StringValue;
            //spDisplay.RequestId = "RequestId " +

            string op = "=";
            string val = adjustedValue;

            if (v.IsNull)
            {
                op = "is";
                val = "null";
            }
            else if (v.IsNotNull)
            {
                op = "is";
                val = "not null";
            }
            else if (v.IsLike)
            {
                op = "like";
                //val = adjustedValue;
            }
            else if (v.IsNotLike)
            {
                op = "not like";
            }
            else if (v.IsNot)
            {
                op = "not";
            }
            else
            {
            }
            if (operation != null)
            {
                op = operation;
            }
            //string suffix =
            //    v.IsNull ? "is null" :
            //    v.IsNotNull ? "is not null" :
            //    v.IsLike ? "like " + adjustedValue :
            //    v.IsNotLike ? "not like " + adjustedValue :
            //    v.IsNot ? "not " + adjustedValue :
            //    " = " + adjustedValue;

            //val = StringX.

            //buf.AppendFormat("{0} <i>{1}</i>, ", label, suffix);

            string bgColor = "#FFFFCC";
            //string bgColor = "Yellow";
            if (!string.IsNullOrEmpty(val))
            {
                val = val.Replace('%', '*');
            }
            buf.AppendFormat("{0} {1} <span style='font-style: italic; color: Black; background-color:{3}'>{2}</span>, ",
                label, op, val, bgColor);
        }
    }
}