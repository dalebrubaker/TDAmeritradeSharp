using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TDAmeritradeSharp
{
    /// <summary>
    /// Good video: 
    /// For Seq do this command in PowerShell: https://youtu.be/_iryZxv8Rxw C# Logging with Serilog and Seq - Structured Logging Made Easy by IAmTimCorey
    ///     docker run -d  --restart unless-stopped --name seq -e ACCEPT_EULA=Y -v D:\Logs\TDAmeritradeSharp:/data -p 8081:80 datalust/seq:latest
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
            try
            {
                Log.Information("Application starting");
                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((hostContext, services) => { services.AddScoped<MainForm>(); })
                    .UseSerilog()
                    .Build();
                using var serviceScope = host.Services.CreateScope();
                var services = serviceScope.ServiceProvider;
                var form1 = services.GetRequiredService<MainForm>();
                Application.Run(form1);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to run");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}