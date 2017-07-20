using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
    }

    public class Startup
    {
        //Uncomment below and logging starts working again.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddMvc();
        //    services.AddTransient<IAmAInterface, Implementation>();
        //}

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var container = new Container();
            container.Configure(config =>
            {
                config.For<IAmAInterface>().Use<Implementation>();
                config.Populate(services);
            });
            return container.GetInstance<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) => app.UseMvc();
    }

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> logger;
        private readonly IAmAInterface dependency;

        public ValuesController(ILogger<ValuesController> logger, IAmAInterface dependency)
        {
            this.logger = logger;
            this.dependency = dependency;
        }
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var something = await dependency.Something();
            //NO LOGGERS CONFIGURED
            logger.LogError(something);
            return new string[] { "value1", "value2" };
        }
    }

    public interface IAmAInterface
    {
        Task<string> Something();
    }

    public class Implementation : IAmAInterface
    {
        public async Task<string> Something()
        {
            await Task.CompletedTask;
            return "Charlie";
        }
    }
}