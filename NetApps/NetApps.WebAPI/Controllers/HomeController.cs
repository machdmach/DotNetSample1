using NetApps.WebAPI.Models;
using System.Diagnostics;

namespace Libx.Mvc.App;
public class HomeController : AppControllerBase
{
    public IActionResult Index()
    {
        Init();
        var buf = new StringBuilder();
        buf.AppendFormat("<br><br> <h2 style='text-align:center'> <a href='{0}'> Main Page </a> </h2>", MvcLib.AppMainPageUrl);
        return MyPassThroughView(buf);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

