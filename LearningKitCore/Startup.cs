using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using CMS.Helpers;

using Kentico.Web.Mvc;
using Kentico.Membership;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Activities.Web.Mvc;
using Kentico.OnlineMarketing.Web.Mvc;

namespace LearningKitCore
{
    public class Startup
    {
        /// <summary>
        /// The application authentication cookie name
        /// </summary>
        private const string AUTHENTICATION_COOKIE_NAME = "identity.authentication";

        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }


        public Startup(IWebHostEnvironment environment,
                       IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var kenticoServiceCollection = services.AddKentico(features =>
            {
                features.UsePageBuilder();
                features.UsePageRouting();
                features.UseActivityTracking();
                features.UseABTesting();
                features.UseWebAnalytics();
            })
            .SetAdminCookiesSameSiteNone();

            if (Environment.IsDevelopment())
            {
                kenticoServiceCollection.DisableVirtualContextSecurityForLocalhost();
            }

            services.AddControllersWithViews();

            // Adds Xperience services required by the system's Identity implementation
            ConfigureApplicationIdentity(services);

            // Registers a filter for page templates of the LandigPage type
            ConfigurePageBuilderFilters();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseRewriter(new RewriteOptions()
                                .Add(new AdminRedirect(Configuration)));

            app.UseStaticFiles();

            app.UseKentico();

            app.UseCookiePolicy();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Kentico().MapRoutes();

                endpoints.MapControllerRoute(
                   name: "error",
                   pattern: "error/{code}",
                   defaults: new { controller = "HttpErrors", action = "Error" }
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ConfigurePageBuilderFilters()
        {
            PageBuilderFilters.PageTemplates.Add(new LandingPageTemplateFilter());
        }


        private void ConfigureApplicationIdentity(IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher<ApplicationUser>, Kentico.Membership.PasswordHasher<ApplicationUser>>();
            services.AddScoped<IMessageService, MessageService>();

            services.AddApplicationIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // A place to configure Identity options, such as password strength requirements
                options.SignIn.RequireConfirmedEmail = true;
            })
                    // Adds default token provider used to generate tokens for 
                    // email confirmations, password resets, etc.
                    .AddApplicationDefaultTokenProviders()
                    // Adds an implementation of the UserStore for working
                    // with Xperience user objects
                    .AddUserStore<ApplicationUserStore<ApplicationUser>>()
                    // Adds an implementation of the RoleStore used for 
                    // working with Xperience roles
                    .AddRoleStore<ApplicationRoleStore<ApplicationRole>>()
                    // Adds an implementation of the UserManager for Xperience membership
                    .AddUserManager<ApplicationUserManager<ApplicationUser>>()
                    // Adds the default implementation of the SignInManger
                    .AddSignInManager<SignInManager<ApplicationUser>>();

            services.AddAuthorization();
            //DocSection:ExternalAuth
            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "placeholder";
                    facebookOptions.AppSecret = "placeholder";
                })
                .AddMicrosoftAccount(microsoftAccountOptions =>
                {
                    microsoftAccountOptions.ClientSecret = "placeholder";
                    microsoftAccountOptions.ClientId = "placeholder";
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "placeholder";
                    googleOptions.ClientSecret = "placeholder";
                })
                .AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = "placeholder";
                    twitterOptions.ConsumerSecret = "placeholder";
                    twitterOptions.RetrieveUserDetails = true;
                });
            //EndDocSection:ExternalAuth

            services.ConfigureApplicationCookie(c =>
            {
                c.LoginPath = new PathString("/");
                c.ExpireTimeSpan = TimeSpan.FromDays(14);
                c.SlidingExpiration = true;
                c.Cookie.Name = AUTHENTICATION_COOKIE_NAME;
            });

            // Registers the authentication cookie with the 'Essential' cookie level
            // Ensures that the cookie is preserved when changing a visitor's allowed cookie level below 'Visitor'
            CookieHelper.RegisterCookie(AUTHENTICATION_COOKIE_NAME, CookieLevel.Essential);
        }
    }
}
