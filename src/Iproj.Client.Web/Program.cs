using Iproj.Client.Web.Client;

var builder = WebApplication.CreateBuilder(args);

/*builder.WebHost.UseKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});*/

// Add services to the container.
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

//app.InitAccessor();
var env = app.Environment;
startup.Configure(app, env);
var scope = app.Services.CreateScope();

app.Run();
