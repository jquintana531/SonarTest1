using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Security;
using System.Threading;

namespace MTC.WebApp.BackOffice
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            


            Configuration = builder.Build();

            var evironment = Configuration["ApplicationSettings:BaseUrlApi"];


        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<AppSettings>(Configuration.GetSection("ApplicationSettings"));
            services.Configure<ConfigurationSettings>(Configuration.GetSection("ConfigurationSettings"));
            services.AddAntiforgery(options => { 
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Expiration = TimeSpan.FromHours(9);
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews().AddNewtonsoftJson().AddSessionStateTempDataProvider();
            services.AddRazorPages().AddSessionStateTempDataProvider(); ; 
            //services.AddScoped<MTCAuthorizationHandler>();
            //services.AddCors();

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("https://mtcpos.com.mx", "https://www.mtcpos.com.mx");
            }));


            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                //options.IdleTimeout = TimeSpan.FromMinutes(2);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultureInfo = new CultureInfo("es-MX");
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("es-MX"),
                };

                options.DefaultRequestCulture = new RequestCulture("es-MX");
                // Formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            });

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


            services.AddGrpcClient<DBData.DBDataClient>(o =>
            {
                var scheduleProviderOptions = Configuration.GetSection("ApplicationSettings").Get<AppSettings>();
                o.Address = new Uri(scheduleProviderOptions.DBClient);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var httpClient = new HttpClient(httpClientHandler);

                return httpClientHandler;
            }).ConfigureChannel( channel => {

                channel.HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true,
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        // Leave certs unvalidated for debugging
                        RemoteCertificateValidationCallback = delegate { return true; }
                    }

            };

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                /*endpoints.MapControllerRoute(
                                             name: "default",
                                             pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();*/
            });


            app.Use( (context, next) =>
            {
                //cambiar cuando se pase a produ
                string path = context.Request.Path.Value;
                if (path != null && !path.ToLower().Contains("/api"))
                {
                    // XSRF-TOKEN used by angular in the $http if provided
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN",
                      tokens.RequestToken, new CookieOptions
                      {
                          HttpOnly = false,
                          Secure = false
                      }
                    );
                }
                
                //
                app.UseCors();
                // X-Content-Type-Options header
                app.UseXContentTypeOptions();
                // Referrer-Policy header.
                app.UseReferrerPolicy(opts => opts.NoReferrer());
                // X-Xss-Protection header
                app.UseXXssProtection(options => options.EnabledWithBlockMode());
                // X-Frame-Options header
                app.UseXfo(options => options.Deny());
                // Content-Security-Policy header
                app.UseCsp(opts => opts
                    .BlockAllMixedContent()
                    .StyleSources(s => s.Self())
                    .StyleSources(s => s.UnsafeInline())
                    .FontSources(s => s.Self())
                    .FormActions(s => s.Self())
                    .FrameAncestors(s => s.Self())
                    .ImageSources(s => s.Self())
                    .ScriptSources(s => s.Self())
                );
                return next();
            });





            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    spa.Options.StartupTimeout = new TimeSpan(0, 0, 200);
                }
            });


        }
    }
}
