using Newtonsoft.Json;

namespace TDAmeritradeSharpUI;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private MainFormSettings Settings { get; set; } = new();

    private string SettingsPath => Path.Combine(Program.UserSettingsDirectory, $"{GetType().Name}.json");

    private void MainForm_Load(object sender, EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }
        LoadConfig();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }
        SaveConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            Settings = JsonConvert.DeserializeObject<MainFormSettings>(json) ?? new MainFormSettings();
            WindowPlacement.RestoreWindow(Handle, Settings.WindowPlacementJson);
        }
        mainFormSettingsBindingSource.DataSource = Settings;
    }

    private void SaveConfig()
    {
        Settings.WindowPlacementJson = WindowPlacement.SaveWindow(Handle);
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText(SettingsPath, json);
    }
}