using Gizmo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RedirectMachine_2_0
{
    internal class Job
    {
        public string Directory { get; set; }
        public string OldSiteUrlFilePath { get; set; }
        public string NewSiteUrlFilePath { get; set; }
        public string ExistingRedirectFilePath { get; set; }
        public string LoggerFile { get; set; }
        public string OutputFolder { get; set; }
        public string OutputFoundRedirects { get; set; }
        public string OutputLostRedirects { get; set; }
        public string OutputCatchalls { get; set; }
        //public List<string> LogEntries = new List<string>();
        //public CatchAllUtils catchAllUtilObject = new CatchAllUtils();
        //public List<Tuple<string, string>> newUrlSiteMap = new List<Tuple<string, string>>();
        //public List<UrlDto> urlDtos = new List<UrlDto>();
        public string EmailAddresses { get; set; }

        public Job(string directory)
        {
            Directory = directory;
            OldSiteUrlFilePath = System.IO.Directory.GetFiles(Directory, "*OldSiteUrls.csv")[0];
            NewSiteUrlFilePath = System.IO.Directory.GetFiles(Directory, "*NewSiteUrls.csv")[0];
            ExistingRedirectFilePath = System.IO.Directory.GetFiles(Directory, "*ExistingRedirects.csv")[0];
            LoggerFile = System.IO.Directory.GetFiles(Directory, "Log.txt")[0];
            OutputFolder = Path.Combine(Directory, @"Output");
            OutputFoundRedirects = Path.Combine(OutputFolder, @"FoundRedirects.csv");
            OutputLostRedirects = Path.Combine(OutputFolder, @"LostRedirects.csv");
            OutputCatchalls = Path.Combine(OutputFolder, @"PossibleCatchalls.csv");

            Console.WriteLine(LoggerFile);

            checkforEmailAddressesInLog();
        }

        private void checkforEmailAddressesInLog()
        {
            string line = File.ReadLines(LoggerFile).First();

            //using (StreamReader sr = new StreamReader(LoggerFile))
            //{
            //    string line2;
            //    while ((line2 = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(line2);
            //    }
            //}

            //Console.WriteLine(line);
            EmailAddresses = (line.Contains('@')) ? line : "timothy.darrow@scorpion.co";
            Console.WriteLine(EmailAddresses);
        }

        public void Start()
        {
            //buildLoggerFile();
            //LogEntries.Add($"Beginning of log for {Path.GetDirectoryName(Directory)}");
            //catchAllUtilObject.GenerateCatchAllParams(ExistingRedirectFilePath);
            //ImportNewUrlsIntoList(NewSiteUrlFilePath);
            //DirectoryInfo dr = System.IO.DirectoryInfo.CreateDirectory(OutputFolder);
            System.IO.Directory.CreateDirectory(@"" + OutputFolder);
            Console.WriteLine("starting");
            Gremlin.EmailTo = EmailAddresses;
            startRedirectFinder();

            //Gremlin.Info($"Your redirect job is done. Go get it from {Directory}", sendEmail: true);
            Gremlin.SendEmail(EmailAddresses, $"Your redirect job is done. Go get it from {Directory}");
        }

        private void startRedirectFinder()
        {
            RedirectFinder redirectFinder = new RedirectFinder();
            redirectFinder.osUrlFile = OldSiteUrlFilePath;
            redirectFinder.nsUrlFile = NewSiteUrlFilePath;
            redirectFinder.existingRedirectsFile = ExistingRedirectFilePath;
            redirectFinder.catchAllFile = OutputCatchalls;
            redirectFinder.foundUrlFile = OutputFoundRedirects;
            redirectFinder.lostUrlFile = OutputLostRedirects;
            redirectFinder.Run();
        }

        private void buildLoggerFile()
        {
            try
            {

                // Delete the file if it exists.
                if (File.Exists(LoggerFile))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    File.Delete(LoggerFile);
                }


                // Create the file.
                File.Create(LoggerFile);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        private void writeToLog(string v)
        {
            using (StreamWriter fs = new StreamWriter(LoggerFile))
            {
                // Add some information to the file.
                fs.WriteLine(v);
            }
        }

        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        //private void ImportNewUrlsIntoList(string urlFile)
        //{
        //    using (var reader = new StreamReader(@"" + urlFile))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string[] tempArray = reader.ReadLine().ToLower().Split(",");
        //            newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
        //        }
        //    }
        //}
    }
}