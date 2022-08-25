using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace Libx;
public class PathX
{
    [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

    public static void WNetGetConnection_eg()
    {
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType.Equals(System.IO.DriveType.Network))
            {
                StringBuilder uncPathBuf = new StringBuilder(255);
                int len = uncPathBuf.Capacity; //Capacity of the string builder (required for the Win32 API call

                string deviceName = drive.Name;
                if (deviceName.EndsWith(@"\")) { deviceName = deviceName.Substring(0, deviceName.Length - 1); }

                WNetGetConnection(deviceName, uncPathBuf, ref len);

                //LogX.Debug(String.Format("Device '{0}' is a networkDrive that mapped to (unc): '{1}'", deviceName, uncPathBuf.ToString()));
            }
            else
            {
                //LogX.Debug("Drive is not a Network drive: " + drive.Name);
            }
        }
    }
    public static string FlattenPath(string path)
    {
        path = Regex.Replace(path, @"[\W]+", "_");  //Non-Word chars a-z0-9         
        return path;
        //int i_colon = path.IndexOf(':');
        //if (i_colon >= 0)
        //{
        //   path = path.Substring(i_colon + 1);
        //}         
        //C:\path to\dir 2
        //path = path.Replace(':', '_');
        //path = path.Replace('\\', '_');
        //path = path.Replace('/', '_');
        //path = path.Replace(' ', '_');
        //return path;
    }


    public static string GetFolderNameAt(string pfname, char folderSep = '\\', int positionInHierarchy = 0)
    {
        var segments = pfname.Split(new char[] { folderSep }, StringSplitOptions.RemoveEmptyEntries);
        var ret = segments[positionInHierarchy];
        return ret;
    }

    //========================================================================
    public static string[] BinaryFileExtentions = new string[] { ".exe", ".bin", ".dll", ".pdb", ".zip", ".gzip", ".doc", ".iso" };
    public static bool zIsBinaryExtension(string path)
    {
        foreach (string ext in BinaryFileExtentions)
        {
            if (path.EndsWith(ext)) { return true; }
        }
        return false;
    }

    public static string[] WebFileExtentions = new string[] {
     ".xml",
     ".xslt",
     ".xsl",
     ".xsd",
     ".htm",
     ".html",
     ".xhtml",
     ".css",
     ".js",
     ".php",
     ".php5",
     ".pl",
     ".iso",
  };
    public static string[] MiscTextFileExtentions = new string[] {
     ".bat",
     ".build",
     ".ant",
     ".conf",
     ".cfg",
     ".ini",
     ".properties",
     ".info",
     ".svg",
     ".text",
     ".txt",
     ".c",
     ".sql",


  };
    public static string[] DotNetSourceCodeExtentions = new string[] {
     ".asax",
     ".ascx",
     ".aspx",
     ".csproj",
     ".sln",
     ".cs",
     ".vb",
     ".config",
     ".exclude",
     ".skin",
     ".sitemap",
     ".master",
  };
    public static bool IsBinaryFileExtension(string path)
    {
        return !IsTextFileExtension(path);
    }
    public static string[] TextFileExtentions =
        StringArray.Union(DotNetSourceCodeExtentions,
              //StringArray.Union(JavaSourceCodeExtentions,
              StringArray.Union(WebFileExtentions, MiscTextFileExtentions));

    public static bool IsTextFileExtension(string path)
    {
        string[] fileExts = TextFileExtentions;

        string ext = Path.GetExtension(path);
        if (ext == "") { return true; } //assuming files without extension are text file

        foreach (string s in fileExts)
        {
            if (s == ext) { return true; }
        }
        return false;
    }

    //====================================================================
    public static string GetTempVirtualPath(string relPath)
    {
        if (relPath.StartsWith("/") || relPath.StartsWith("~"))
        {
            throw new Exception("relPath must not start with / nor ~: " + relPath);
        }
        relPath = "Temp/" + relPath;
        return GetStorageVirtualPath(relPath);
    }
    //Uri AbsolutePath=/webdesign/Admin1/Info.aspx 
    public static string GetTempPhysicalPath(string relPath)
    {
        string tempPath = GetTempVirtualPath(relPath);

        //HttpContext ctxt = HttpContext.Current;
        return "?"; //ctxt.Server.MapPath(tempPath);
    }
    //====================================================================
    public static string GetTempFileName_cm()
    {
        return Path.GetRandomFileName();
    }

    //==============================================================
    public static string GetStoragePhysicalPath(string relPath)
    {
        string virtualPath = GetStorageVirtualPath(relPath);
        //return HttpContext.Current.Request.MapPath(virtualPath);
        return "?";
    }
    //==============================================================
    public static string GetApp_DataPhysicalPath(string relPath)
    {
        //string s = GetStoragePhysicalPath(relPath);
        //s = s.Replace("App_Storage", "App_Data");
        string s = GetStoragePhysicalPath(Path.Combine("App_Data", relPath));

        return s;
    }
    //==============================================================
    public static string GetStorageVirtualPath(string relPath)
    {
        return null;
        //if (relPath.StartsWith("/") || relPath.StartsWith("~"))
        //{
        //    throw new Exception("relPath must not start with / nor ~: " + relPath);
        //}
        ////if (relPath.StartsWith("/")) { relPath = relPath.Substring(1); }

        ////Can't put in PathX class b/c of IsRemoteProdbasdfasdf
        //if (EnvironmentX.IsProd)
        //{
        //    return "~/App_Storage/Prod/" + relPath;
        //}
        //else if (EnvironmentX.IsRemoteTest)
        //{
        //    return "~/App_Storage/Test/" + relPath;
        //}
        //else
        //{
        //    return "~/App_Storage/Dev/" + relPath;
        //}
    }


    //====================================================================

    public static NameObjectCollection ToNVs(string path)
    {
        NameObjectCollection nvs = new NameObjectCollection();
        string s;
        nvs.Add("Original Path", path);

        nvs.Add("GetDirectoryName", Path.GetDirectoryName(path)); //D:\path\to\a
        nvs.Add("GetFileName", Path.GetFileName(path)); //dir
        nvs.Add("GetFileNameWithoutExtension", Path.GetFileNameWithoutExtension(path)); //dir
        nvs.Add("GetPathRoot", Path.GetPathRoot(path)); //D:\
        nvs.Add("IsPathRooted", Path.IsPathRooted(path)); //true
        nvs.Add("AltDirectorySeparatorChar", Path.AltDirectorySeparatorChar); // /
        nvs.Add("ChangeExtension", Path.ChangeExtension(path, "newExt")); // dir.newExt
        //nvs.Add("GetInvalidFileNameChars", Path.GetInvalidFileNameChars()); //

        try { s = Path.GetTempPath(); }
        catch (Exception e) { s = e.Message; }
        nvs.Add("GetTempPath", s);

        try { s = Path.GetTempFileName(); } ////C:\DOcument and Settings....\tmp1BD.tmp
        catch (Exception e) { s = e.ToString(); }

        nvs.Add("GetTempFileName, created zero-byte temp file on disk", s);

        nvs.Add("GetRandomFileName", Path.GetRandomFileName()); // g33mlikj.bgk

        return nvs;
    }
    //public static string ToString(string path)
    //{
    //    return NameObjectCollection.ToHtml(ToNVs(path));
    //}
    public static string GetHomeDirectory()
    {
        if (NotRunThis) //PlatformHelper.IsUnix)
        {
            return Environment.GetEnvironmentVariable("HOME");
        }
        else
        {
            return Environment.GetEnvironmentVariable("USERPROFILE");
        }
    }


    public static string GetCleanedFileNameFromPath(string pfname)
    {
        //if (string.IsNullOrEmpty(pfname))
        //{
        //    return pfname;
        //}
        string fname = Path.GetFileName(pfname);
        return SanitizeFileName(pfname);
    }

    //===================================================================================================
    public static string SanitizeFolderName(string folderName)
    {
        //// Remove . \ / | : ? * " < >
        //return Regex.Replace(folderName, @"[.\\/|:?*""<>\p{C}]", "_", RegexOptions.None);
        return SanitizeFileName(folderName);
    }

    //===================================================================================================
    public static string SanitizeFileName(string fileName)
    {
        //// Replace dots in the name with underscores (only one dot can be there... security issue).
        //fileName = Regex.Replace(fileName, @"\.(?![^.]*$)", "_", RegexOptions.None);

        //// Remove \ / | : ? * " < >
        //return Regex.Replace(fileName, @"[\\/|:?*""<>\p{C}]", "_", RegexOptions.None);

        if (string.IsNullOrEmpty(fileName))
        {
            return fileName;
        }
        var buf = new StringBuilder();
        foreach (char c in fileName)
        {
            if (char.IsLetterOrDigit(c) ||
                char.IsWhiteSpace(c) ||
                c == '_' ||
                c == '-' ||
                c == '.' ||
                c == '(' ||
                c == ')' ||
                c == '+' ||
                c == '&')
            {
                buf.Append(c);
            }
        }
        string rval = buf.ToString();
        rval = StringX.CompressWhiteSpaces(rval);
        return rval;
    }

}
