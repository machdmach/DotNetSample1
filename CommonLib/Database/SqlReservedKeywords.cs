namespace Libx;
/// <summary>
/// #sqlKeyWords
/// </summary>
public static class SqlReservedKeywords2
{
    private static readonly HashSet<string> SqlServerKeywords; // = new HashSet<string>();
    static SqlReservedKeywords2()
    {
        //string[] ReservedKeywords = new string[] { "SELECT FROM WHERE GROUP BY ORDER" };
        string words = "SELECT FROM WHERE GROUP BY ORDER, INSERT UPDATE SET DELETE";
        SqlServerKeywords =  words.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet();

        SqlServerKeywords.Add("ADD");

        SqlServerKeywords.Add("EXISTS");
        SqlServerKeywords.Add("PRECISION");

        SqlServerKeywords.Add("EXEC");
        SqlServerKeywords.Add("PIVOT");
        SqlServerKeywords.Add("WITH");
        SqlServerKeywords.Add("EXECUTE");
        SqlServerKeywords.Add("PLAN");
        SqlServerKeywords.Add("WRITETEXT");
    }
    public static string SafeGuardToken(string token)
    {
        string tokenUpper = token.ToUpper();
        if (SqlServerKeywords.Contains(tokenUpper))
        {
            token = '"' + token + '"';
        }
        return token;
    }
    public static bool IsReserved(string token)
    {
        token = token.ToUpper();
        return SqlServerKeywords.Contains(token);
    }
}