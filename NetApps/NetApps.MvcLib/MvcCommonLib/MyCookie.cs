using System.Collections.Specialized;
using System.Data;
using System.Net;

namespace Libx.Mvc;
public class MyCookie //<T=string>
{
    ///Default Expires = "01/01/0001"  meaning expires when browser is closed.
    /// <summary>
    /// <see cref="CookieContainer"/>
    /// </summary>
    public string Name { get; set; }
    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }

    public MyCookie(string name, HttpRequest request) : this(name, request.HttpContext) { }
    public MyCookie(string name, HttpContext httpContext)
    {
        this.Name = name;
        this.Request = httpContext.Request;
        this.Response = httpContext.Response;
    }
    public string GetValue(string defaultVal = null)
    {
        string ret;
        if (Request.Cookies.TryGetValue(Name, out var cookieVal))
        {
            ret = cookieVal;
        }
        else
        {
            ret = defaultVal;
        }
        return ret;
    }
    public int GetInt(int defaultVal = 0)
    {
        int ret;
        string s = GetValue();
        if (s != null)
        {
            ret = Int32.Parse(s);
        }
        else
        {
            ret = defaultVal;
        }
        return ret;
    }
    public void SetValue(object val)
    {
        string sVal = val.ToString();
        Response.Cookies.Append(Name, sVal, new() { Expires = DateTimeOffset.MaxValue });
    }
    public void SetValueToSession(object val)
    {
        string sVal = val.ToString();
        Response.Cookies.Append(Name, sVal);
    }

    static string DecryptString(string mesg)
    {
        //return TryDecryptString(mesg, "?");
        return mesg;
    }
    static string EncryptString(string mesg)
    {
        //return TryDecryptString(mesg, "?");
        return mesg;
    }

    //public static String zToString(CookieCollection ckCol)
    //{
    //    NameObjectCollection nvs = new NameObjectCollection();

    //    for (int i = 0; i < ckCol.Count; i++)
    //    {
    //        Cookie ck = ckCol[i];
    //        nvs.Add("ck_" + i, ToString(ck));
    //    }
    //    foreach (string ckName in ckCol) //NameObjectCollectionBase
    //    {
    //    }

    //    return nvs.ToStr();
    //}
    //===================================================================================================

    public static String ToString(Cookie ck)
    {
        NameObjectCollection nvs = new NameObjectCollection();

        nvs.Add(ck.Name, ck.Value);
        nvs.Add("Name", ck.Name); //
        nvs.Add("Value", ck.Value); //
        nvs.Add("Domain", ck.Domain); //
        nvs.Add("Path", ck.Path); //
        nvs.Add("Expires", ck.Expires); //
        //nvs.Add("HasKeys", ck.HasKeys); //
        nvs.Add("HttpOnly (!client-side script accessible)", ck.HttpOnly); //
        nvs.Add("Secure", ck.Secure); //

        //WebRequestMethods wrm;

        StringBuilder buf = new StringBuilder();
        //NameValueCollection col = ck.Values;
        //if (col == null)
        //{
        //    buf.Append("null");
        //}
        //else
        //{
        //    foreach (String k in ck.Values.Keys) //Keys: NameObjectCollectionBase.KeysCollection
        //    {
        //        string v = ck.Values[k];//Not need cast for NVCol
        //                                //v = HttpServerUtility.UrlTokenEncode(base[]);
        //                                //HttpServerUtility u = new HttpServerUtility(); //err No ctor defined

        //        buf.Append(String.Format("[{0:}] = [{1}], ", k, v));
        //    }
        //}
        //nvs.Add("Values", buf.ToString());
        //return nvs.ToStr();
        return null;
    }


    //========================================================================
    //public static DataTable ToDataTable(CookieCollection ckCol)
    //{
    //    if (ckCol == null) return null;
    //    DataTable dt = new DataTable("CookieCollection");
    //    foreach (String k in ckCol.AllKeys)
    //    {
    //        Cookie ck = ckCol.Get(k);
    //        AppendToDataTable(ck, dt);
    //    }
    //    return dt;
    //}
    //========================================================================
    static void AppendToDataTable(Cookie ck, DataTable dt)
    {
        if (dt.Columns.Count == 0)
        {
            dt.Columns.Add("Name");
            dt.Columns.Add("DecryptedValue");
            dt.Columns.Add("Value");
            dt.Columns.Add("Domain");
            dt.Columns.Add("Path");
            dt.Columns.Add("Expires", typeof(DateTime));
            dt.Columns.Add("HasKeys", typeof(bool));
            dt.Columns.Add("HttpOnly", typeof(bool));
            dt.Columns.Add("Secure", typeof(bool));
            dt.Columns.Add("Values");
        }
        DataRow dr = dt.NewRow();

        StringBuilder buf = new StringBuilder();
        //NameValueCollection col = ck.Values;
        //if (col == null)
        //{
        //    buf.Append("null");
        //}
        //else
        //{
        //    foreach (String k in ck.Values.Keys) //Keys: NameObjectCollectionBase.KeysCollection
        //    {
        //        string v = ck.Values[k];//Not need cast for NVCol
        //                                //v = HttpServerUtility.UrlTokenEncode(base[]);
        //                                //HttpServerUtility u = new HttpServerUtility(); //err No ctor defined
        //        buf.Append(String.Format("[{0:}] = [{1}], ", k, v));
        //    }
        //}
        //string encryptedValue = ck.Value;
        //string decryptedValue;
        //try { decryptedValue = DecryptString(encryptedValue); }
        //catch (Exception e) { decryptedValue = e.Message; }

        //String values = buf.ToString();
        ////if (values.StartsWith("[]")) { values = ""; }
        //dr["Name"] = ck.Name;
        //dr["Value"] = (encryptedValue.Length < 20) ? encryptedValue : encryptedValue.Substring(0, 19) + "...";
        //dr["DecryptedValue"] = decryptedValue;
        //dr["Values"] = values;

        //dr["Domain"] = ck.Domain; //
        //dr["Path"] = ck.Path; //
        //dr["Expires"] = ck.Expires; //
        //dr["HasKeys"] = ck.HasKeys; //
        //dr["HttpOnly"] = ck.HttpOnly; // (!client-side script accessible)
        //dr["Secure"] = ck.Secure; //

        dt.Rows.Add(dr);
    }
    public static Cookie CloneCookie(Cookie cookie)
    {
        Cookie clonedCookie = new Cookie(cookie.Name, cookie.Value);
        clonedCookie.Domain = cookie.Domain;
        clonedCookie.Expires = cookie.Expires;
        clonedCookie.HttpOnly = cookie.HttpOnly;
        clonedCookie.Path = cookie.Path;
        clonedCookie.Secure = cookie.Secure;
        return clonedCookie;
    }

    //*****************************************************************************
    //*****************************************************************************
    //*****************************************************************************
    //*****************************************************************************

    //public static Cookie AddToResponse(string key, object value)
    //{
    //    //Create new cookie
    //    Cookie newCookie = new Cookie(key);

    //    //Configure cookie
    //    //newCookie.Domain = "yoursite.com";
    //    //newCookie.Expires = new DateTime(2006, 12, 1);
    //    newCookie.Name = key;
    //    newCookie.Path = "/";
    //    newCookie.Secure = false;
    //    newCookie.Value = EncryptString(value.ToString());
    //    //Add cookie to response object
    //    //if (Contains(key))
    //   // HttpContext.Current.Response.Cookies.Set(newCookie);
    //    //else
    //    //    Page.Response.Cookies.Add(newCookie);
    //    return newCookie;
    //}
    //========================================================================
    //public static Cookie ClearCookie(String cookieName)
    //{
    //    var aCookie = new Cookie(cookieName);
    //    //if (aCookie.Name != "__utmz")
    //    {
    //        aCookie.Value = "";    //set a blank value to the cookie 
    //        aCookie.Expires = DateTime.Now.AddDays(-1);

    //        HttpContext.Current.Response.Cookies.Add(aCookie);
    //    }
    //    return aCookie;
    //}
}

