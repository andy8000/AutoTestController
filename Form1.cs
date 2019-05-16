using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;


namespace AutoTestController.Client
{
    public delegate string MethodCaller(string result);
    public partial class Form1 : Form
    {
        public event MethodCaller ReadOutput;
        List<string> fileList = new List<string>();
        List<string> selectedTestCasesList = new List<string>();
        StringBuilder sb = new StringBuilder();
        string rootPath = string.Empty;
        string logPath = string.Empty;
        int testCaseIndex = 0;
        Process iniProc = null;
        bool isStart = false;
        int failCount = 0;
        string failureCountStr = string.Empty;
        Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
        string runTime = string.Empty;
        string testCasesLabel = string.Empty;
        bool isTestCaseBegin = false;
        Dictionary<int, bool> testResult = new Dictionary<int, bool>();
        public Form1()
        {
            //切换语言
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            Init();
        }
        private void RunTestCase_Click(object sender, EventArgs e)
        {
            InitBeforeRunTestCases();
            //ResetTestPlanListBox();
            progressBar1.ForeColor = Color.LimeGreen;
            iniProc = new Process();            
            if (selectedTestCasesList.Count == 0)
            {
                MessageBox.Show("请选择测试用例生成测试计划！","错误",MessageBoxButtons.OK,MessageBoxIcon.Error,MessageBoxDefaultButton.Button2);
                return;
            }
            StopBtn.Enabled = true;
            button1.Enabled = false;
            myStopwatch.Start();
            //初始化
            StatusLabel.BackColor = Color.Green;
            StatusLabel.Text = ">>>测试用例执行中<<<";     
            progressBar1.Value = 0;
            progressBar1.Maximum = selectedTestCasesList.Count;
            string testCase = selectedTestCasesList[testCaseIndex];            
            string fileName = GetExecuteFile(testCase + ".bat");
            curTestCase.Text = fileName;
            timer1.Enabled = true;
            ExecuteTestCase(fileName, iniProc);
            Thread.Sleep(3000);
        }

        private void InitBeforeRunTestCases()
        {
            ExeResultTBX.Clear();
            Output.Clear();
            testResult.Clear();
            myStopwatch.Reset();
            RefreshTestResultIcon.Enabled = true;
            Helper.indexRecord = 0;
        }
        private string GetExecuteFile(string givenValue)
        {
            string retValue = string.Empty;
            foreach (string str in fileList)
            {
                if (str.Contains(givenValue))
                {
                    retValue = str;
                    break;
                }
            }
            return retValue;
        }
        private void ExecuteTestCase(string file,Process proc)
        {
            string output = string.Empty;
            string fileName = file;
            isTestCaseBegin = true;
            try
            {
                //Show test case contents
                ParseLuaTestCases(GetExecuteFile(selectedTestCasesList[testCaseIndex] + ".lua"));
                testCaseIndex++;
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.WorkingDirectory = fileName.Substring(0,fileName.LastIndexOf('\\'));
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                proc.EnableRaisingEvents = true;
                proc.Start();
                if (!isStart) ReadOutput += new MethodCaller(ReadStdOutputAction);
                proc.BeginOutputReadLine();
            }
            catch (Exception)
            {
                Teardown();
            }
        }
        private void OutputHandler(Object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                if (!string.IsNullOrEmpty(outLine.Data))
                {
                    this.Invoke(ReadOutput, new object[] { outLine.Data });
                }
            }
            catch (Exception)
            {
                //ToDo:
            }
        }
        private void Init()
        {
            failureCountStr = TestCaseFailCount.Text;
            testCasesLabel = TestCaseNumber.Text;
            runTime = RunTime.Text;
            StatusLabel.BackColor = Color.Yellow;
            StatusLabel.Text = ">>>空闲中<<<";
            // 解决打开软件即生成树的需求, 2017-7-24            
            try
            {
                if (!File.Exists(Environment.CurrentDirectory + "\\config.txt")) return;
                GetArgumentsFromConfig(Environment.CurrentDirectory + "\\config.txt");
            }
            catch
            {
                return;
            }
            GenerateTreeView();
            //加载模版
            templateComBox.Items.Add("Default");
            GenerateTemplate();
        }

