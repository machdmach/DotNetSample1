using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NetApps.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder_x(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder_x(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
