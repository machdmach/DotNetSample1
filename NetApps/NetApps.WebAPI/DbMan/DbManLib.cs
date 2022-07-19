using System.IO;

namespace Libx.Mvc.App;
public class DbManLib
{
    public static string ResolveSqlFilePath(string relPath)
    {
        string baseDir = MvcLib.GetAbsoluteFsPath("SqlScripts");
        string ret = Path.Combine(baseDir, relPath);
        return ret;
    }

    public static string ReadSqlFile(string relPath)
    {
        string pfname = ResolveSqlFilePath(relPath);
        string ret = System.IO.File.ReadAllText(pfname);
        return ret;
    }
}
