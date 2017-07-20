using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
    }

    public class Startup
    {
        //Uncomment this line and logging starts working again.
        //public void ConfigureServices(IServiceCollection services) => services.AddMvc();

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var container = new Container();
            container.Configure(config => config.Populate(services));
            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) => app.UseMvc();
    }

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            this.logger = logger;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //NO LOGGERS CONFIGURED
            logger.LogError("Charlie Charlie");
            return new string[] { "value1", "value2" };
        }
    }
}