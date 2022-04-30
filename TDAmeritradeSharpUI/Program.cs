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
        var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<AuthUserControl>()
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
    }
}