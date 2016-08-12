using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Idsrv4Sample.Contexts;
using Idsrv4Sample.Models;
using Idsrv4Sample.Configuration;
using Idsrv4Sample.Extensions;

namespace Idsrv4Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        private IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]));

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            var certificate = new X509Certificate2(Path.Combine(Environment.WebRootPath, @"Certificate\idsrv3test.pfx"), "idsrv3test");

            var builder = services
                .AddIdentityServer()
                .SetSigningCredential(certificate)
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get())
                .ConfigureAspNetIdentity<ApplicationUser>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddDebug(LogLevel.Trace);

            app.UseDeveloperExceptionPage();

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseStaticFiles();
        }
    }
}
