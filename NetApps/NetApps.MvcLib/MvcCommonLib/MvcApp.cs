using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;

namespace Libx.Mvc;
public class MyAppSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> AppSettings { get; set; } //= new Dictionary<string, string>();
    //[JsonConstructor]
    public MyAppSettings()
    {
        //IEqualityComparer<String> equalityComparer = StringComparer.OrdinalIgnoreCase;
        ////ConnectionStrings = new Dictionary<string, string>(equalityComparer);
        //AppSettings = new Dictionary<string, string>(equalityComparer);
        //AppSettings.Add("kkkkkkk", "not working here");
    }

    //===================================================================================================
    public static MyAppSettings ReadConfigFile(string jsonFilename)
    {
        var options = new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true, MaxDepth = 4, PropertyNameCaseInsensitive = true };

        var fullPath = MvcLib.GetAbsoluteFsPath(jsonFilename);
        MOutput.WriteLine("Reloading Config File: " + fullPath);

        var jsonText = System.IO.File.ReadAllText(fullPath);
        //MOutput.Write("fileContent read for[" + fullPath + "]:<br>\n" + jsonText);
        var ret = JsonSerializer.Deserialize<MyAppSettings>(jsonText, options);
        //string s1 = JsonSerializer.Serialize(ret, options);
        //MOutput.Write("serialized again:<br>\n" + s1);
        return ret;
    }
}
//===================================================================================================
public static class MvcApp
{
    static DateTime ConfigFileLastLoadedOn = DateTime.MinValue;
    public static void ReloadConfigFiles(bool forceReload = false)
    {
        //MOutput.WriteLine("Reloading Config Files");
        var fullPath = MvcLib.GetAbsoluteFsPath("appSettings.json");
        var fi = new FileInfo(fullPath);
        if (fi.LastWriteTime == ConfigFileLastLoadedOn)
        {
            if (!forceReload)
            {
                MOutput.Write("Config file unchanged, not reload");
                return;
            }
        }
        ConfigFileLastLoadedOn = fi.LastWriteTime;

        var settings = MyAppSettings.ReadConfigFile("appSettings.json");
        var commonSettings = MyAppSettings.ReadConfigFile("appSettingsX.Common.json");

        var appSettings = new Dictionary<string, string>(settings.AppSettings, StringComparer.OrdinalIgnoreCase);
        appSettings.LeftJoinIn(commonSettings.AppSettings);

        var connectionStrings = new Dictionary<string, string>(settings.ConnectionStrings, StringComparer.OrdinalIgnoreCase);
        connectionStrings.LeftJoinIn(commonSettings.ConnectionStrings);

        AppSettingsX.AppSettings = appSettings;
        DbConnectionString.ConnectionStrings = connectionStrings;

        if (NotRunThis)
        {
            MOutput.WriteLine(HtmlValue.OfList(AppSettingsX.AppSettings.Keys, new() { Caption = "AppSettings reloaded" }));
            MOutput.WriteLine(HtmlValue.OfList(DbConnectionString.ConnectionStrings.Keys, new() { Caption = "ConnectionStrings reloaded" }));
        }

        ConfigX.AppName = AppSettingsX.GetString("AppName");
    }

    //===================================================================================================
    //public static void ReloadConfigFiles2()
    //{
    //    MOutput.WriteLine("Reloading Config Files 1");

    //    ////Configuration = configuration;
    //    ////var as1 = config.GetSection("AppSettings");
    //    //config.GetReloadToken().RegisterChangeCallback((state) =>
    //    //{
    //    //    throw new Exception("xx");

    //    //    AppSettingsX.AppSettings = Config_getNVC(config, "appSettings");
    //    //    MyDbConnection.ConnectionStrings = Config_getNVC(config, "conNectionStrings");
    //    //}, null);


    //    //MyDbConnection.ConnectionStrings = Config_getNVC(config, "conNectionStrings");
    //    //AppSettingsX.AppSettings = Config_getNVC(config, "appSettings");

