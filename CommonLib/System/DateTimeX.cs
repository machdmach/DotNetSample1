using System.Globalization;
using System.Text.RegularExpressions;
namespace Libx;
public static class DateTimeX
{
    public static Boolean IsBefore(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        //isCurrrentDateTimeBefore
        var targetDate = new DateTime(year, month, day, hour, minute, second);
        var ret = DateTime.Now < targetDate;
        return ret;
    }
    public static Boolean IsAfter(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        //isCurrrentDateTimeAfter
        var targetDate = new DateTime(year, month, day, hour, minute, second);
        var ret = targetDate < DateTime.Now;
        return ret;
    }

    /*
    //22 Feb 2020 18:13:00
                if (DateTime.TryParseExact(created, "dd MMM yyyy HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
    */

    public static double ToUnixEpochTicks(DateTime dt)
    {
        // returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates js:new Date().getTime())
        DateTime d1 = new DateTime(1970, 1, 1);
        DateTime d2 = dt.ToUniversalTime();
        TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
        return ts.TotalMilliseconds;
    }
    public static int UnixEpochNowSeconds()
    {
        //var epocDt = new DateTime(2020, 3, 20, 0, 0, 0, DateTimeKind.Utc);
        var epocDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan ts = DateTime.UtcNow - epocDt;
        int secondsSinceEpoch = (int)ts.TotalSeconds;
        return secondsSinceEpoch;
    }

    //s: DateTimeFormatInfo.SortableDateTimePattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss".      

    //http://www.geekzilla.co.uk/View00FF7904-B510-468C-A2C8-F859AA20581F.htm


    //yyyy-MM-dd  2009-04-24

    //update pottery set date_modified = '1-Mar-2004 3:23:23'
    //update pottery set date_modified = '1/2/03'  --1/2/2003 00:00:00

    //M, MM, MMM, MMMM:  3, 03, Mar, March
    //d, dd, ddd:        9, 09, Wed=DateTimeFormatInfo.AbbreviatedDayNames 
    // HH:mm:ss tt;  23:59:59 AM/PM

    //d: MM/dd/yyyy     12/31/2005

    //DateTime dt = o as DateTime; //Error must be refenceType
    public static string Reformat_z()
    {

        string s = "12/28/2005";
        Match m = Regex.Match(s, @"\b(?<month>\d{1,2})/(?<day>\d{1,2})/(?<year>\d{2,4})\b");
        string dateStr = m.Groups["year"] + "-" + m.Groups["month"] + "-" + m.Groups["day"];

        //LogX.Debug("{0} reformatted = {1}", s, dateStr);
        return dateStr;
    }
    //DateTimeX.Parse("22/4/2008 5:00:38");

    public static string ToOracleDateString_copyMe(DateTime dt)
    {
        //DateTimeFormatInfo dtfi = DateTimeFormatInfo.ReadOnly;
        //         dtfi.SortableDateTimePattern
        return dt.ToString("dd-MMM-yyyy");
    }
    public static string ToOracleSqlText_copyMe(DateTime dt)
    {
        //select to_date('7/30/2007 6:34:47 PM', 'mm/dd/yyyy hh:mi:ss PM') from dual;
        //nvs.Add("ToString()", dt.ToString()); // 7/30/2007 6:34:47 PM
        //string s = dt.ToString();
        return string.Format("to_date('{0}', ''mm/dd/yyyy hh:mi:ss PM')", dt.ToString());
    }

    public const string dd_MMM_yyyy = "dd-MMM-yyyy";
    public const string MySQL_DateTime_Format = "yyyy-MM-dd HH:mm:ss"; //'2008-11-12 19:30:10'
                                                                       //SELECT STR_TO_DATE('04/31/2004', '%m/%d/%Y');    -> '2004-04-31'




    public static string toISOString(DateTime dt)
    {
        //AppRunIdLong = DateTime.Now.ToString("s").Replace(':', '-'); //s=sortable, 2022-05-03T18-31-47
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string fmt = "yyyy-MM-dd'T'HH:mm:ss.fffK"; //"2017-06-26T20:45:00.070Z"
        string s = dt.ToString(fmt, CultureInfo.InvariantCulture);
        return s;
    }

    public static string toLocaleDateString(DateTime dt)
    {
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string fmt = "MM/dd/yyyy"; //""
        string s = dt.ToString(fmt, CultureInfo.InvariantCulture);
        return s;
    }

    public static string toLocaleTimeString(DateTime dt)
    {
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string fmt = "hh:mm:ss tt"; //"10:45:00 PM"
        string s = dt.ToString(fmt, CultureInfo.InvariantCulture);
        return s;
    }


    public static string ToString_dd_MMM_yyyy(DateTime dt)
    {
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string s = dt.ToString("dd-MMM-yyyy");
        return s;
    }

