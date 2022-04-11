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
            this.logControl1 = new LogControl();
            this.textBoxAuthUrl = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxEncodedAuthCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxDecodedAuthCode = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxClientId = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxRedirectUri = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnGetAuthCode = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.authUserControlSettingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(782, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Authentication data is stored privately in your Users directory under TDAmeritrad" +
    "eSharp, e.g. C:\\Users\\dale\\AppData\\Roaming\\TDAmeritradeSharp ";
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
            this.authUserControlSettingsBindingSource.DataSource = typeof(AuthUserControlSettings);
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
            this.logControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logControl1.HideTimestamps = false;
            this.logControl1.Location = new System.Drawing.Point(0, 449);
            this.logControl1.MaximumLogLengthChars = 104857600;
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(1051, 123);
            this.logControl1.TabIndex = 8;
            this.logControl1.Title = "Log";
            // 
            // textBoxAuthUrl
            // 
            this.textBoxAuthUrl.Location = new System.Drawing.Point(13, 233);
            this.textBoxAuthUrl.Name = "textBoxAuthUrl";
            this.textBoxAuthUrl.ReadOnly = true;
            this.textBoxAuthUrl.Size = new System.Drawing.Size(268, 23);
            this.textBoxAuthUrl.TabIndex = 10;
            this.textBoxAuthUrl.Text = "UrlForEncodedAuthCode";
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
            this.textBoxEncodedAuthCode.TextChanged += new System.EventHandler(this.textBoxEncodedAuthCode_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 255);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(563, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Browse to https://developer.tdameritrade.com/authentication/apis/post/token-0 and" +
    " enter the following:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 279);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 15);
            this.label9.TabIndex = 18;
            this.label9.Text = "grant_type:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(84, 276);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(124, 23);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "authorization_code";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(84, 304);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(124, 23);
            this.textBox2.TabIndex = 21;
            this.textBox2.Text = "offline";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 307);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 15);
            this.label10.TabIndex = 20;
            this.label10.Text = "access_type:";
            // 
            // textBoxDecodedAuthCode
            // 
            this.textBoxDecodedAuthCode.Location = new System.Drawing.Point(84, 333);
            this.textBoxDecodedAuthCode.Multiline = true;
            this.textBoxDecodedAuthCode.Name = "textBoxDecodedAuthCode";
            this.textBoxDecodedAuthCode.ReadOnly = true;
            this.textBoxDecodedAuthCode.Size = new System.Drawing.Size(782, 23);
            this.textBoxDecodedAuthCode.TabIndex = 23;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 336);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "code:";
            // 
            // textBoxClientId
            // 
            this.textBoxClientId.Location = new System.Drawing.Point(85, 362);
            this.textBoxClientId.Multiline = true;
            this.textBoxClientId.Name = "textBoxClientId";
            this.textBoxClientId.ReadOnly = true;
            this.textBoxClientId.Size = new System.Drawing.Size(335, 23);
            this.textBoxClientId.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 365);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 15);
            this.label12.TabIndex = 24;
            this.label12.Text = "client_id:";
            // 
            // textBoxRedirectUri
            // 
            this.textBoxRedirectUri.Location = new System.Drawing.Point(84, 391);
            this.textBoxRedirectUri.Multiline = true;
            this.textBoxRedirectUri.Name = "textBoxRedirectUri";
            this.textBoxRedirectUri.ReadOnly = true;
            this.textBoxRedirectUri.Size = new System.Drawing.Size(124, 23);
            this.textBoxRedirectUri.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 394);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 15);
            this.label7.TabIndex = 26;
            this.label7.Text = "redirect_uri:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(875, 337);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(16, 15);
            this.label13.TabIndex = 28;
            this.label13.Text = "...";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(990, 232);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(16, 15);
            this.label14.TabIndex = 29;
            this.label14.Text = "...";
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
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(897, 337);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(122, 15);
            this.label16.TabIndex = 31;
            this.label16.Text = "(Decoded Auth Code)";
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 215);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(341, 15);
            this.label6.TabIndex = 34;
            this.label6.Text = "Below here is for manual entries at developer.tdameritrade.com";
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
            // AuthUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGetAuthCode);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxRedirectUri);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxClientId);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxDecodedAuthCode);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxEncodedAuthCode);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxAuthUrl);
            this.Controls.Add(this.logControl1);
            this.Controls.Add(this.textBoxCallbackUrl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxConsumerKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "AuthUserControl";
            this.Size = new System.Drawing.Size(1051, 589);
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
        private TextBox textBoxAuthUrl;
        private Button buttonLogin;
        private FolderBrowserDialog folderBrowserDialog1;
        private TextBox textBoxEncodedAuthCode;
        private Label label8;
        private Label label9;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label10;
        private TextBox textBoxDecodedAuthCode;
        private Label label11;
        private TextBox textBoxClientId;
        private Label label12;
        private TextBox textBoxRedirectUri;
        private Label label7;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Button btnGetAuthCode;
        private Label label6;
        private Label label18;
        private Button button1;
    }
}
