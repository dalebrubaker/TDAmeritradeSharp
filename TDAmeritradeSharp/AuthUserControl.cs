﻿using System.Diagnostics;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TDAmeritradeSharp
{
    public partial class AuthUserControl : UserControl
    {
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
                Settings = JsonSerializer.Deserialize<AuthUserControlSettings>(json) ?? new AuthUserControlSettings();
            }
            authUserControlSettingsBindingSource.DataSource = Settings;
        }

        private void SaveConfig()
        {
            var json = JsonSerializer.Serialize(Settings);
            File.WriteAllText(SettingsPath, json);
        }

        private void textBoxConsumerKey_TextChanged(object sender, EventArgs e)
        {
            ResetUrlForEncodedAuthorizationCode();
            textBoxClientId.Text = textBoxConsumerKey.Text + "@AMER.OAUTHAP";
        }

        private void textBoxCallbackUrl_TextChanged(object sender, EventArgs e)
        {
            ResetUrlForEncodedAuthorizationCode();
            textBoxRedirectUri.Text = textBoxCallbackUrl.Text;
        }

        private void ResetUrlForEncodedAuthorizationCode()
        {
            textBoxAuthUrl.Text = $"https://auth.tdameritrade.com/auth?response_type=code&&redirect_uri={textBoxCallbackUrl.Text}&&client_id={textBoxConsumerKey.Text}%40AMER.OAUTHAP";
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            var psInfo = new ProcessStartInfo {
                FileName = textBoxAuthUrl.Text, UseShellExecute = true
            };
            Process.Start(psInfo);
        }

        private void textBoxEncodedAuthCode_TextChanged(object sender, EventArgs e)
        {
            textBoxDecodedAuthCode.Text = HttpUtility.UrlDecode(textBoxEncodedAuthCode.Text);
        }

        private void buttonRequestAuthCode_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var url = $"https://api.tdameritrade.com/v1/oauth2/token";
        }
    }
}