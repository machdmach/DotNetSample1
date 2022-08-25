namespace Libx;
public class MyHttpCacheStore
{
    MLogger logger;
    public MyHttpCacheStore(MLogger logger = null)
    {
        this.logger = logger ?? MyAppContext.Logger;
    }
    public bool NoRetrieve { get; set; }
    public bool NoSave { get; set; }

    public string BaseFolder { get; set; } = @"C:\temp1\";
    public bool TraceFlag = false;
    //===================================================================================================
    public virtual string GetFilePathFromUrl(string url)
    {
        Uri uri = UriX.CreateUriFromFullyUrl(url);
        string ret = GetFilePath(uri);
        return ret;
    }
    public virtual string GetFilePath(Uri uri)
    {
        string hostNormalized = uri.Host;
        hostNormalized = hostNormalized.ToLower();
        if (hostNormalized.StartsWith("www."))
        {
            hostNormalized = hostNormalized.Substring(4);
        }
        if (!uri.IsDefaultPort)
        {
            hostNormalized += "_" + uri.Port;
        }
        //string fname = PathX.SanitizeFileName(uri.PathAndQuery);
        string fname = uri.PathAndQuery;
        if (fname.StartsWith("/"))
        {
            fname = fname.Substring(1);
        }
        //fname = fname.Replace("\\", "_");
        fname = fname.Replace("/", "__");
        fname = fname.Replace("?", "$");
        if (fname == "")
        {
            fname = "_homepage_index.html";
        }
        //if (UriX.IsHtmlPage(fname)) //&& !fname.EndsWith(".html"))
        //{
        //    fname += ".html";
        //}
        string dirName = Path.Combine(BaseFolder, hostNormalized);
        if (!Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }
        string pfname = Path.Combine(dirName, fname);
        return pfname;
    }
    public bool IsFilePathOK(string pfname, bool forReading)
    {
        if (pfname.Length > 200)
        {
            if (forReading)
            {
                logger.LogInfo("Reading, FilePath is too long: " + pfname);
            }
            else
            {
                logger.LogInfo("Saving, FilePath is too long");
            }
            return false;
        }
        else
        {
            return true;
        }
    }
    //===================================================================================================
    //public string Save(string url, string content)
    //{
    //    Uri uri = UriX.CreateUriFromFullyUrl(url);
    //    string pfname = Save(uri, content);
    //    return pfname;
    //}
    public string Save(Uri uri, string content)
    {
        string pfname = GetFilePath(uri);
        SaveText(pfname, content);
        return pfname;
    }
    public virtual string SaveText(string pfname, string content)
    {
        if (NoSave)
        {
            return null; //-------------------------------------------
        }
        if (!IsFilePathOK(pfname, false))
        {
            return null; //-------------------------------------------
        }
        System.IO.File.WriteAllText(pfname, content);
        if (TraceFlag) MOutput.WriteLine("MyHttpCacheStore: Saved content to pfname=" + pfname);
        return pfname;
    }

    //===================================================================================================
    //public string TryRetrieve(string url)
    //{
    //    Uri uri = UriX.CreateUriFromFullyUrl(url);
    //    string content = TryRetrieve(uri);
    //    return content;
    //}
    public string TryRetrieve(Uri uri)
    {
        string pfname = GetFilePath(uri);
        string content = TryRetrieveText(pfname);
        return content;
    }
    public virtual string TryRetrieveText(string pfname)
    {
        if (NoRetrieve)
        {
            return null; //-------------------------------------------
        }
        if (!IsFilePathOK(pfname, true))
        {
            return null; //-------------------------------------------
        }
        string ret;
        if (File.Exists(pfname))
        {
            ret = System.IO.File.ReadAllText(pfname);
            if (TraceFlag) MOutput.WriteLine("MyHttpCacheStore: Retrieved content from pfname=" + pfname);
        }
        else
        {
            ret = null;
            if (TraceFlag) MOutput.WriteLine("MyHttpCacheStore: NOT EXIST, Retrieved content from pfname=" + pfname);
        }
        return ret;
    }
}