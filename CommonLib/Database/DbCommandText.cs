using System.Text.RegularExpressions;
//using System.Data.OleDb;

namespace Libx;
public class DbCommandText
{
    public static List<string> GetParameterNames(ref string sql, string parameterMarker)
    {
        List<string> ret = new List<string>();
        string regexPattern = @$"[^{parameterMarker}]{parameterMarker}([a-zA-Z_][\w]*)";
        Regex regex = new Regex(regexPattern, RegexOptions.Compiled);
        // statement = Regex.Replace(statement, @":(?=(?:'[^']*'|[^'\n])*$)", "@");
        //string output = Regex.Replace(input, @"(?<=\W):(?=\w+)", "@");


        //int markerLen = parameterMarker.Length;
        foreach (Match match in regex.Matches(sql))
        {
            string val = match.Groups[1].Value; //.Substring(markerLen);
            ret.Add(val);            
        }
        return ret;
    }

}

