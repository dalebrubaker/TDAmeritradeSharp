using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace TDAmeritradeSharp
{
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
            DefaultBuilderStart();
        }

        private static void DefaultBuilderStart()
        {
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