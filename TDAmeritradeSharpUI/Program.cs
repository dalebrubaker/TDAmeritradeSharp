using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TDAmeritradeSharpClient;

namespace TDAmeritradeSharpUI;

internal static class Program
{
    static Program()
    {
        UserSettingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(TDAmeritradeSharpUI));
        if (!Directory.Exists(UserSettingsDirectory))
        {
            Directory.CreateDirectory(UserSettingsDirectory);
        }
    }

    /// <summary>
    ///     The directory in %AppData% where private application settings are stored, NOT including appsettings.json
    /// </summary>
    internal static string UserSettingsDirectory { get; }

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
        var seqURL = Environment.GetEnvironmentVariable("SeqURL");
        var apiKey = Environment.GetEnvironmentVariable("SeqApiKeyTDAmeritradeSharp");
        if (string.IsNullOrEmpty(seqURL))
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
        }
        else
        {
            // Add Seq using environment variables, to keep them out of appsettings.json
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq(seqURL, apiKey:apiKey)
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
        }
        try
        {
            Log.Verbose("Application starting");
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to run");
        }
        finally
        {
            Log.Verbose("Exiting");
            Log.CloseAndFlush();
        }
    }
}