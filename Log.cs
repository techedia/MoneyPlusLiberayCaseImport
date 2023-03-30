using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace MoneyPlusLiberayCaseImport
{
    class Log
    {
        private string strLogFilePath = "";
        private string strLogFilePathErrors = "";
        private string strUser = "";
        private string strFullUserName = "";

        public Log()
        {
            SetupUserName();
            SetupLogFile();
        }

        public void LogToFile(string strTextToLog, bool boolLogToErrorFile)
        {
            try
            {
                Console.WriteLine(strTextToLog);

                strTextToLog = DateTime.Now.ToString("ddMMyy HH:mm:ss") + " : " + strTextToLog;

                if (strLogFilePath != "")
                {
                    //Log to current log file
                    using (StreamWriter swFile = File.AppendText(strLogFilePath))
                    {
                        swFile.WriteLine(strTextToLog);
                    }

                    if (boolLogToErrorFile)
                    {
                        using (StreamWriter swFile = File.AppendText(strLogFilePathErrors))
                        {
                            swFile.WriteLine(strTextToLog);
                        }
                    }
                }

                Console.WriteLine(strTextToLog);
            }
            catch (Exception ex)
            {

            }
        }

        private string SetupLogFile()
        {
            //Log file for each day so setup folder for each day (create if required) and then create log file per uses
            string strLogFileDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Logging\" + DateTime.Now.ToString("ddMMyyyy");

            if (!Directory.Exists(strLogFileDir))
            {
                Directory.CreateDirectory(strLogFileDir);
                System.Threading.Thread.Sleep(1000);
            }

            strLogFilePath = strLogFileDir + @"\" + strUser + ".txt";
            strLogFilePathErrors = strLogFileDir + @"\" + strUser + "_Error.txt";

            return "";
        }

        private void SetupUserName()
        {
            strFullUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            strUser = Remove_Special_Characters(strFullUserName);
        }

        public static string Remove_Special_Characters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string FullUserName
        {
            get { return strFullUserName; }
        }
    }
}
