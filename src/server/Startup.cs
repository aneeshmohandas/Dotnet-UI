using System;
using System.Net;
using DotnetUI.Hubs;
using DotnetUI.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DotnetUI
{
    public class Startup
    {
        public static string uiVersion = "dotnetUi_1.1.0";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSpaStaticFiles(spa =>
           {
               spa.RootPath = "wwwroot";
           });

            services.AddControllers();
            services.AddSignalR();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotnetUI", Version = "v1" });
            });
            //services 
            services.AddScoped<IProjectManagerService, ProjectManagerService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotnetUI v1"));
            }
            app.UseCors("AllowSpecificOrigin");
            // app.UseHttpsRedirection();
            if (env.WebRootPath == null)
            {
                Console.WriteLine("WEB ROOT PATH EMPTY");
                System.IO.Directory.CreateDirectory(env.ContentRootPath + "/wwwroot");
                using (var client = new WebClient())
                {
                    Console.WriteLine("DOWNLOADING FILE");
                    client.DownloadFile($"https://github.com/aneeshmohandas/Dotnet-UI/raw/main/Release/{uiVersion}.zip", env.ContentRootPath + $"/{uiVersion}.zip");
                    Console.WriteLine("DOWNLOADING COMPLETED");
                    Console.WriteLine("EXTRACTING FILES");

                    System.IO.Compression.ZipFile.ExtractToDirectory(env.ContentRootPath + $"/{uiVersion}.zip", env.ContentRootPath + "/wwwroot");
                    Console.WriteLine("EXTRACTING FINISHED");
                }
                env.WebRootPath = env.ContentRootPath + "/wwwroot";

            }
            DotnetUiHelper.AppIntialSetup(env);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ApplicationHub>("/appHub");
            });
            app.UseSpaStaticFiles();

            app.UseSpa(configuration: builder =>
            {
                if (env.IsDevelopment())
                {
                    builder.UseProxyToSpaDevelopmentServer("http://localhost:8080");
                }
            });

        }
    }
}
