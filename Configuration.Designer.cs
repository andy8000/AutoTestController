namespace AutoTestController.Client
{
    partial class Configure
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this.label1 = new System.Windows.Forms.Label();
            this.ConfigTBX = new System.Windows.Forms.TextBox();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.LogTBX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.projPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ProjFolderBrowserDialog = new System.Windows.Forms.Button();
            this.LogFolderBrowserDialog = new System.Windows.Forms.Button();
            this.logPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // ConfigTBX
            // 
            resources.ApplyResources(this.ConfigTBX, "ConfigTBX");
            this.ConfigTBX.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ConfigTBX.Name = "ConfigTBX";
            this.toolTip1.SetToolTip(this.ConfigTBX, resources.GetString("ConfigTBX.ToolTip"));
            this.ConfigTBX.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ConfigTBX_MouseClick);
            // 
            // SaveBtn
            // 
            resources.ApplyResources(this.SaveBtn, "SaveBtn");
            this.SaveBtn.Name = "SaveBtn";
            this.toolTip1.SetToolTip(this.SaveBtn, resources.GetString("SaveBtn.ToolTip"));
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // LogTBX
            // 
            resources.ApplyResources(this.LogTBX, "LogTBX");
            this.LogTBX.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LogTBX.Name = "LogTBX";
            this.toolTip1.SetToolTip(this.LogTBX, resources.GetString("LogTBX.ToolTip"));
            this.LogTBX.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LogTBX_MouseDoubleClick);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // projPathDialog
            // 
            resources.ApplyResources(this.projPathDialog, "projPathDialog");
            this.projPathDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // ProjFolderBrowserDialog
            // 
            resources.ApplyResources(this.ProjFolderBrowserDialog, "ProjFolderBrowserDialog");
            this.ProjFolderBrowserDialog.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ProjFolderBrowserDialog.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.ProjFolderBrowserDialog.FlatAppearance.BorderSize = 0;
            this.ProjFolderBrowserDialog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.ProjFolderBrowserDialog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.ProjFolderBrowserDialog.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ProjFolderBrowserDialog.Name = "ProjFolderBrowserDialog";
            this.toolTip1.SetToolTip(this.ProjFolderBrowserDialog, resources.GetString("ProjFolderBrowserDialog.ToolTip"));
            this.ProjFolderBrowserDialog.UseVisualStyleBackColor = false;
            this.ProjFolderBrowserDialog.Click += new System.EventHandler(this.ProjFolderBrowserDialog_Click_1);
            // 
            // LogFolderBrowserDialog
            // 
            resources.ApplyResources(this.LogFolderBrowserDialog, "LogFolderBrowserDialog");
            this.LogFolderBrowserDialog.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.LogFolderBrowserDialog.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.LogFolderBrowserDialog.FlatAppearance.BorderSize = 0;
            this.LogFolderBrowserDialog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LogFolderBrowserDialog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LogFolderBrowserDialog.Name = "LogFolderBrowserDialog";
            this.toolTip1.SetToolTip(this.LogFolderBrowserDialog, resources.GetString("LogFolderBrowserDialog.ToolTip"));
            this.LogFolderBrowserDialog.UseVisualStyleBackColor = false;
            this.LogFolderBrowserDialog.Click += new System.EventHandler(this.LogFolderBrowserDialog_Click);
            // 
            // logPathDialog
            // 
            resources.ApplyResources(this.logPathDialog, "logPathDialog");
            // 
            // toolTip1
            // 
            this.toolTip1.BackColor = System.Drawing.Color.ForestGreen;
            this.toolTip1.IsBalloon = true;
            // 
            // Configure
            // 
            this.AcceptButton = this.SaveBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LogFolderBrowserDialog);
            this.Controls.Add(this.ProjFolderBrowserDialog);
            this.Controls.Add(this.LogTBX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.ConfigTBX);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Configure";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ConfigTBX;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.TextBox LogTBX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog projPathDialog;
        private System.Windows.Forms.Button ProjFolderBrowserDialog;
        private System.Windows.Forms.Button LogFolderBrowserDialog;
        private System.Windows.Forms.FolderBrowserDialog logPathDialog;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}