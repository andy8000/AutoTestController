using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace AutoTestController.Client
{
    class Helper
    {
        public static int indexRecord = 0;
        public static string HandleOutputStr(string inputStr,out bool isSuccess)
        {
            isSuccess = false;
            string retValue = string.Empty;
            StringBuilder sb = new StringBuilder();
            string []outputs = inputStr.Split('~');
            foreach (string str in outputs)
            {
                if(!str.Contains("Lambda")&&!str.Contains("Worker")&&!str.Contains("Halt"))
                sb.AppendLine(str);
            }
            retValue = sb.ToString();
            //isSuccess = GetLastStatus(outputs);
            isSuccess = GetTestResultStatus(outputs);
            return retValue;
        }
        public static string HandleLuaFile(string path)
        {
            string[] lines = File.ReadAllLines(path,Encoding.Default);
            StringBuilder sb = new StringBuilder();
            foreach (string str in lines)
            {
                sb.AppendLine(str);
            }
            return sb.ToString();
        }
        //判断测试用成功还是失败,由GetTestResultStatus替代
        private static bool GetLastStatus(string[] sb)
        {
            Stack<string> myStack = new Stack<string>();
            for (int startIndex = 0; startIndex < sb.Length; startIndex++)
            {
                if (sb[startIndex].ToString().ToLower().Contains("fail") || sb[startIndex].ToString().ToLower().Contains("pass"))
                {
                    myStack.Push(sb[startIndex].ToString().ToLower());
                }
            }
            if (myStack.Count != 0)
            {
                if (myStack.Pop().Contains("pass")) return true;
            }
            else
            {
                return false;
            }
            return false;
        }
        //通过关键字[Test Pass]/[Test Fail]得出结果，Added by Andy - 2017/7/28
        private static bool GetTestResultStatus(string[] sb)
        {
            bool retValue = false;
            try
            {
                for (int startIndex = indexRecord; startIndex < sb.Length; startIndex++)
                {
                    if (sb[startIndex].ToString().ToLower().Contains("[test pass]"))
                    {
                        retValue = true;
                        indexRecord = startIndex + 1;
                        break;
                    }
                }
            }
            catch
            { }
            return retValue;
        }
        public static void KillProcess(string processName)
        {
            Process[] curProcesses = Process.GetProcessesByName(processName);
            foreach (Process item in curProcesses)
            {
                if (string.Compare(processName, item.ProcessName, true) == 0) item.Kill();
            }
        }

        public static void HandleTestResult(bool isSuccess,int index, ref Dictionary<int, bool> resultDic)
        {
            if (isSuccess)
            {
                if (!resultDic.ContainsKey(index))
                {
                    resultDic.Add(index, true);
                }
            }
            else
            {
                if (!resultDic.ContainsKey(index))
                {
                    resultDic.Add(index, false);
                }
            }
        }

        public static StringBuilder SetPort(string path)
        {
            StringBuilder updateLog = new StringBuilder();
            string file = path + "\\..\\Test_Core_Lib\\Name_Mapping.lua";
            if (!File.Exists(file)) return null;
            string[] lines = File.ReadAllLines(file,Encoding.Default);
            int startIndex = 0;
            string str = string.Empty;
            string portPath = string.Empty;
            StringBuilder sb = new StringBuilder();
            string resultLog = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Begin_Set_Port_Mapping"))
                {
                    startIndex = i;
                    str = lines[i];
                    portPath = str.Substring(str.IndexOf('(') + 1, str.Length - str.IndexOf('(') - 2);
                    sb.AppendLine(lines[i]);
                    i++;
                    do
                    {
                        string newStr = ReplaceStr(lines[i], path + "\\..\\" + portPath,out resultLog);
                        if (string.IsNullOrEmpty(newStr))
                        {
                            sb.AppendLine(lines[i]);
                        }
                        else
                        {
                            sb.AppendLine(newStr);
                            updateLog.AppendLine(resultLog);
                        }
                        i++;
                    }
                    while (!lines[i].Contains("End_Set_Port_Mapping"));
                }
                sb.AppendLine(lines[i]);
            }
            File.WriteAllText(file, sb.ToString(),Encoding.Default);
            return updateLog;
        }

        private static string ReplaceStr(string pattern,string fileName,out string replaceLog)
        {
            string retValue = string.Empty;
            replaceLog = string.Empty;
            try
            {                
                if (!File.Exists(fileName) || !pattern.Contains("--")) return retValue;
                string[] lines = File.ReadAllLines(fileName, Encoding.Default);
                string searchPattern = pattern.Substring(pattern.IndexOf("--") + 2);
                string replaceTo = GetMappingValue(pattern);
                string replaceBy = string.Empty;
                foreach (string line in lines)
                {
                    if (line.Contains(searchPattern))
                    {
                        replaceBy = GetMappingValueFromBitPortFile(line);
                        if (pattern.Contains("_Emulated"))
                        {
                            retValue = pattern.Replace(replaceTo, replaceBy + "_Emulated");
                            replaceLog = "替换" + searchPattern + ":" + replaceBy + "_Emulated";
                        }
                        else
                        {
                            retValue = pattern.Replace(replaceTo, replaceBy);
                            replaceLog = "替换" + searchPattern + ":" + replaceBy;
                        }
                        break;
                    }
                }
            }
            catch
            { }
            return retValue; 
        }
        private static string GetMappingValue(string str)
        {
            string retValue = string.Empty;
            string [] strArr = str.Split(',');
            try
            {
                string [] arrs = strArr[0].Split('"');
                retValue = arrs[1].Trim();
            }
            catch
            { }
            return retValue;

        }
        private static string GetMappingValueFromBitPortFile(string str)
        {
            string retValue = string.Empty;
            string[] strArr = str.Split(',');
            try
            {
                foreach (string s in strArr)
                {
                    if (s.ToLower().Contains("path"))
                    {
                        string subS = s.Trim();
                        retValue = subS.Substring(subS.IndexOf('=') + 2, subS.Length - subS.IndexOf('=') - 3);
                    }
                }
            }
            catch
            { }
            return retValue;
        }
        public static void ReplaceFile(string path,string template)
        {
            try
            {
                string[] contents = File.ReadAllLines(path + "//..//Template//" + template, Encoding.Default);
                File.WriteAllLines(path + "//..//Test_Core_Lib//Name_Mapping.lua", contents, Encoding.Default);
            }
            catch
            { }
        }

        public static void WriteToTestLog(string logPath, string input)
        {
            var contents = new List<string>();
            contents.Add(input);
            string logName = string.Format("Test_Result_{0}_{1}_{2}_{3}_{4}_{5}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
            File.WriteAllLines(logPath + "\\" + logName, contents);
        }
    }
}
