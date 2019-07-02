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
        internal List<Tuple<string, string>> temp301s = new List<Tuple<string, string>>();
        internal List<Tuple<string, string>> tempUrlHeaderMaps = new List<Tuple<string, string>>();

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

        /// <summary>
        /// using the Existing301Redirects file, determine what lines are catchalls and what are redirected site maps
        /// if the line ends with a true bool or is null, add the line as a catchall redirect
        /// if the line ends with false, add the line as a headerMap tuple
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportExisting301s(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] redirectLine = reader.ReadLine().ToLower().Split(',');
                    if (redirectLine[2] == "true" || redirectLine[2] == null)
                    {
                        temp301s.Add(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
                        //catchAllUtilObject.AddNewCatchAllParam(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
                    }
                    else
                        tempUrlHeaderMaps.Add(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
                }
            }
            Console.WriteLine("Done Importing");
        }

        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        internal List<Tuple<string, string>> ImportNewUrlsIntoList()
        {
            List<Tuple<string, string>> newUrlSiteMap = new List<Tuple<string, string>>();
            using (var reader = new StreamReader(InputNewUrlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] tempArray = reader.ReadLine().ToLower().Split(',');
                    newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
                }
            }
            Console.WriteLine("Done importing new urls into list");
            return newUrlSiteMap;
        }
    }
}