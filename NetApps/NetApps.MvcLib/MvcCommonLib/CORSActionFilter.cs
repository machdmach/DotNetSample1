using Microsoft.AspNetCore.Mvc.Filters;

namespace Libx.Mvc;
public class CORSActionFilter : ActionFilterAttribute
{
    //#cors
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Request.Method == "OPTIONS")
        {
            var Request = filterContext.HttpContext.Request;
            var Response = filterContext.HttpContext.Response;

            //string accessControl_AllowOrigin = null;
            //if (accessControl_AllowOrigin == null)
            //{
            //    accessControl_AllowOrigin = Request.Headers["Origin"]; //"http://localhost:8080";
            //}
            string accessControl_AllowOrigin = Request.Headers["Origin"]; //"http://localhost:8080";
            if (accessControl_AllowOrigin != null)
            {
                //https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
                //Access-Control-Allow-Methods: POST, PUT, GET, OPTIONS
                //Access-Control-Allow-Headers: Origin, X-Requested-With, Content-Type, Accept, Authorization
                Response.Headers["Access-Control-Allow-Headers"] = "X-App-Entry-URL, X-Pass, X-Page-Size";
                Response.Headers["Access-Control-Allow-Origin"] = accessControl_AllowOrigin; //#cors
                Response.Headers["Access-Control-Allow-Credentials"] = "true";
                Response.Headers["Vary"] = "Origin";
            }
#if docs
                <system.webServer>
                  <modules runAllManagedModulesForAllRequests="true" >
                  </modules>
                  <httpProtocol>
                    <customHeaders>
                      <add name="Access-Control-Allow-Origin" value="*" />
                    </customHeaders>
                  </httpProtocol>
                </system.webServer>
#endif

            // do nothing let IIS deal with reply!
            filterContext.Result = new EmptyResult();
        }
        else
        {
            base.OnActionExecuting(filterContext);
        }
    }
}
