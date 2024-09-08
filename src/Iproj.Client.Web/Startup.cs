﻿using Iproj.Client.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
            options.Cookie.SameSite = SameSiteMode.Lax;  // Allows cookies on HTTP
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;  // Do not enforce HTTPS
        })
        .AddOpenIdConnect("oidc", options =>
        {
            //options.Authority = "http://192.168.0.30:8080";
            options.Authority = "http://45.130.148.192:8080";
            options.ClientId = "oidcMVCApp";
            options.ClientSecret = "Wabase";

            options.ResponseType = "code";
            options.UsePkce = true;
            options.ResponseMode = "query";
            options.Scope.Add("weatherApi.read");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("role");

            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = false;
            options.SaveTokens = true;

            options.CallbackPath = "/signin-oidc";
        });
        services.AddControllersWithViews();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            //app.UseHsts();
        }

        app.UseMiddleware<IprojMiddleware>();

        //app.UseHttpsRedirection();
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
