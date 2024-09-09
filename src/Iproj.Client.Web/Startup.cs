using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using static System.Formats.Asn1.AsnWriter;
namespace Iproj.Client.Web.Client;

public class Startup
{
    public IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = "oidc";
            options.DefaultSignOutScheme = "Cookies";
        })
        .AddCookie("Cookies", options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax;  
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Do not enforce HTTPS
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = "https://auth.iproj.uz";
            options.ClientId = "oidcMVCApp";
            options.ClientSecret = "Wabase";

            options.ResponseType = "code";
            options.UsePkce = true;
            options.ResponseMode = "query";
            options.Scope.Add("message.read");
            options.Scope.Add("message.read");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("role");

            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = true;
            options.SaveTokens = true;

            options.CallbackPath = "/signin-oidc";
        });
        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }


        /*var fordwardedHeaderOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        fordwardedHeaderOptions.KnownNetworks.Clear();
        fordwardedHeaderOptions.KnownProxies.Clear();

        app.UseForwardedHeaders(fordwardedHeaderOptions);*/

        app.Use((context, next) =>
        {
            context.Request.Scheme = "https"; return next();
        });

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
        });
    }
}
