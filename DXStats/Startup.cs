using Discord;
using Discord.WebSocket;
using DxStats.Services;
using DXStats.Configuration;
using DXStats.Interfaces;
using DXStats.Persistence;
using DXStats.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tweetinvi;

namespace DXStats
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddScoped<IDxDataRepository, DxDataRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Override env variable DATABASE_DIR to specify directory
            services.AddDbContext<DxStatsDbContext>(opt =>
                opt.UseSqlite($"Data Source={Configuration["DATABASE_DIR"]}/dxstats.db"));

            var apiEndpoints = new ApiEndpoints();
            Configuration.GetSection("ApiEndpoints").Bind(apiEndpoints);

            services.Configure<DiscordCredentials>(options =>
                Configuration.GetSection("Discord").Bind(options));

            services.AddHttpClient<IBlocknetApiService, BlocknetApiService>(client =>
            {
                client.BaseAddress = new Uri(apiEndpoints.Blocknet);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient<IDxDataService, DxDataService>(client =>
            {
                client.BaseAddress = new Uri(apiEndpoints.Native);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            var twitterCredentials = Configuration.GetSection("Twitter").Get<TwitterCredentials>();

            Auth.SetUserCredentials(
                twitterCredentials.ConsumerKey,
                twitterCredentials.ConsumerSecret,
                twitterCredentials.UserAccessToken,
                twitterCredentials.UserAccessSecret
            );

            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }));

            services.AddSingleton<DiscordStartupService>();

            services.AddScoped<IComposeTweetService, ComposeTweetService>();

            services.AddCors(corsOptions =>
            {
                corsOptions.AddPolicy("fully permissive", configurePolicy => configurePolicy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
                //.WithOrigins("http://localhost:44305")

            });


            //services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            IServiceScopeFactory _scopeFactory = app.ApplicationServices.GetService(typeof(IServiceScopeFactory)) as IServiceScopeFactory;

            Task.Run(() => app.ApplicationServices.GetRequiredService<DiscordStartupService>().StartAsync());

            //app.UseHttpsRedirection();

            app.UseCors("fully permissive");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

    }
}
