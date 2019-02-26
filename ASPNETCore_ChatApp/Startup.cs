using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ASPNETCore_ChatApp.Models;
using ASPNETCore_ChatApp.Hubs;
using ASPNETCore_ChatApp.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_ChatApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            /*services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
            {
                option.Cookie.Name = "aspnet_chatapp_authCookie";
                option.Cookie.Domain = "localhost";
                option.SlidingExpiration = true;
                option.Cookie.HttpOnly = true;
                option.Cookie.Path = "/chatHub";
                option.ExpireTimeSpan = new TimeSpan((new DateTime()).Millisecond + (1000 * 60 * 60));
                /*option.TicketDataFormat = ticketFormat;
                option.CookieManager = new CustomChunkingCookieManager();*/
            /*});*/
            
            services.AddSignalR();
            services.AddSession();
            services.AddSingleton<RequestHandler>();
            services.AddHttpContextAccessor();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlite("Data Source=users.sqlite",
            optionsBuilder => optionsBuilder.MigrationsAssembly("ASPNETCore_ChatApp")));
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders(); ;
            services.Add(new ServiceDescriptor(typeof(ChatDBContext), new ChatDBContext(Configuration.GetConnectionString("DefaultConnection"))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IdentityDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseAuthentication();

                app.UseDeveloperExceptionPage();

                dbContext.Database.Migrate(); //this will generate the db if it does not exist
            }
            else
            {
                app.UseExceptionHandler("/Chat/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            app.UseCookiePolicy();
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Chat}/{action=Index}/{id?}");
            });
        }
    }
}
