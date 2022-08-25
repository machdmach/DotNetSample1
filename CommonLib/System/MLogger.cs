namespace Libx;
public class MyLogger : MLogger
{
    private static int LoggerIdCounter = 0;
    public MyLogger()
    {
        LoggerId = ++LoggerIdCounter;
        Clear();
    }
}
///<seealso cref="AppErrorHandler"/>
public abstract class MLogger
{
    public bool ShowCallerStack = false;
    protected int LoggerId;
    private int LogEntryIdCounter = 0;
    public string ExtensiveInfo = "";
    public CircularQueue<LogEntry> EntryQueue;
    public static CircularQueue<LogEntry> Last3Warnings = new CircularQueue<LogEntry>(3);
    public static CircularQueue<LogEntry> Last3Errors = new CircularQueue<LogEntry>(3);

    public void Clear()
    {
        EntryQueue = new CircularQueue<LogEntry>(3600);
        LogEntryIdCounter = 0;
    }

    //public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    public void Log(MyLogLevel logLevel, String state, Exception exception, Func<String, Exception, string> formatter)
    {
        throw new NotImplementedException();
    }
    public void Log(MyLogLevel logLevel, string? message, params object?[] args)
    {
        Log(logLevel, message, args);
    }

    public void Log(MyLogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        var ent = new LogEntry { LoggerId = LoggerId, LogLevel = logLevel };
        ent.EntryId = ++LogEntryIdCounter;

        message ??= "";
        if (args.Length > 0) message = String.Format(message, args);
        var mesg = message;
        if (exception != null)
        {
            mesg += "<br><br>\n" + exception.ToString();
        }

        if (ShowCallerStack)
        {
            var ignoredTypes = new List<Type> { typeof(MOutput), typeof(MLogger) };
            string methodStack = StackTraceX.GetStackInfo(ignoredTypes);
            methodStack = string.Format("<pre>{0}</pre>", methodStack);
            mesg += "<br>\n" + methodStack;
        }

        ent.Message = mesg;
        EntryQueue.Enqueue(ent);
        if (logLevel == MyLogLevel.Error)
        {
            Last3Errors.Enqueue(ent);
        }
        else if (logLevel == MyLogLevel.Warning)
        {
            Last3Warnings.Enqueue(ent);
        }
    }

    //===================================================================================================
    public void LogError(string? message, params object?[] args)
    {
        LogError(null, message, args);
    }
    public void LogError(Exception? exception, string? message = null, params object?[] args)
    {
        Log(MyLogLevel.Error, exception, message, args);
    }

    //===================================================================================================
    //logger.LogWarning("Processing request from {Address}", address)
    public void LogWarning(string? message, params object?[] args)
    {
        LogWarning(null, message, args);
    }
    public void LogWarning(Exception? exception, string? message, params object?[] args)
    {
        Log(MyLogLevel.Warning, exception, message, args);
    }

    //===================================================================================================
    public void LogInfo(string? message, params object?[] args)
    {
        Log(MyLogLevel.Information, null, message, args);
    }
    public void LogHtml(string? message, params object?[] args)
    {
        Log(MyLogLevel.Information, null, message, args);
    }
    public void LogHtmlValueOfAny(Object obj, ToHtmlOptions options = null)
    {
        string message = HtmlValue.OfAny(obj, options);
        object args = null;
        Log(MyLogLevel.Information, null, message, args);
    }
    public void LogInfo(Exception? exception, string? message, params object?[] args)
    {
        Log(MyLogLevel.Information, exception, message, args);
    }
    //LogTrace, LogDebug, LogCritical

    //===================================================================================================
    public string LogSql(DbCommand cmd)
    {
        string sql = DbCommandX.ToStr(cmd);
        LogSql(sql);
        return sql;
    }
    public static string LastSqlHtmlPre = "";
    //public static List<string> SqlList;
    public void LogSql(String sql)
    {
        sql = HtmlText.HtmlEncodeAndPre(sql.Trim());
        //sql = HtmlText.HtmlEncode(sql);

        LastSqlHtmlPre = sql + "On: " + DateTime.Now;
        //MOutput.WriteLine(LastRunSql);
        //MOutput.WriteHtmlEncodedPre(sql);
        //SqlList.Add(sql);
        LogInfo(sql);

        var sqlLower = sql.ToLower();
        //-------------------------------
        if (sqlLower.Contains("zzthrowuser"))
        {
            throw new Exception("You asked for this");
        }
        else if (sqlLower.Contains("zzthrow"))
        {
            throw new UserInputDataException("User err, You asked for this");
        }
    }
    ////===================================================================================================
    //public static string GetLastSqlList_Html()
    //{
    //    var buf = new StringBuilder();
    //    //foreach (var sql in SqlList)
    //    //{
    //    //    buf.AppendLine(sql);
    //    //    buf.AppendLine();
    //    //}
    //    buf.Append(LastRunSql);
    //    string s = HtmlText.HtmlEncode(buf.ToString());
    //    s = string.Format("<pre>{0} </pre>", s);
    //    return s;
    //}

    //Microsoft.Extensions.Logging.ILogger logger;
    //Logger<String> logger1; ILogger
    //public IDisposable BeginScope<TState>(TState state)
    //{
    //    throw new NotImplementedException();
    //}
    //public bool IsEnabled(MyLogLevel logLevel)
    //{
    //    throw new NotImplementedException();
    //}
    public string ToHtml(ToHtmlOptions? options = null)
    {
        options ??= new ToHtmlOptions { Caption = "Logger Content", ReverseRows = true };
        options.ReverseRows = false;
        return HtmlValue.OfList(EntryQueue.Select(e=>e.Message), options);
    }
}
