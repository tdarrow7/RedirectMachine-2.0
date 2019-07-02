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
        internal RedirectJobIOProcessor jobIOProcessor;
        internal RedirectFinder redirectFinder;
        internal UrlUtils utils;
        internal Existing301Utils existing301Utils;
        internal List<Tuple<string, string>> newUrlSiteMap;
        internal List<Tuple<string, string>> urlHeaderMaps = new List<Tuple<string, string>>();

        public Job(string directory)
        {
            jobIOProcessor = new RedirectJobIOProcessor(directory);
            redirectFinder = new RedirectFinder();
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            EmailAddresses = jobIOProcessor.getEmailAddresses();
            jobIOProcessor.addToLogDump($"job started at {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}");
            jobIOProcessor.CreateOutputDirectory();
            Console.WriteLine("starting");
            importExisting301s();
            importUrlHeaderMaps();

            startRedirectFinder();

            jobIOProcessor.writeToLogDump();
            Console.WriteLine($"Sending email to {EmailAddresses}");
            Gremlin.SendEmail(EmailAddresses, $"Your redirect job for {Path.GetFileName(jobIOProcessor.Directory)} is done.", $"Your redirect job for {jobIOProcessor.Directory} is done. Please retrieve it within 24 hours");
        }

        private void importExisting301s()
        {
            foreach (var tuple in jobIOProcessor.temp301s)
            {
                existing301Utils.AddNewCatchAllParam(tuple);
            }
        }

        private void importUrlHeaderMaps()
        {
            foreach (var tuple in jobIOProcessor.temp301s)
            {
                urlHeaderMaps.Add(tuple);
            }
        }

        private void startRedirectFinder()
        {
            //redirectFinder = new RedirectFinder();
            redirectFinder.InputOldUrlFile = jobIOProcessor.InputOldUrlFile;
            redirectFinder.InputNewUrlFile = jobIOProcessor.InputNewUrlFile;
            redirectFinder.InputExisting301File = jobIOProcessor.InputExisting301File;
            redirectFinder.OutputCatchAllFile = jobIOProcessor.OutputCatchAllFile;
            redirectFinder.OutputFoundUrlFile = jobIOProcessor.OutputFoundUrlFile;
            redirectFinder.OutputLostUrlFile = jobIOProcessor.OutputLostUrlFile;
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