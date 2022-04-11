namespace TDAmeritradeSharp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.authUserControl1 = new TDAmeritradeSharp.AuthUserControl();
            this.mainFormSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mainFormSettingsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // authUserControl1
            // 
            this.authUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authUserControl1.Location = new System.Drawing.Point(0, 0);
            this.authUserControl1.Name = "authUserControl1";
            this.authUserControl1.Size = new System.Drawing.Size(1008, 481);
            this.authUserControl1.TabIndex = 0;
            // 
            // mainFormSettingsBindingSource
            // 
            this.mainFormSettingsBindingSource.DataSource = typeof(TDAmeritradeSharp.MainFormSettings);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 481);
            this.Controls.Add(this.authUserControl1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainFormSettingsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AuthUserControl authUserControl1;
        private BindingSource mainFormSettingsBindingSource;
    }
}