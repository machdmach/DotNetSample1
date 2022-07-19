using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace NetApps.WebAPI
{
    public class Startup
    {
        //public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            //Configuration = configuration;
            MvcApp.ReloadConfigFiles();
            //DevLinks.InjectDependency(typeof(AppDevLinks)); //#todo
        }

        //===================================================================================================
        //string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            //services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.AddControllersWithViews();

            //Access to script at 'http://localhost:4141/NetApps/cs/WebUIx/NetApps/mainNetApps.js?ts=2022-06-27T09-47-34' from origin 'https://localhost:44321' has been blocked by CORS policy: 
            //            No 'Access-Control-Allow-Origin' header is present on the requested resource.

            //Response Headers:
            //Access-Control-Allow-Methods: PUT,DELETE,GET
            //Access-Control-Allow-Origin: https://cors1.azurewebsites.net 

            //MOutput.WriteLine("Cors service added to the container: " + MyAllowSpecificOrigins);
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins, policy =>
            //    {
            //        policy.WithOrigins("https://localhost:44321", //Access-Control-Allow-Origin: https://cors1.azurewebsites.net 
            //                           "http://localhost:4141",
            //                           "http://*.example.com");
            //        policy.AllowAnyHeader();
            //        //policy.WithHeaders(HeaderNames.ContentType, "x-custom-header"); //Access-Control-Allow-Headers: Content-Type,x-custom-header
            //        policy.AllowAnyMethod(); //Access-Control-Allow-Methods: PUT,DELETE,GET,OPTIONS
            //    });
            //});
        }

        //===================================================================================================
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //https://svap-tfs.ccdoa.net/DefaultCollection/DAS/_git/Intranet?path=%2FDecal%2FCode%2FDecal.Web%2FStartup.cs
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Site/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Site/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();

            //app.UseCors();
            //app.UseCors(MyAllowSpecificOrigins);

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/test123", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Home}/{action=Index}/{method?}/{id?}");
            });
        }
    }
}