    //    IConfigurationRoot config = new ConfigurationBuilder()
    //        //.SetBasePath(env.ContentRootPath)
    //        //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)

    //        .AddJsonFile("appSettingsX.Common.json", optional: false, reloadOnChange: true)
    //        .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
    //        .AddJsonFile("settings2.json", optional: true, reloadOnChange: false)
    //        .Build();

    //    //AppSettingsX.AppSettings = Config_getNVC(config, "appSettings");
    //    //MyDbConnection.ConnectionStrings = Config_getNVC(config, "conNectionStrings");

    //    //MOutput.WriteLine(AppSettingsX.AppSettings.ToHtml(new ToHtmlOptions { Caption="AppSettings reloaded"}));
    //    //MOutput.WriteLine(MyDbConnection.ConnectionStrings.ToHtml(new ToHtmlOptions { Caption = "AppSettings reloaded" }));

    //    // register change callback
    //    //ChangeToken.OnChange(
    //    //    () => configuration.GetReloadToken(),
    //    //    () =>
    //    //    {
    //    //        logger.Info("Configuration file has been changed"); //want to know specifically what has changed
    //    //    }
    //    //);
    //}

    //===================================================================================================
    public static NameValueCollection Config_getNVC(IConfiguration config, string sectionName)
    {
        var ret = new NameValueCollection();
        var configSection = config.GetSection(sectionName);
        if (!configSection.Exists())
        {
            throw new Exception("No configSection found for: " + sectionName);
        }
        IEnumerable<KeyValuePair<string, string>> kvs = configSection.AsEnumerable(true);
        var list = kvs.ToList();

        list.ForEach(kv =>
        {
            if (ret[kv.Key] == null) { ret.Add(kv.Key, kv.Value); }
        });
        return ret;

    }

    //===================================================================================================
    //public static String GetInfo_Html()
    //{
    //    var nvs = new NameObjectCollection();
    //    try
    //    {
    //        //web stuff
    //        //nvs.Add("IsDevelopmentEnvironment", HostingEnvironment.IsDevelopmentEnvironment);
    //        //nvs.Add("IsHosted", HostingEnvironment.IsHosted);

    //        //try { nvs.Add("HttpContext.Current.Request.LogonUserIdentity.Name", HttpContext.Current.Request.LogonUserIdentity.Name); } catch { }
    //        //try { nvs.Add("HttpContext.Current.User.Identity.Name", HttpContext.Current.User.Identity.Name); } catch { }

    //        //nvs.Add("MaxConcurentRequestPerCPU", HostingEnvironment.MaxConcurrentRequestsPerCPU);
    //        //nvs.Add("MaxConcurrentThreadsPerCPU", HostingEnvironment.MaxConcurrentThreadsPerCPU);
    //        //nvs.Add("ApplicationPhysicalPath", HostingEnvironment.ApplicationPhysicalPath);
    //        //nvs.Add("ApplicationVirtualPath", HostingEnvironment.ApplicationVirtualPath);
    //        //nvs.Add("SiteName", HostingEnvironment.SiteName);
    //        //nvs.Add("Cache.Count", HostingEnvironment.Cache.Count);

    //        //if (isFromCommandLine)
    //        //{
    //        //    buf.AppendLine(nvs.ToStr());
    //        //    //buf.Append("<h3>reqData</h3>");
    //        //    //buf.Append(reqData.ToS());
    //        //}
    //        //else
    //        //{
    //        //    buf.AppendLine(nvs.ToHtml());
    //        //    buf.Append("<h3>reqData</h3>");
    //        //    buf.Append(reqData.ToHtml());
    //        //}          

    //        //return buf.ToString();

    //    }
    //    catch (Exception ex)
    //    {
    //        nvs.Add("Ex1", ex.ToString());
    //    }
    //    //return nvs;
    //    string rval = nvs.ToHtml();
    //    return rval;
    //}
    //===================================================================================================

}
