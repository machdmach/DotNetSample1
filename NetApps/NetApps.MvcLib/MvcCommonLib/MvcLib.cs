using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Net;

namespace Libx.Mvc;
public static class MvcLib
{
    public static bool IsAccept(HttpRequest Request, string requestMediaType)
    {
        MediaType parsedRequestMediaType = new MediaType(requestMediaType);

        StringValues requestAccept = Request.Headers[HeaderNames.Accept];
        if (StringValues.IsNullOrEmpty(requestAccept))
        {
            return true;
        }

        //if (MvcLib.IsSubsetOfAnyContentType(requestAccept, ContentTypes))
        //    return true;
        var mediaType = new MediaType(requestAccept);
        return parsedRequestMediaType.IsSubsetOf(mediaType);
    }

    //===================================================================================================
    public static bool IsSubsetOfAnyContentType(string targetMediaType, MediaTypeCollection mediaTypes)
    {
        MediaType parsedRequestMediaType = new MediaType(targetMediaType);
        for (var i = 0; i < mediaTypes.Count; i++)
        {
            string mediaTypeStr = mediaTypes[i];
            var mediaType = new MediaType(mediaTypeStr);
            if (parsedRequestMediaType.IsSubsetOf(mediaType))
            {
                return true;

            }
        }
        return false;
    }
    //===================================================================================================
    public static bool IsLocalRequest(HttpRequest req)
    {
        HttpContext context = req.HttpContext;
        bool ret = context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress) ||
             IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
        //IPAddress.IsLoopback(context.Connection.LocalIpAddress);
        return ret;
    }
    //===================================================================================================
    public static String GetAbsoluteFsPath(string relPath)
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        if (ConfigX.IsIISExpress)
        {
            //baseDir = @"C:\wdo\PublicBackendApps\NetApps\NetApps.WebAPI\bin\Debug\net6.0";
            baseDir = baseDir.Replace(@"\bin\Debug\net6.0", "");
        }
        var ret = Path.Combine(baseDir, relPath);
        return ret;
    }
    //===================================================================================================
    public static bool IsUrlExternal(string hostname)
    {
        hostname += "";
        //var url = this.AbsoluteUri;
        //var hostnameLower = new Uri(url).Host.ToLower(); //url.ToLower();

        var hostnameLower = hostname.ToLower();
        return
            hostnameLower.EndsWith(".us") ||
            hostnameLower.EndsWith(".com") ||
            hostnameLower.EndsWith(".aero");
    }
    //===================================================================================================
    public static string ResolveHtmlTextToAbsolutePath(string html)
    {
        string relPath = "~/";
        string absPath = ResolveToAbsolutePath(relPath);
        string ret = html.Replace(relPath, absPath);
        return ret;
    }
    //===================================================================================================
    public static string ResolveToAbsolutePath(string relativePath)
    {
        var applicationPath = ConfigX.WebAppVirtualPath; //Request.PathBase;
        if (string.IsNullOrEmpty(relativePath))
        {
            return applicationPath;
        }
        else
        {
            if (relativePath[0] == '~')
            {
                //var segment = new PathString(relativePath.Substring(1));
                relativePath = relativePath.Substring(1);
                //return applicationPath + segment; //applicationPath.Add(segment).Value;
            }
            return applicationPath + relativePath;
        }
    }
    //===================================================================================================
    public static string AppMainPageUrl
    {
        get
        {
            var url = AppSettingsX.GetString("AppMainPageUrl");
            url = MvcLib.ResolveToAbsolutePath(url);
            return url;
        }
    }
}
