using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Management;
using System.Text.RegularExpressions;

namespace EventLoggingMPC5
{
    public static class ProcessLogger
    {
        public static void Start() {
            string processName = "mpc-hc.exe";
            string typeOfProcess = "Win32_Process";
            string query = String.Format("Select * From __InstanceCreationEvent Within 1 Where TargetInstance Isa '{0}' And TargetInstance.Name = '{1}'", typeOfProcess, processName);
            WqlEventQuery wqlQuery = new WqlEventQuery(query);

            ManagementEventWatcher watcher = new ManagementEventWatcher(wqlQuery);
            watcher.EventArrived += new EventArrivedEventHandler(ProcessStarted);
            watcher.Start();
        }

        static void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processInfo = targetInstance.Properties["CommandLine"].Value.ToString();
            string fileToLog;
            List<string> tmp;
            if ((tmp = Regex.Split(processInfo, "\" \"").ToList()).Count() > 1)
            {
                fileToLog = tmp[1];
                fileToLog = fileToLog.Trim('"');
                LogOpening(fileToLog);
            }
        }

        static void LogOpening(string fp)
        {
            string currentDateTime = DateTime.Now.ToString("u");
            string logOutput = String.Format("{0} - INFO - {1}", currentDateTime, fp);
            string userProfileDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string logFile = Path.Combine(userProfileDirectory, "MPC-HC-alt.log");
            string DropBoxDirectory = DropboxPath();

            if (!File.Exists(logFile))
            {
                File.Create(logFile);
            }
            // Append to the file.
            using (StreamWriter w = File.AppendText(logFile))
            {
                w.WriteLine(Regex.Replace(logOutput, @"\r\n?|\n", ""));
            }
            // Copy into Dropbox if it exists.
            if (DropBoxDirectory != null)
            {
                File.Copy(logFile, Path.Combine(DropBoxDirectory, "MPC-HC-alt.log"), true);
            }
        }

        static string DropboxPath()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbPath = System.IO.Path.Combine(appDataPath, "Dropbox\\host.db");
                var lines = System.IO.File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);
                string folderPath = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
                return folderPath;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
