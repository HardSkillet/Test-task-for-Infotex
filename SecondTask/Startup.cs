using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecondTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddXmlFile("config.xml");
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ContextHtml>();
            services.AddControllersWithViews();
            services.Configure<ConfigRss>(Configuration);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Tape}/{id?}");
            });
        }
    }
}
