using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace AutoTestController.Client
{
    public partial class Configure : Form
    {
        public Configure()
        {
            InitializeComponent();
            Init();           
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            List<string> lines = new List<string>();
            if (!Directory.Exists(ConfigTBX.Text))
            {
                ConfigTBX.Text = "路径不存在或不符合格式，请重新输入！";
                ConfigTBX.ForeColor = Color.Red;
                ConfigTBX.BackColor = Color.White;
                return;
            }            
            lines.Add(ConfigTBX.Text);
            lines.Add(LogTBX.Text);
            File.WriteAllLines(Environment.CurrentDirectory + "\\config.txt",lines);
            this.Hide();
            MessageBox.Show("保存成功", "Save", MessageBoxButtons.OK,MessageBoxIcon.Information);
            this.Close();
        }

        private void ConfigTBX_MouseClick(object sender, MouseEventArgs e)
        {
            ConfigTBX.BackColor = Color.White;
            ConfigTBX.Clear();
            ConfigTBX.ResetText();
        }

        private void LogTBX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LogTBX.BackColor = Color.White;
            LogTBX.Clear();
            LogTBX.ResetText();
        }

        private void LogFolderBrowserDialog_Click(object sender, EventArgs e)
        {
            string folderPath = @"D:\";
            logPathDialog.Description = "查找日志文件夹";
            logPathDialog.SelectedPath = folderPath;
            logPathDialog.ShowNewFolderButton = true;
            if (logPathDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = logPathDialog.SelectedPath;
            }
            LogTBX.Text = folderPath;
        }

        private void ProjFolderBrowserDialog_Click_1(object sender, EventArgs e)
        {
            string folderPath = @"C:\Program Files\Weihong\NcStudio\Test\TestScripts";
            projPathDialog.Description = "浏览TestScripts文件夹";
            projPathDialog.SelectedPath = folderPath;
            projPathDialog.ShowNewFolderButton = true;
            if (projPathDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = projPathDialog.SelectedPath;
            }
            ConfigTBX.Text = folderPath;
        }

        private bool GetPathsFromConfig(ref string rootPath, ref string logPath)
        {
            bool retValue = false;
            if (!File.Exists(Environment.CurrentDirectory + "\\config.txt")) return false;
            string[] paths = File.ReadAllLines(Environment.CurrentDirectory + "\\config.txt");
            if (paths.Length == 2)
            {
                rootPath = paths[0];
                logPath = paths[1];
                retValue = true;
            }
            return retValue;
        }
        private void Init()
        {
            string rootPath = string.Empty;
            string logPath = string.Empty;
            if (GetPathsFromConfig(ref rootPath,ref logPath))
            {
                ConfigTBX.Text = rootPath;
                LogTBX.Text = logPath;
            }
            toolTip1.SetToolTip(ProjFolderBrowserDialog, "查找项目路径");
            toolTip1.SetToolTip(LogFolderBrowserDialog, "查找日志路径");
        }
    }
}
