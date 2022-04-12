namespace TDAmeritradeSharpUI
{
    partial class AuthUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxConsumerKey = new System.Windows.Forms.TextBox();
            this.authUserControlSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBoxCallbackUrl = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.logControl1 = new TDAmeritradeSharpUI.LogControl();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxEncodedAuthCode = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnGetAuthCode = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonShowManualAuth = new System.Windows.Forms.Button();
            this.lblRequestsInLastMinute = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblAccessTokenExpires = new System.Windows.Forms.Label();
            this.lblRefreshTokenExpires = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.authUserControlSettingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(932, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Authentication data is stored privately in your Users directory under TDAmeritrad" +
    "eSharpClient, e.g. C:\\Users\\dale\\AppData\\Roaming\\TDAmeritradeSharpClient\\AuthRes" +
    "ult.json ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(600, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Prerequisite: Register at developer.tdameritrade.com and add an App using https:/" +
    "/127.0.0.1 for the Callback URL";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(875, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Prerequisite: In Windows Settings under Turn Windows features on or off, turn on " +
    " Internet Information Services and Internet Information Services Hostable Web Co" +
    "re";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Consumer Key:";
            // 
            // textBoxConsumerKey
            // 
            this.textBoxConsumerKey.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.authUserControlSettingsBindingSource, "ConsumerKey", true));
            this.textBoxConsumerKey.Location = new System.Drawing.Point(105, 90);
            this.textBoxConsumerKey.Name = "textBoxConsumerKey";
            this.textBoxConsumerKey.Size = new System.Drawing.Size(269, 23);
            this.textBoxConsumerKey.TabIndex = 5;
            this.textBoxConsumerKey.TextChanged += new System.EventHandler(this.textBoxConsumerKey_TextChanged);
            // 
            // authUserControlSettingsBindingSource
            // 
            this.authUserControlSettingsBindingSource.DataSource = typeof(TDAmeritradeSharpUI.AuthUserControlSettings);
            // 
            // textBoxCallbackUrl
            // 
            this.textBoxCallbackUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.authUserControlSettingsBindingSource, "CallbackUrl", true));
            this.textBoxCallbackUrl.Location = new System.Drawing.Point(105, 62);
            this.textBoxCallbackUrl.Name = "textBoxCallbackUrl";
            this.textBoxCallbackUrl.Size = new System.Drawing.Size(142, 23);
            this.textBoxCallbackUrl.TabIndex = 7;
            this.textBoxCallbackUrl.Text = "https://127.0.0.1";
            this.textBoxCallbackUrl.TextChanged += new System.EventHandler(this.textBoxCallbackUrl_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 66);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Callback URL:";
            // 
            // logControl1
            // 
            this.logControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logControl1.HideTimestamps = false;
            this.logControl1.Location = new System.Drawing.Point(0, 316);
            this.logControl1.MaximumLogLengthChars = 104857600;
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(1051, 89);
            this.logControl1.TabIndex = 8;
            this.logControl1.Title = "Log";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(12, 119);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(231, 23);
            this.buttonLogin.TabIndex = 11;
            this.buttonLogin.Text = "Login (will fail, but this is to get a code)";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // textBoxEncodedAuthCode
            // 
            this.textBoxEncodedAuthCode.Location = new System.Drawing.Point(286, 148);
            this.textBoxEncodedAuthCode.Name = "textBoxEncodedAuthCode";
            this.textBoxEncodedAuthCode.Size = new System.Drawing.Size(676, 23);
            this.textBoxEncodedAuthCode.TabIndex = 13;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(968, 151);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 15);
            this.label15.TabIndex = 30;
            this.label15.Text = "...";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 151);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(268, 15);
            this.label17.TabIndex = 32;
            this.label17.Text = "Encoded Auth Code (Paste url after \'code=\' here):";
            // 
            // btnGetAuthCode
            // 
            this.btnGetAuthCode.Location = new System.Drawing.Point(13, 178);
            this.btnGetAuthCode.Name = "btnGetAuthCode";
            this.btnGetAuthCode.Size = new System.Drawing.Size(234, 23);
            this.btnGetAuthCode.TabIndex = 33;
            this.btnGetAuthCode.Text = "Get Auth Code";
            this.btnGetAuthCode.UseVisualStyleBackColor = true;
            this.btnGetAuthCode.Click += new System.EventHandler(this.btnGetAuthCode_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(13, 151);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(268, 15);
            this.label18.TabIndex = 32;
            this.label18.Text = "Encoded Auth Code (Paste url after \'code=\' here):";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 178);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(268, 23);
            this.button1.TabIndex = 33;
            this.button1.Text = "Get Auth Code";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnGetAuthCode_Click);
            // 
            // buttonShowManualAuth
            // 
            this.buttonShowManualAuth.Location = new System.Drawing.Point(286, 178);
            this.buttonShowManualAuth.Name = "buttonShowManualAuth";
            this.buttonShowManualAuth.Size = new System.Drawing.Size(268, 23);
            this.buttonShowManualAuth.TabIndex = 35;
            this.buttonShowManualAuth.Text = "Show Manual Auth";
            this.buttonShowManualAuth.UseVisualStyleBackColor = true;
            this.buttonShowManualAuth.Click += new System.EventHandler(this.buttonShowManualAuth_Click);
            // 
            // lblRequestsInLastMinute
            // 
            this.lblRequestsInLastMinute.AutoSize = true;
            this.lblRequestsInLastMinute.Location = new System.Drawing.Point(14, 245);
            this.lblRequestsInLastMinute.Name = "lblRequestsInLastMinute";
            this.lblRequestsInLastMinute.Size = new System.Drawing.Size(132, 15);
            this.lblRequestsInLastMinute.TabIndex = 36;
            this.lblRequestsInLastMinute.Text = "Requests in last minute:";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblAccessTokenExpires
            // 
            this.lblAccessTokenExpires.AutoSize = true;
            this.lblAccessTokenExpires.Location = new System.Drawing.Point(187, 245);
            this.lblAccessTokenExpires.Name = "lblAccessTokenExpires";
            this.lblAccessTokenExpires.Size = new System.Drawing.Size(132, 15);
            this.lblAccessTokenExpires.TabIndex = 37;
            this.lblAccessTokenExpires.Text = "Access token expires in:";
            // 
            // lblRefreshTokenExpires
            // 
            this.lblRefreshTokenExpires.AutoSize = true;
            this.lblRefreshTokenExpires.Location = new System.Drawing.Point(14, 220);
            this.lblRefreshTokenExpires.Name = "lblRefreshTokenExpires";
            this.lblRefreshTokenExpires.Size = new System.Drawing.Size(122, 15);
            this.lblRefreshTokenExpires.TabIndex = 38;
            this.lblRefreshTokenExpires.Text = "Refresh token expires:";
            // 
            // AuthUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblRefreshTokenExpires);
            this.Controls.Add(this.lblAccessTokenExpires);
            this.Controls.Add(this.lblRequestsInLastMinute);
            this.Controls.Add(this.buttonShowManualAuth);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGetAuthCode);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.textBoxEncodedAuthCode);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.logControl1);
            this.Controls.Add(this.textBoxCallbackUrl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxConsumerKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "AuthUserControl";
            this.Size = new System.Drawing.Size(1051, 405);
            this.Load += new System.EventHandler(this.AuthUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.authUserControlSettingsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label label2;
        private Label label1;
        private Label label3;
        private Label label4;
        private TextBox textBoxConsumerKey;
        private TextBox textBoxCallbackUrl;
        private Label label5;
        private LogControl logControl1;
        private BindingSource authUserControlSettingsBindingSource;
        private Button buttonLogin;
        private FolderBrowserDialog folderBrowserDialog1;
        private TextBox textBoxEncodedAuthCode;
        private Label label15;
        private Label label17;
        private Button btnGetAuthCode;
        private Label label18;
        private Button button1;
        private Button buttonShowManualAuth;
        private Label lblRequestsInLastMinute;
        private System.Windows.Forms.Timer timer1;
        private Label lblAccessTokenExpires;
        private Label lblRefreshTokenExpires;
    }
}
