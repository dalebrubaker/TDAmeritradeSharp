using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TDAmeritradeSharpClient;

namespace TDAmeritradeSharpUI;

public partial class AuthUserControl : UserControl
{
    private Client? _client;
    private ILogger<AuthUserControl>? _logger;
    private string? _authCodeUrl;
    private string? _decodedAuthCode;

    public AuthUserControl()
    {
        InitializeComponent();
    }

    private AuthUserControlSettings Settings { get; set; } = new();

    private string SettingsPath => Path.Combine(Program.UserSettingsDirectory, $"{GetType().Name}.json");

    private void AuthUserControl_Load(object sender, EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        // Here in Load we now have a ParentForm and can retrieve a logger from DI
        // ReSharper disable once AssignNullToNotNullAttribute
        var mainForm = (MainForm)ParentForm;
        _logger = mainForm.ServiceProvider.GetRequiredService<ILogger<AuthUserControl>>();
        _logger?.LogTrace("Loading AuthUserControl");

        LoadConfig();

        _client = mainForm.ServiceProvider.GetRequiredService<Client>();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }
        SaveConfig();
        base.OnHandleDestroyed(e);
    }

    private void LoadConfig()
    {
        if (File.Exists(SettingsPath))
        {
            var json = File.ReadAllText(SettingsPath);
            Settings = JsonConvert.DeserializeObject<AuthUserControlSettings>(json) ?? new AuthUserControlSettings();
        }
        authUserControlSettingsBindingSource.DataSource = Settings;
    }

    private void SaveConfig()
    {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText(SettingsPath, json);
    }

    private void textBoxConsumerKey_TextChanged(object sender, EventArgs e)
    {
        ResetUrlForEncodedAuthorizationCode();
    }

    private void textBoxCallbackUrl_TextChanged(object sender, EventArgs e)
    {
        ResetUrlForEncodedAuthorizationCode();
    }

    private void ResetUrlForEncodedAuthorizationCode()
    {
        _authCodeUrl = $"https://auth.tdameritrade.com/auth?response_type=code&&redirect_uri={textBoxCallbackUrl.Text}&&client_id={textBoxConsumerKey.Text}%40AMER.OAUTHAP";
    }

    private void buttonLogin_Click(object sender, EventArgs e)
    {
        logControl1.LogMessage($"Starting web page: {_authCodeUrl}");
        var psInfo = new ProcessStartInfo
        {
            FileName = _authCodeUrl, UseShellExecute = true
        };
        Process.Start(psInfo);
    }
    
    private void textBoxEncodedAuthCode_TextChanged(object sender, EventArgs e)
    {
        _decodedAuthCode = HttpUtility.UrlDecode(textBoxEncodedAuthCode.Text);
    }

    private async void btnGetAuthCode_Click(object sender, EventArgs e)
    {
        if (_client == null)
        {
            return;
        }
        logControl1.LogMessage($"Setting access tokens: {_authCodeUrl}");
        var code = textBoxEncodedAuthCode.Text;
        var consumerKey = Settings.ConsumerKey;
        var callback = Settings.CallbackUrl;
        var result = await _client.SetAuthResults(code, consumerKey, callback);
        logControl1.LogMessage(result);
        MessageBox.Show(result);
        if (result == Client.Success)
        {
            SaveConfig();
            var authResultJsonLines = await File.ReadAllLinesAsync(_client.PathAuthResult);
            logControl1.LogMessage($"The following json is saved at {_client.PathAuthResult}");
            logControl1.LogMessages(authResultJsonLines);
        }
    }

    private int _counter = 0;
    private void buttonShowManualAuth_Click(object sender, EventArgs e)
    {
        logControl1.LogMessage("");
        logControl1.LogMessage(_counter++.ToString());
        logControl1.LogMessage("Below here is for manual entries at developer.tdameritrade.com");
        logControl1.LogMessage("Browse to https://developer.tdameritrade.com/authentication/apis/post/token-0 and enter the following:");
        logControl1.LogMessage("grant_type\tauthorization_code");
        logControl1.LogMessage("access_type\toffline");
        var code = textBoxEncodedAuthCode.Text;
        var consumerKey = Settings.ConsumerKey;
        var callback = Settings.CallbackUrl;
        logControl1.LogMessage($"code\t\t{_decodedAuthCode}");
        logControl1.LogMessage($"client_id\t\t{consumerKey}@AMER.OAUTHAP");
        logControl1.LogMessage($"redirect_uri\t{callback}");
    }
}
