using Microsoft.Extensions.Configuration;
using Serilog;

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
        
        CreateSerilogLogger();

        Application.Run(new MainForm());
    }

    private static void CreateSerilogLogger()
    {
        var seqURL = Environment.GetEnvironmentVariable("SeqURL");
        var apiKey = Environment.GetEnvironmentVariable("SeqApiKeyTDAmeritradeSharp");
        IConfigurationRoot? configuration;
        if (string.IsNullOrEmpty(seqURL))
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:8081")
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
        }
        else
        {
            // Add Seq using environment variables, to keep them out of appsettings.json
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettingsWriteSeqFromEnvironmentVariables.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq(seqURL, apiKey: apiKey)
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
        }
    }
}