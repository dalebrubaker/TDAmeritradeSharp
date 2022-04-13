using System.Diagnostics;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TDAmeritradeSharpClient;

namespace TDAmeritradeSharpUI;

public partial class AuthUserControl : UserControl
{
    private string? _authCodeUrl;
    private Client _client = null!;

    private int _counter;
    private ILogger<AuthUserControl>? _logger;

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
            timer1.Enabled = true;
        }
    }

    private void buttonShowManualAuth_Click(object sender, EventArgs e)
    {
        logControl1.LogMessage("");
        logControl1.LogMessage(_counter++.ToString());
        logControl1.LogMessage("Below here is for manual authentication at developer.tdameritrade.com");
        logControl1.LogMessage("Browse to https://developer.tdameritrade.com/authentication/apis/post/token-0 and enter the following:");
        logControl1.LogMessage("grant_type\tauthorization_code");
        logControl1.LogMessage("access_type\toffline");
        var code = textBoxEncodedAuthCode.Text;
        var decoded = HttpUtility.HtmlDecode(code);
        var consumerKey = Settings.ConsumerKey;
        var callback = Settings.CallbackUrl;
        logControl1.LogMessage($"code\t\t{decoded}");
        var apiKey = $"{consumerKey}@AMER.OAUTHAP";
        logControl1.LogMessage($"client_id\t\t{apiKey}");
        logControl1.LogMessage($"redirect_uri\t{callback}");
        logControl1.LogMessage("");
        logControl1.LogMessage("For manual entry at developer.tdameritrade.com, e.g. https://developer.tdameritrade.com/quotes/apis/get/marketdata/%7Bsymbol%7D/quotes");
        logControl1.LogMessage($"apikey:\t\t{apiKey}");
        var decodedAccessToken = HttpUtility.HtmlDecode(_client.AuthResult.access_token);
        var bearerCode = $"Bearer<{decodedAccessToken}>";
        logControl1.LogMessage($"Authorization:\t{bearerCode}");
    }

    private async void timer1_Tick(object sender, EventArgs e)
    {
        timer1.Enabled = false;
        try
        {
            await _client.RequireNotExpiredTokensAsync();
        }
        catch (Exception ex)
        {
            // Show message and return without enabling timer. It will be reenabled on btnGetAuthCode_Click
            MessageBox.Show(ex.Message);
            return;
        }
        lblRequestsInLastMinute.Text = $"Requests in last minute: {_client.ThrottledThrottledRequestTimesUtc.Count}";
        if (_client.AuthResult.AccessTokenExpirationUtc.Date != DateTime.MinValue.Date)
        {
            var timeUntilAccessTokenExpires = _client.AuthResult.AccessTokenExpirationUtc - DateTime.UtcNow;
            if (timeUntilAccessTokenExpires.Ticks < 0)
            {
                timeUntilAccessTokenExpires = _client.AuthResult.AccessTokenExpirationUtc - DateTime.UtcNow;
            }
            lblAccessTokenExpires.Text = timeUntilAccessTokenExpires.Ticks < 0 
                ? "Access token is no longer valid." 
                : $"Access token expires in {timeUntilAccessTokenExpires.Pretty()}";
        }
        else
        {
            lblAccessTokenExpires.Text = "Access token is not valid yet.";
        }
        lblRefreshTokenExpires.Text = _client.AuthResult.RefreshTokenExpirationUtc.Date != DateTime.MinValue.Date 
            ? $"Refresh token expires {_client.AuthResult.RefreshTokenExpirationUtc.Date:d}" 
            : "Refresh token is expired or never was set. Initialize using buttons above.";
        timer1.Enabled = true;
    }
}