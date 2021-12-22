/// Quickly adding a postgres db
/// 1) Import Microsoft.EntityFrameworkCore
/// 2) Import Npgsql.EntityFrameworkCore.PostgreSQL
/// 3) Uncomment lines in this file
/// 4) Uncomment lines in docker-compose.yml
/// 5) Add connection strings to secrets
/// 5) Implement db Context

global using Microsoft.Extensions.Logging;

using Serilog;
//using Microsoft.EntityFrameworkCore;

//--------------create builder--------------//
var builder = WebApplication.CreateBuilder(args);

ConfigureHost(builder.Host);
ConfigureServices(builder.Services, builder.Configuration);

//--------------create app--------------//
WebApplication app = builder.Build();

var logger = new Logger<Program>(app.Services.GetRequiredService<ILoggerFactory>());
Constants.ConfigureConstants(app);

// make sure db is up to date if we are running inside docker
if (Constants.InDocker)
{
    //await UpdateDb<FILL ME>(app.Services);
}

ConfigurePipeline(app);

//--------------run app--------------//
Run(app);

await app.RunAsync();


void ConfigureHost(ConfigureHostBuilder host)
{
    // https://github.com/serilog/serilog-aspnetcore
    // rest of config is in appsettings.json
    host.UseSerilog((context, services, configuration) => configuration
               .ReadFrom.Configuration(context.Configuration)
               .ReadFrom.Services(services)
               );
    host.ConfigureAppConfiguration((hostingContext, config) =>
    {
        //if we are inside a container load the secret
        // since the secret manager is only for development you can use this for secrets
        if (Constants.InDocker)
            // for some bizzare reason docker strips the file extension
            config.AddJsonFile("/run/secrets/server", false, false);
    });
}

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton(services);
    services.AddHttpClient();
}

void ConfigurePipeline(WebApplication app)
{

}

void Run(WebApplication app)
{
    
}

//async Task UpdateDb<T>(IServiceProvider services) where T : DbContext
//{
//    using var scope = services.CreateScope();
//    var logger = new Logger<Program>(services.GetRequiredService<ILoggerFactory>());
//    var db = scope.ServiceProvider.GetRequiredService<T>();
//
//    await Task.Delay(5000); // wait for db to start
//    while (true)
//    {
//        try
//        {
//            logger.LogInformation($"Migrating {typeof(T).Name}...");
//            await db.Database.MigrateAsync();
//            break;
//        }
//        catch (Exception e)
//        {
//            logger.LogError(e, "Migration failed! Db probably hasn't started yet. Retrying after 5 seconds...");
//            await Task.Delay(5000);
//        }
//    }
//}

//sometimes you just have to have static constants
public static class Constants
{
    public static WebApplication App { get; private set; } = null!;
    public static bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    public static bool IsDevelopment { get; private set; }
    
    public static void ConfigureConstants(WebApplication app)
    {
        App = app;
        IsDevelopment = app.Environment.IsDevelopment();
    }
}
