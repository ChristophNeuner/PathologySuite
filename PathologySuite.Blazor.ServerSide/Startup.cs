using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PathologySuite.Blazor.ServerSide.Areas.Identity;
using PathologySuite.Blazor.ServerSide.Data;
using PathologySuite.Shared.Core.Interfaces;
using PathologySuite.Shared.Core;
using PathologySuite.Shared.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;
using RabbitMQ;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace PathologySuite.Blazor.ServerSide
{
    public class Startup
    {
        private readonly PathOptions _pathOptions;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _pathOptions = new PathOptions(wsiBasePath: $@"{env.WebRootPath}/histo", wsiBaseFolderName: $@"histo", wsiBaseUri: new System.Uri($@"localhost:5000/"));

            // create necessary rabbitMQ exchanges
            //TODO: specify them at a central place and use a seperate script for creation
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "PathologySuite.AI", type: "topic");

                //Fair dispatch
                channel.BasicQos(0, 1, false);
            }

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            //services.AddServerSideBlazor(/*options => options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(120)*/);
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 10240000000; //about 10GB
            }).AddCircuitOptions(options =>
            {
                options.DetailedErrors = true;
            });

            services.AddScoped<IWsiProcessor, WsiProcessorNetVips>();
            services.AddScoped<IDziReader, DziReaderNetVips>();
            services.AddSingleton<PathOptions>(_pathOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles(); // For the wwwroot folder

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(_pathOptions.WsiBasePath), 
            //    RequestPath = "/histo"
            //});

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
