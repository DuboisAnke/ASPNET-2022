using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Data.Default;
using PXLPRO2022Shoppers04.Services.Interfaces;
using PXLPRO2022Shoppers04.Services.Repositories;

namespace PXLPRO2022Shoppers04
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
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("connString")), ServiceLifetime.Scoped);
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = false;
            });
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId =
                    "934661097320-mko6ft2vonc1i2rkl333s6scu6s064vb.apps.googleusercontent.com";
                options.ClientSecret = "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";
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
            services.AddAuthentication().AddTwitter(options =>
            {
                options.ConsumerKey = "JOvi1hQfntjQJSddPza7bLscx";
                options.ConsumerSecret = "wiJ2QjyamfNvdy1hw3ZVgjJV065gMIMPZLiLK2ZU2uUjRh5VJJ";
            });
            services.AddAuthentication().AddDiscord(options =>
            {
                options.ClientId = "953287925561458749";
                options.ClientSecret = "k6En5pknsCUMuHpEkYDf8NlxkouHaAe2";
            });
            services.AddAuthentication().AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://localhost:9001";
                options.ClientId = "test.client.id";
                options.ClientSecret = "test.client.secret";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SaveTokens = true;
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.GetClaimsFromUserInfoEndpoint = true;
            });

            services.AddAuthentication()
                .AddFacebook(fbOpts =>
                {
                    fbOpts.AppId = "1816828625190151";
                    fbOpts.AppSecret = "b196f12e1009faa060cfcc4c9977da5c";
                });
            Stripe.StripeConfiguration.ApiKey =
                "sk_test_51Kgve1KqtguiaIDq1T5qPfdtgfbv8cDtgIIZsd4wa8GEqER4tyF4INJA3bti2CQawfDpXI3WffwpLVOFjsWZo1Iv00sFP2clVs";
            services.AddScoped<IAuthentication, AuthenticationRepository>();
            services.AddScoped<IProduct, ProductRepository>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddServerSideBlazor();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToController("/manage/{*path:nonfile}", "Index", "Blazor");
            });

            await SeedData.EnsurePopulatedAsync(app);
        }
    }
}