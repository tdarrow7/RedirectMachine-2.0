using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine_2_0
{
    internal class RedirectJobIOProcessor
    {
        public string Directory { get; set; }
        public string InputOldUrlFile { get; set; }
        public string InputNewUrlFile { get; set; }
        public string InputExisting301File { get; set; }
        public string LoggerFile { get; set; }
        public string OutputFolder { get; set; }
        public string OutputFoundUrlFile { get; set; }
        public string OutputLostUrlFile { get; set; }
        public string OutputCatchAllFile { get; set; }
        private string emailAddresses { get; set; }
        private List<string> logDump = new List<string>();

        public RedirectJobIOProcessor(string directory)
        {
            Directory = directory;
            InputOldUrlFile = System.IO.Directory.GetFiles(Directory, "*OldSiteUrls.csv")[0];
            InputNewUrlFile = System.IO.Directory.GetFiles(Directory, "*NewSiteUrls.csv")[0];
            InputExisting301File = System.IO.Directory.GetFiles(Directory, "*Existing301s.csv")[0];
            LoggerFile = Directory + @"\Log.txt";
            OutputFolder = Path.Combine(Directory, @"Output");
            OutputFoundUrlFile = Path.Combine(OutputFolder, @"FoundRedirects.csv");
            OutputLostUrlFile = Path.Combine(OutputFolder, @"LostRedirects.csv");
            OutputCatchAllFile = Path.Combine(OutputFolder, @"PossibleCatchalls.csv");

            checkForLog();
        }

        internal string getEmailAddresses()
        {
            return emailAddresses;
        }

        private void checkForLog()
        {
            string line = "";
            if (File.Exists(LoggerFile) && new FileInfo(LoggerFile).Length > 0)
            {
                line = File.ReadLines(LoggerFile).First();
                File.Delete(LoggerFile);
            }

            emailAddresses = (line.Contains('@')) ? line : "timothy.darrow@scorpion.co";
            addToLogDump(emailAddresses);

        }

        internal void addToLogDump(string v)
        {
            logDump.Add(v);
        }

        internal void writeToLog(string v)
        {
            using (StreamWriter fs = new StreamWriter(LoggerFile))
            {
                fs.WriteLine(v);
            }
        }

        internal void CreateOutputDirectory()
        {
            System.IO.Directory.CreateDirectory(@"" + OutputFolder);
        }

        /// <summary>
        /// write all information collected in dump collection to dump file
        /// </summary>
        internal void writeToLogDump()
        {
            using (StreamWriter fs = new StreamWriter(LoggerFile))
            {
                foreach (var line in logDump)
                {
                    fs.WriteLine(line);
                }
            }
        }
    }
}