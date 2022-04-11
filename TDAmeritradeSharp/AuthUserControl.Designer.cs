namespace TDAmeritradeSharp
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
            this.logControl1 = new TDAmeritradeSharp.LogControl();
            this.textBoxUrlTmp = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
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
            this.label4.Size = new System.Drawing.Size(163, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Consumer Key (aka client_id):";
            // 
            // textBoxConsumerKey
            // 
            this.textBoxConsumerKey.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.authUserControlSettingsBindingSource, "ConsumerKey", true));
            this.textBoxConsumerKey.Location = new System.Drawing.Point(181, 91);
            this.textBoxConsumerKey.Name = "textBoxConsumerKey";
            this.textBoxConsumerKey.Size = new System.Drawing.Size(269, 23);
            this.textBoxConsumerKey.TabIndex = 5;
            this.textBoxConsumerKey.TextChanged += new System.EventHandler(this.textBoxConsumerKey_TextChanged);
            // 
            // authUserControlSettingsBindingSource
            // 
            this.authUserControlSettingsBindingSource.DataSource = typeof(TDAmeritradeSharp.AuthUserControlSettings);
            // 
            // textBoxCallbackUrl
            // 
            this.textBoxCallbackUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.authUserControlSettingsBindingSource, "CallbackUrl", true));
            this.textBoxCallbackUrl.Location = new System.Drawing.Point(181, 63);
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
            this.label5.Size = new System.Drawing.Size(170, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Callback URL (aka redirect_uri):";
            // 
            // logControl1
            // 
            this.logControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logControl1.HideTimestamps = false;
            this.logControl1.Location = new System.Drawing.Point(0, 347);
            this.logControl1.MaximumLogLengthChars = 104857600;
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(983, 225);
            this.logControl1.TabIndex = 8;
            this.logControl1.Title = "Log";
            // 
            // textBoxUrlTmp
            // 
            this.textBoxUrlTmp.Location = new System.Drawing.Point(12, 129);
            this.textBoxUrlTmp.Name = "textBoxUrlTmp";
            this.textBoxUrlTmp.ReadOnly = true;
            this.textBoxUrlTmp.Size = new System.Drawing.Size(950, 23);
            this.textBoxUrlTmp.TabIndex = 10;
            this.textBoxUrlTmp.Text = "UrlForEncodedAuthorizationCode";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(12, 158);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(231, 23);
            this.buttonLogin.TabIndex = 11;
            this.buttonLogin.Text = "Login (will fail, but this is to get a code)";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // AuthUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxUrlTmp);
            this.Controls.Add(this.logControl1);
            this.Controls.Add(this.textBoxCallbackUrl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxConsumerKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "AuthUserControl";
            this.Size = new System.Drawing.Size(983, 572);
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
        private TextBox textBoxUrlTmp;
        private Button buttonLogin;
    }
}
