//You cannot nest the seealso tag inside the summary tag.
/// <zsummary>
// <zseex cref="Temp"/>
/// </zsummary>
namespace Libx;
///<see cref="zTemp" href="https://abc.com"/>
public class StringConvert
{
    public static object ParseToType(string s, Type type)
    {
        return ParseToType(s, new TypeInfoX(type));
    }

    public static object ParseToType(string s, TypeInfoX type)
    {
        if (s == null)
        {
            return null;
        }
        object ret = DBNull.Value;
        //DBNull dBNull2 = ret;

        if (type.IsBoolean)
        {
            ret = ParseBoolean(s).Value;
        }
        else
        {
            ret = Convert.ChangeType(s, type.UnderlyingType);
        }
        return ret;
    }

    //===================================================================================================
    public static ParseResult<object> ParseToType(string s, Type type, ParseOptions options = null)
    {
        return ParseValue<object>(s, options, ret =>
        {
            ret.Value = Convert.ChangeType(s, type);
            ret.Success = true;
        });
    }

    //===================================================================================================
    public static ParseResult<Boolean> ParseBoolean(string s, ParseOptions options = null)
    {
        return ParseValue<Boolean>(s, options, ret =>
        {
            ret.Success = Boolean.TryParse(s, out bool val);
            if (!ret.Success)
            {
                var sLower = s.ToLower();
                if (sLower == "yes" || sLower == "y" || sLower == "true" || sLower == "t" ||  sLower == "on" || sLower == "1")
                {
                    ret.Value = true;
                    ret.Success = true;
                }
                else if (sLower == "no" || sLower == "n" || sLower == "false" || sLower == "f" || sLower == "off" || sLower == "0" || sLower == "none")
                {
                    ret.Value = false;
                    ret.Success = true;
                }
            }
        });
    }
    //===================================================================================================
    public static ParseResult<DateTime> ParseDateTime(string s, ParseOptions options = null)
    {
        return ParseValue<DateTime>(s, options, ret =>
        {
            ret.Success = DateTime.TryParse(s, out DateTime val);
            ret.Value = val;
        });
    }
    //===================================================================================================
    public static ParseResult<Int32> ParseInt32(string s, ParseOptions options = null)
    {
        return ParseValue<Int32>(s, options, ret =>
        {
            ret.Success = Int32.TryParse(s, out int val);
            ret.Value = val;
        });
    }
    //===================================================================================================
    public static ParseResult<T> ParseValue<T>(string s, ParseOptions options, Action<ParseResult<T>> parseFunc)// where T: ValueType
    {
        options ??= new ParseOptions();

        ParseResult<T> ret = new();
        string errMesg = options.ExceptionMessage;
        string label = options.Label;

        if (string.IsNullOrWhiteSpace(s))
        {
            if (options.Required)
            {
                ret.ErrorMesg = (label == null) ? "arg cannot be null or blank" :
                    label + " is required";
            }
        }
        else
        {
            parseFunc(ret);
            if (!ret.Success)
            {
                ret.ErrorMesg = (label == null) ? s + " is not a valid value" :
                    s + " is not a valid value for " + label;
            }
        }
        if (ret.ErrorMesg != null)
        {
            if (options.ExceptionType != null)
            {
                Exception ex = (Exception)Activator.CreateInstance(options.ExceptionType, ret.ErrorMesg);
                if (ex == null) { throw new Exception("never"); }
                throw ex;
            }
        }
        return ret;
    }
}

//===================================================================================================
public class ParseOptions
{
    public Type ExceptionType { get; set; } //
    public string ExceptionMessage { get; set; }
    public string Label { get; set; }
    public bool Required { get; set; }
}
//===================================================================================================
public class ParseEntityOptions
{
    public Type ExceptionType { get; set; }
    public bool ValidateRequired { get; set; }
    //public bool ValidateForInsert { get; set; }
}
public class ParseResult<T>
{
    public T Value { get; set; }
    public string ErrorMesg { get; set; }
    public bool Success { get; set; }
    //public bool Success => ErrorMesg != null;
}