        private void GenerateTemplate()
        {
            if (!Directory.Exists(rootPath + "\\..\\Template")) return;
            string[] files = Directory.GetFiles(rootPath + "\\..\\Template");
            foreach (string str in files)
            {
                FileInfo fi = new FileInfo(str);
                templateComBox.Items.Add(fi.Name);
            }
        }
        private int GenerateTreeView()
        {
            //防止重复创建树
            fileList.Clear();
            TestProj.Nodes.Clear();
            TreeNode root = new TreeNode();
            root.Text = "测试用例";
            TestProj.Nodes.Add(root);
            try
            {
                string[] folders = Directory.GetDirectories(rootPath);
                CreateNode(folders, root);
            }
            catch (Exception)
            {
                //ToDo:
            }
            TestCaseNumber.Text = testCasesLabel + allTestCaseNum;
            return allTestCaseNum;
        }
        int allTestCaseNum = 0;
        private void CreateNode(string []folders, TreeNode root)
        {            
            for (int i = 0; i < folders.Length; i++)
            {
                TreeNode module = new TreeNode();
                module.Text = folders[i].Substring(folders[i].LastIndexOf('\\') + 1);
                root.Nodes.Add(module);
                string[] files = Directory.GetFiles(folders[i]);
                for (int j = 0; j < files.Length; j++)
                {
                    fileList.Add(files[j]);
                    FileInfo fi = new FileInfo(files[j]);
                    if (fi.Extension.ToLower().Contains(".lua")) continue;
                    TreeNode testCase = new TreeNode();
                    testCase.Text = fi.Name.Substring(0, fi.Name.IndexOf('.'));
                    module.Nodes.Add(testCase);
                    allTestCaseNum++;
                }
                //递归调用
                string[] subFolders = Directory.GetDirectories(folders[i]);
                if (subFolders.Length != 0) CreateNode(subFolders, module);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool isSuccess = false;
            RunTime.Text = runTime + myStopwatch.Elapsed;
            //RefreshTestPlanList();
            try
            {
                if (testCaseIndex >= selectedTestCasesList.Count && iniProc.HasExited)
                {
                    this.StatusLabel.BackColor = Color.Yellow;
                    this.StatusLabel.Text = ">>>空闲中<<<";
                    ExeResultTBX.Text = Helper.HandleOutputStr(Output.Text, out isSuccess);
                    //string retResult = Helper.HandleOutputStr(Output.Text, out isSuccess);
                    //if(retResult.Contains("测试脚本名称"))ExeResultTBX.SelectionColor = Color.YellowGreen;
                    //ExeResultTBX.AppendText(retResult);
                    //Logging
                    Helper.WriteToTestLog(logPath,ExeResultTBX.Text);
                    if (!isSuccess)
                    {
                        progressBar1.ForeColor = Color.Red;
                        failCount++;
                    }
                    Helper.HandleTestResult(isSuccess, testCaseIndex, ref testResult);
                    RefreshTestResultIcon.Enabled = true;//Refresh test result icon
                    TestCaseFailCount.Text = failureCountStr + failCount;
                    this.progressBar1.Value = testCaseIndex;
                    timer1.Enabled = false;
                    button1.Enabled = true;
                    Teardown();
                    return;
                }
                if (iniProc.HasExited)
                {
                    isStart = true;
                    timer1.Interval += 2000;
                    Thread.Sleep(2000);
                    ExeResultTBX.Text = Helper.HandleOutputStr(Output.Text, out isSuccess);
                    timer1.Interval -= 2000;
                    //Increased after finished
                    if (!isSuccess)
                    {
                        progressBar1.ForeColor = Color.Red;
                        failCount++;
                        TestCaseFailCount.Text = failureCountStr + failCount;
                    }
                    Helper.HandleTestResult(isSuccess, testCaseIndex, ref testResult);
                    RefreshTestResultIcon.Enabled = true;//Refresh test result icon
                    progressBar1.Value = testCaseIndex;
                    iniProc.CancelOutputRead();
                    iniProc.Dispose();
                    iniProc = new Process();
                    string testCase = GetExecuteFile(selectedTestCasesList[testCaseIndex] + ".bat");
                    //Show test case name
                    curTestCase.Text = testCase;
                    ExecuteTestCase(testCase, iniProc);
                    StatusLabel.BackColor = Color.Green;
                    StatusLabel.Text = ">>>测试用例执行中<<<";
                }
            }
            catch (System.InvalidOperationException ex)
            {
                Teardown();
                MessageBox.Show(ex.Message);
            }
        }
        private void TestProj_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                CheckAllNodes(e.Node,e.Node.Checked);
            }
        }
        private void CheckAllNodes(TreeNode tn, bool isChecked)
        {
            foreach (TreeNode subTN in tn.Nodes)
            {
                subTN.Checked = isChecked;
                if (subTN.Nodes.Count != 0) CheckAllNodes(subTN, isChecked);
            }
            
        }
        private void GenerateTestCasePlanListMenu_Click(object sender, EventArgs e)
        {
            //TestPlanList.Items.Clear();
            TestPlanListView.Clear();
            selectedTestCasesList.Clear();
            testCaseIndex = 0;
            int runCounter = 0;
            foreach (string testCase in GetAllCheckedNodes())
            {
                runCounter++;
                //TestPlanList.Items.Add("[" + runCounter + "] " + testCase);
                GenerateTestCasePlanListView(runCounter, testCase);
                selectedTestCasesList.Add(testCase);
            }
            RunTestCase.Text = "｜测试运行数：" + runCounter;
            testPlan.SelectTab("TestCaseListTab");
        }

        private List<string> GetAllCheckedNodes()
        {
            List<string> nodeList = new List<string>();
            foreach (TreeNode subTN in TestProj.Nodes[0].Nodes)
            {
                if (subTN.Nodes.Count != 0)
                {
                    foreach (TreeNode tn in subTN.Nodes)
                    {
                        if (tn.Nodes.Count != 0)
                        {
                            foreach (TreeNode suTn in tn.Nodes)
                            {
                                if (suTn.Checked == true && suTn.Nodes.Count == 0 && (suTn.Text.ToLower().Contains("test_") || suTn.Text.ToLower().Contains("bug_"))) nodeList.Add(suTn.Text);
                            }
                        }
                        else
                        {
                            if (tn.Checked == true && tn.Nodes.Count == 0 && (tn.Text.ToLower().Contains("test_") || tn.Text.ToLower().Contains("bug_"))) nodeList.Add(tn.Text);
                        }
                    }
                }
            }
            return nodeList;

        }
        private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestProj.ExpandAll();
        }
        private void TestPlanList_DoubleClick(object sender, EventArgs e)
        {
            tabControl2.SelectTab(0);
            TestSteps.Clear();
            foreach(string file in fileList)
            {
                string fileName = TestPlanList.SelectedItem.ToString().Substring(TestPlanList.SelectedItem.ToString().IndexOf(' '))+".lua";
                if (file.ToLower().Contains(fileName.Trim().ToLower()))
                {
                    ParseLuaTestCases(file);
                }
            }
        }

        private void ParseLuaTestCases(string file)
        {
            TestSteps.Clear();
            string[] lines = File.ReadAllLines(file, Encoding.Default);
            foreach (string str in lines)
            {
                if (str.Contains("--"))
                {
                    TestSteps.SelectionColor = Color.Green;
                }
                else if (str.Contains("TestCaseBegin") || str.Contains("TestCaseEnd"))
                {
                    TestSteps.SelectionColor = Color.Blue;
                }
                else
                {
                    TestSteps.SelectionColor = Color.Black;
                }
                TestSteps.AppendText(str);
                TestSteps.AppendText(Environment.NewLine);
            }
        }
        private string ReadStdOutputAction(string result)
        {
            if (isTestCaseBegin)
            {
                this.Output.AppendText("~测试脚本名称：" + selectedTestCasesList[testCaseIndex - 1] + ".lua~");
                isTestCaseBegin = false;
            }
            //只显示有用信息,要对result作处理
            this.Output.AppendText(Environment.NewLine);
            if (result.Contains("Fail") || result.Contains("Exception") || result.Contains("Error"))
            {
                Output.SelectionColor = Color.Red;
            }
            else if (result.Contains("Pass"))
            {
                Output.SelectionColor = Color.Green;
            }
            else if (result.Contains("[Test Case"))
            {
                Output.SelectionColor = Color.Blue;
            }
            else if (result.Contains("Check Point"))
            {
                Output.SelectionColor = Color.Black;
            }
            else
            {
                Output.SelectionColor = Color.DimGray;
            }
            this.Output.AppendText(result);
            sb.AppendLine(result);
            return result;
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Helper.KillProcess("NcStudio");
                Helper.KillProcess("EX31Simulator");
                Helper.KillProcess("LambdaSimulator");
                iniProc.Kill();               
                Teardown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            button1.Enabled = true;
            StopBtn.Enabled = false;
            //Teardown();
        }

        private void Teardown()
        {
            iniProc.Dispose();
            testCaseIndex = 0;
            failCount = 0;
            myStopwatch.Stop();
            timer1.Enabled = false;
            isStart = false;
            ReadOutput -= new MethodCaller(ReadStdOutputAction);
            this.StatusLabel.BackColor = Color.Yellow;
            this.StatusLabel.Text = ">>>空闲中<<<";

        }

        private void EcapsulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestProj.CollapseAll();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Version: 2.3" + Environment.NewLine + "Owner: chch2005@sohu.com" + Environment.NewLine + "Support: Phoenix Test Team";
            MessageBox.Show(message, "About", MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
        }

        private void AboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("To Be Continue...", "Helper", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3, MessageBoxOptions.ServiceNotification);
                Process chmProc = new Process();
                chmProc.StartInfo.FileName = Environment.CurrentDirectory + "\\TRunnerManual.chm";
                chmProc.Start();
            }
            catch
            {
                return;
            }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenToolStripMenuItem.Enabled = false;
            allTestCaseNum = 0;
            new Configure().ShowDialog();
            if (!File.Exists(Environment.CurrentDirectory + "\\config.txt")) return;
            GetArgumentsFromConfig(Environment.CurrentDirectory + "\\config.txt");
            if (!Directory.Exists(rootPath)) return;
            GenerateTreeView();
            OpenToolStripMenuItem.Enabled = true;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewToolStripMenuItem.Enabled = false;
            allTestCaseNum = 0;
            ConfigOpenFile.InitialDirectory = Environment.CurrentDirectory;
            if (ConfigOpenFile.ShowDialog() == DialogResult.OK)
            {
                GetArgumentsFromConfig(ConfigOpenFile.FileName);
            }
            GenerateTreeView();
            testPlan.SelectTab("TestCaseTreeTab");
            NewToolStripMenuItem.Enabled = true;
        }

        private bool GetArgumentsFromConfig(string fileName)
        {
            string [] paths = File.ReadAllLines(fileName);
            if (paths.Length != 2) return false;
            else
            {
                rootPath = paths[0];
                logPath = paths[1];
                return true;
            }
        }

        private void TestPlanList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush myBrush = Brushes.Black;
                /*if (e.Index < resultStatus.Count)
                {
                    if (resultStatus[e.Index] == true)
                    {
                        myBrush = Brushes.Green;
                    }
                    else
                    {
                        myBrush = Brushes.Red;
                    }
                }*/
                e.DrawFocusRectangle();
                e.Graphics.DrawString(TestPlanList.Items[e.Index].ToString(), e.Font, myBrush, e.Bounds);
            }
        }

        private void RefreshTestPlanList()
        {
            TestPlanList.Items.Clear();
            foreach (string item in selectedTestCasesList)
            {
                TestPlanList.Items.Add(item);
            }
        }

        private void ResetTestPlanListBox()
        {
            TestPlanList.DrawMode = DrawMode.OwnerDrawFixed;
        }

        private void GenerateTestCasePlanListView(int count, string testCase)
        {
            TestPlanListView.SmallImageList = icoList;
            //创建列
            if (count == 1)
            {
                TestPlanListView.Clear();
                this.TestPlanListView.Columns.Add("", 30, HorizontalAlignment.Center);
                this.TestPlanListView.Columns.Add("ID", 50, HorizontalAlignment.Center);
                this.TestPlanListView.Columns.Add("Test Case", 350,HorizontalAlignment.Left);                
            }
            TestPlanListView.BeginUpdate();
            ListViewItem lvi = new ListViewItem();
            lvi.ImageIndex = 0;
            lvi.SubItems.Add("["+count.ToString()+"]");
            lvi.SubItems.Add(testCase);
            TestPlanListView.Items.Add(lvi);
            TestPlanListView.EndUpdate();
        }

        private void TestPlanListView_DbClick(object sender, EventArgs e)
        {
            tabControl2.SelectTab(0);
            TestSteps.Clear();
            foreach (string file in fileList)
            {
                string fileName = TestPlanListView.SelectedItems[0].SubItems[2].Text + ".lua";
                if (file.ToLower().Contains(fileName.Trim().ToLower()))
                {
                    ParseLuaTestCases(file);
                }
            }
        }

        private void RefreshTestResultIcon_Tick(object sender, EventArgs e)
        {
            if (testResult.Count == 0) return;

            for (int i = 1; i <= testResult.Count; i++)
            {
                if (testResult[i] == true) TestPlanListView.Items[i-1].ImageIndex = 2;
                else TestPlanListView.Items[i-1].ImageIndex = 1;
                
            }
            RefreshTestResultIcon.Enabled = false;
        }

        private void GenerateAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TestPlanListView.SelectedItems.Count == 0) return;
            //清除已经有的测试用例列表
            selectedTestCasesList.Clear();
            foreach (ListViewItem item in TestPlanListView.SelectedItems)
            {
                selectedTestCasesList.Add(item.SubItems[2].Text);
            }
            TestPlanListView.Clear();
            testCaseIndex = 0;
            int runCounter = 0;
            foreach (string testCase in selectedTestCasesList)
            {
                runCounter++;
                GenerateTestCasePlanListView(runCounter, testCase);
            }
            RunTestCase.Text = "｜测试运行数：" + runCounter;
        }

        private void DetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectTab("TestResultDetails");
            if (TestPlanListView.SelectedItems.Count != 1) return;

            detailsRichTBX.Clear();
            ParseSingleTestResult(TestPlanListView.SelectedItems[0].SubItems[2].Text,ExeResultTBX.Text);
        }

        private void ParseSingleTestResult(string testCaseName,string str)
        {
            try
            {
                tabControl2.SelectedTab.Text = testCaseName;
                if (string.IsNullOrEmpty(str)) return;
                string[] contents = str.Split('\n');
                List<string> singleContent = new List<string>();
                bool isFlag = false;
                int pos1 = -1;
                int pos2 = contents.Length - 1;
                for (int i = 0; i < contents.Length; i++)
                {
                    if (isFlag && contents[i].Contains(".lua") && !contents[i].ToLower().Contains("details")) { pos2 = i; break; }
                    
                    if (!contents[i].Contains(testCaseName))
                    {
                        continue;
                    }
                    //脚本名称有包含关系的情况
                    if (string.Compare(contents[i].Substring(contents[i].IndexOf("Test"), contents[i].Length - contents[i].IndexOf("Test") - 4), testCaseName, true)!=0)
                    {
                        continue;
                    }
                    pos1 = i;
                    isFlag = true;
                }
                if (pos1 == -1) return;//默认第一个测试脚本名称不再0行，循环结束还没有找到测试用例名称对应的行则返回
                for (int i = pos1; i < pos2; i++)
                {
                    singleContent.Add(contents[i]);
                }                
                foreach (string line in singleContent)
                {

                    if (line.ToLower().Contains(".lua"))
                    {
                        detailsRichTBX.SelectionColor = Color.Blue;
                    }
                    else if (line.ToLower().Contains("pass"))
                    {
                        detailsRichTBX.SelectionColor = Color.Green;
                    }
                    else if (line.ToLower().Contains("error") || line.ToLower().Contains("fail"))
                    {
                        detailsRichTBX.SelectionColor = Color.Red;
                    }
                    else if (line.ToLower().Contains("check"))
                    {
                        detailsRichTBX.SelectionColor = Color.BlueViolet;
                    }
                    else
                    {
                        detailsRichTBX.SelectionColor = Color.Black;
                    }
                    //在详细结果里添加load和abort行过滤，解决Framework中打印冗余信息的问题。2017/12/6 by Andy
                    if (string.IsNullOrEmpty(line) || line.ToLower().Contains("load") || line.ToLower().Contains("abort")) continue;
                    detailsRichTBX.AppendText(line);
                    detailsRichTBX.AppendText(Environment.NewLine);
                }               
            }
            catch
            {
                return;
            }
        }
        private void ShowLineNo()
        {
            Point p = this.detailsRichTBX.Location;
            int crntFirstIndex = this.detailsRichTBX.GetCharIndexFromPosition(p);
            int crntFirstLine = this.detailsRichTBX.GetLineFromCharIndex(crntFirstIndex);
            Point crntFirstPos = this.detailsRichTBX.GetPositionFromCharIndex(crntFirstIndex);
            p.Y += this.detailsRichTBX.Height;
            int crntLastIndex = this.detailsRichTBX.GetCharIndexFromPosition(p);
            int crntLastLine = this.detailsRichTBX.GetLineFromCharIndex(crntLastIndex);
            Point crntLastPos = this.detailsRichTBX.GetPositionFromCharIndex(crntLastIndex);
            Graphics g = splitContainer2.Panel1.CreateGraphics();
            Font font = new Font(this.detailsRichTBX.Font, this.detailsRichTBX.Font.Style);
            SolidBrush brush = new SolidBrush(Color.Black);
            Rectangle rect = splitContainer2.Panel1.ClientRectangle;
            brush.Color = splitContainer2.Panel1.BackColor;
            g.FillRectangle(brush, 0, 0, splitContainer2.Panel1.ClientRectangle.Width, splitContainer2.Panel1.ClientRectangle.Height);
            brush.Color = Color.White;
            int lineSpace = 0;
            if (crntFirstLine != crntLastLine)
            {
                lineSpace = (crntLastPos.Y - crntFirstPos.Y) / (crntLastLine - crntFirstLine);
            }
            else
            {
                lineSpace = Convert.ToInt32(this.detailsRichTBX.Font.Size);
            }
            int brushX = splitContainer2.Panel1.ClientRectangle.Width - Convert.ToInt32(font.Size * 3);
            int brushY = crntLastPos.Y + Convert.ToInt32(font.Size * 0.21f);
            for (int i = crntLastLine; i >= crntFirstLine - 2; i--)
            {
                g.DrawString((i + 1).ToString(), font, brush, brushX, brushY);
                brushY -= lineSpace;
            }
            g.Dispose();
            font.Dispose();
            brush.Dispose();
        }

        private void detailsRichTBX_TextChanged(object sender, EventArgs e)
        {
            ShowLineNo();
        }

        private void detailsRichTBX_VScroll(object sender, EventArgs e)
        {
            ShowLineNo();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowLineNo();
            ShowTestStepsLineNo();
        }

        private void SetPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("该操作会修改Name_Mapping.lua数据，请慎重选择！", "一键修改端口寻址", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;
            StringBuilder sb = Helper.SetPort(rootPath);
            string message;
            if (sb == null)
            {
                message = "无法处理数据，请检查数据格式！";
                MessageBox.Show(message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                message = sb.ToString();
                MessageBox.Show(message, "修改端口寻址", MessageBoxButtons.OK);
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            ReplaceNameMapping();
        }
        private void ReplaceNameMapping()
        {
            string selectedT = templateComBox.SelectedItem.ToString();
            if (selectedT == "Default") return;
            DialogResult dr = MessageBox.Show("该操作会替换Name_Mapping.lua数据，请慎重选择！", "寻址替换", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
            {                
                return;
            }
            Helper.ReplaceFile(rootPath, selectedT);
            MessageBox.Show(selectedT + " 成功替换Name_Mapping.lua原数据！", "提示", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void ShowTestStepsLineNo()
        {
            Point p = this.TestSteps.Location;
            int crntFirstIndex = this.TestSteps.GetCharIndexFromPosition(p);
            int crntFirstLine = this.TestSteps.GetLineFromCharIndex(crntFirstIndex);
            Point crntFirstPos = this.TestSteps.GetPositionFromCharIndex(crntFirstIndex);
            p.Y += this.TestSteps.Height;
            int crntLastIndex = this.TestSteps.GetCharIndexFromPosition(p);
            int crntLastLine = this.TestSteps.GetLineFromCharIndex(crntLastIndex);
            Point crntLastPos = this.TestSteps.GetPositionFromCharIndex(crntLastIndex);
            Graphics g = splitContainer3.Panel1.CreateGraphics();
            Font font = new Font(this.TestSteps.Font, this.TestSteps.Font.Style);
            SolidBrush brush = new SolidBrush(Color.Black);
            Rectangle rect = splitContainer3.Panel1.ClientRectangle;
            brush.Color = splitContainer3.Panel1.BackColor;
            g.FillRectangle(brush, 0, 0, splitContainer3.Panel1.ClientRectangle.Width, splitContainer3.Panel1.ClientRectangle.Height);
            brush.Color = Color.White;
            int lineSpace = 0;
            if (crntFirstLine != crntLastLine)
            {
                lineSpace = (crntLastPos.Y - crntFirstPos.Y) / (crntLastLine - crntFirstLine);
            }
            else
            {
                lineSpace = Convert.ToInt32(this.TestSteps.Font.Size);
            }
            int brushX = splitContainer3.Panel1.ClientRectangle.Width - Convert.ToInt32(font.Size * 3);
            int brushY = crntLastPos.Y + Convert.ToInt32(font.Size * 0.21f);
            for (int i = crntLastLine; i >= crntFirstLine - 2; i--)
            {
                g.DrawString((i + 1).ToString(), font, brush, brushX, brushY);
                brushY -= lineSpace;
            }
            g.Dispose();
            font.Dispose();
            brush.Dispose();
        }

        private void TestSteps_VScroll(object sender, EventArgs e)
        {
            ShowTestStepsLineNo();
        }

        private void TestSteps_TextChanged(object sender, EventArgs e)
        {
            ShowTestStepsLineNo();
        }

        private void TestSteps_SizeChanged(object sender, EventArgs e)
        {
            ShowTestStepsLineNo();
        }

        private void detailsRichTBX_SizeChanged(object sender, EventArgs e)
        {
            ShowLineNo();
        }
    }
}