    public static string ToString_dd_MMM_yyyy_at_hh_mm_tt(DateTime dt)
    {
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string s = dt.ToString("dd-MMM-yyyy 'at' hh:mm tt");
        return s;
    }

    public static string yyyy_MM_dd__HH_mm_ss = "yyyy_MM_dd__HH_mm_ss";
    public static string ToString_yyyy_MM_dd__HH_mm_ss(DateTime dt)
    {
        //if (dt == null || dt == default(DateTime)) { return ""; }

        string s = dt.ToString(yyyy_MM_dd__HH_mm_ss);
        return s;
    }
    public static DateTime FromString_yyyy_MM_dd__HH_mm_ss(string s)
    {
        if (s == null) { return default(DateTime); }
        DateTime dt = DateTime.ParseExact(s, yyyy_MM_dd__HH_mm_ss, CultureInfo.CurrentCulture);
        return dt;
    }
    //========================================================================
    // Calculate the week number according to ISO 8601

    public static int Iso8601WeekNumber(DateTime dt)
    {
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
    public static int CurrentWeekOfMonth()
    {
        DateTime dt = DateTime.Now;
        // Calculate the WeekOfMonth according to ISO 8601
        int weekOfMonth = Iso8601WeekNumber(dt) - Iso8601WeekNumber(dt.AddDays(1 - dt.Day)) + 1;
        return weekOfMonth;
    }

    public static string ToMinutesSeconds(TimeSpan timeSpan)
    {
        string rval;
        if (timeSpan.TotalMilliseconds < 1000.0)
        {
            rval = string.Format("{0} millis", (long)timeSpan.TotalMilliseconds);
        }
        else
        {
            double totalSecondsDouble = timeSpan.TotalSeconds;
            long totalSecondsLong = (long)totalSecondsDouble;
            long minutes = totalSecondsLong / 60;
            long seconds = totalSecondsLong % 60;
            if (minutes > 0)
            {
                rval = string.Format("{0} minutes and {1} seconds", minutes, seconds);
            }
            else
            {
                rval = string.Format("{0} seconds", seconds);
            }
        }
        return rval;
    }

    //===================================================================================================
    public static bool TryParseTime(string time, out TimeSpan ts)
    {
        bool ok = TryParseTime(time, out ts, out string reason);
        if (ok)
        {
            MOutput.WriteLineFormat("{0} = {1}", time, ts);
        }
        else
        {
            MOutput.WriteLineFormat("!!!  {0}:  Invalid time, {1}", time, reason);
        }
        return ok;
    }
    public static bool TryParseTime(string time, out TimeSpan ts, out string reason)
    {
        return TimeParser.TryParseTime(time, out ts, out reason);
    }
    //===================================================================================================
    public static DateTime SetTime(DateTime date, string time)
    {
        DateTime ret;
        if (string.IsNullOrEmpty(time))
        {
            ret = date;
        }
        else
        {
            if (!TryParseTime(time, out TimeSpan ts, out string reason))
            //if (!TimeSpan.TryParse(time, out ts))
            {
                throw new UserInputDataException("Invalid time: " + reason);
            }
            else
            {
                ret = date.Add(ts);
            }

        }
        return ret;

    }
    //===================================================================================================
    public static DateTime? SetTime(DateTime? date, string time)
    {
        DateTime? ret;
        if (string.IsNullOrEmpty(time))
        {
            ret = date;
        }
        else
        {
            if (!TryParseTime(time, out TimeSpan ts, out string reason))
            //if (!TimeSpan.TryParse(time, out ts))
            {
                throw new UserInputDataException("Invalid time: " + reason);
            }
            if (date.HasValue)
            {
                ret = date.Value.Add(ts);
            }
            else
            {
                ret = DateTime.Today.Add(ts);
            }

        }
        return ret;
    }
    //===================================================================================================
    public static DateTime? SetTime(DateTime? date, int? hh, int? mm)
    {
        DateTime? rval = null;
        if (!hh.HasValue)
        {
            hh = 0;
        }
        if (!mm.HasValue)
        {
            mm = 0;
        }
        if (!date.HasValue)
        {
            date = DateTime.Now;
        }
        //if (date.HasValue && hh.HasValue && mm.HasValue)
        {
            var dt = date.Value;
            if (hh < 0 || 23 < hh)
            {
                throw new Exception("invalid hour: " + hh);
            }
            if (mm < 0 || 59 < mm)
            {
                throw new Exception("invalid minute: " + mm);
            }

            rval = new DateTime(dt.Year, dt.Month, dt.Day, hh.Value, mm.Value, 0);
        }
        return rval;
    }
}
public class DateTimeRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }


}

public class Periodz
{
    public DateTime EffectiveDate { get; set; }
    public DateTime ThroughDate { get; set; }
}

public class Timelinez
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
