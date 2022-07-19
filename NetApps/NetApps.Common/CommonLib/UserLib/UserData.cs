using System.Text.RegularExpressions;

namespace Libx;
public class UserData
{

    //===================================================================================================
    public static DateTime? ParseDateTime(string str, bool isRequired = false, string label = null)
    {
        DateTime rval;
        if (string.IsNullOrWhiteSpace(str))
        {
            if (isRequired)
            {
                throw new UserInputDataException(label + " must not be blank");
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (!DateTime.TryParse(str, out rval))
            {
                throw new UserInputDataException(label + " " + str + " is not a valid date");
            }
        }
        return rval;
    }

    //===================================================================================================
    public static int ParseHour(string hh, string label = null)
    {
        if (string.IsNullOrWhiteSpace(hh))
        {
            return 0;
        }
        else
        {
            if (!Int32.TryParse(hh, out int rval))
            {
                throw new UserInputDataException(hh + " is invalid hour" + label);
            }
            if (rval < 0 || 23 < rval)
            {
                throw new UserInputDataException(hh + " is not in valid hour range 0-23, " + label);
            }
            return rval;
        }
    }
    //===================================================================================================
    public static int ParseMinute(string mm, string label = null)
    {
        if (string.IsNullOrWhiteSpace(mm))
        {
            return 0;
        }
        else
        {
            if (!Int32.TryParse(mm, out int rval))
            {
                throw new UserInputDataException(mm + " is invalid minute" + label);
            }
            if (rval < 0 || 23 < rval)
            {
                throw new UserInputDataException(mm + " is not in valid minute range 0-59, " + label);
            }
            return rval;
        }
    }

    //===================================================================================================
    public static DateTime ParseDateTime(String datePart, string timePart)
    {
        if (!DateTime.TryParse(datePart, out DateTime date1))
        {
            throw new UserInputDataException("Invalid date: " + datePart);
        }
        if (!TimeSpan.TryParse(timePart, out TimeSpan ts1))
        {
            throw new UserInputDataException("Invalid time: " + timePart);
        }
        var rval = date1.Add(ts1);
        return rval;
    }

    //===================================================================================================
    public static DateTime ParseDateTime(DateTime date1, string timePart)
    {
        if (!TimeSpan.TryParse(timePart, out TimeSpan ts1))
        {
            throw new UserInputDataException("Invalid time: " + timePart);
        }
        var rval = date1.Add(ts1);
        return rval;
    }


}