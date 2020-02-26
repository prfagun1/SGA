using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SGA.Interfaces;
using SGA.Lib;
using SGA.Models;
using SGA.Repositories;
using System.Threading;

namespace SGA
{
    public class Startup
    {
        private string connectionString;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<SGAContext>(options => options.UseMySQL(connectionString).EnableSensitiveDataLogging());
            services.AddDbContext<LogContext>(options => options.UseMySQL(connectionString));

            

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IAuthentication, Authentication>();
            services.AddTransient<IDataImport, DataImport>();
            services.AddTransient<IDataWrite, DataWrite>();
            services.AddTransient<IDataImportAD, DataImportAD>();
            services.AddTransient<IDataImportSQL, DataImportSQL>();
            services.AddTransient<IDataImportRest, DataImportRest>();
            services.AddTransient<IUserHelper, UserHelper>();


            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => { 
                options.LoginPath = "/Authentication/Login";
               options.AccessDeniedPath = "/Authentication/AccessDenied";
            });

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeFolder("/");
                options.Conventions.AllowAnonymousToPage("/Authentication/Login");
            });


            services.AddAuthorization(options => Lib.AuthenticationHelper.GetAuthorizationOptions(options));;

        }



        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //Inicia o processo que fará as importações e execuções automáticas
            ScheduledTask scheduled = new ScheduledTask(app.ApplicationServices);
            Thread scheduleTasksThread = new Thread(() => scheduled.Start());
            scheduleTasksThread.Start();

        }
    }


    }
