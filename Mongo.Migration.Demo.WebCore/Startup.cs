using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Startup.DotNetCore;

namespace Mongo.Migration.Demo.WebCore
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            services.Configure<MongoMigrationSettings>(
                options =>
                {
                    
                    options.ConnectionString = _configuration.GetSection("MongoDb:ConnectionString").Value;
                    options.Database = _configuration.GetSection("MongoDb:Database").Value;
                });
            services.AddMigrationOnStartup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();


            app.Run(async context => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}