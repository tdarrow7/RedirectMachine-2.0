using Gizmo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RedirectMachine_2_0
{
    internal class RedirectJob
    {
        public string EmailAddresses { get; set; }
        internal RedirectJobIOProcessor jobIOProcessor;
        internal RedirectFinder redirectFinder;
        internal UrlUtils utils;
        internal Existing301Utils existing301Utils;
        internal List<Tuple<string, string>> newUrlSiteMap;
        internal List<Tuple<string, string>> urlHeaderMaps;
        internal List<UrlDto> urlDtos;

        public RedirectJob(string directory)
        {
            utils = new UrlUtils();
            existing301Utils = new Existing301Utils();
            newUrlSiteMap = new List<Tuple<string, string>>();
            jobIOProcessor = new RedirectJobIOProcessor(directory);
            redirectFinder = new RedirectFinder();
            urlHeaderMaps = new List<Tuple<string, string>>();
            urlDtos = new List<UrlDto>();
        }

        public void Start()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            EmailAddresses = jobIOProcessor.getEmailAddresses();
            jobIOProcessor.addToLogDump($"job started at {DateTime.Now.ToString("MM/dd/yyyy HH:mm")}");
            jobIOProcessor.CreateOutputDirectory();
            Console.WriteLine("starting");
            importListsFromFiles();
            startRedirectFinder(); 
            exportListsToFiles();
            jobIOProcessor.writeToLogDump();
            Console.WriteLine($"Sending email to {EmailAddresses}");
            Gremlin.SendEmail(EmailAddresses, $"Your redirect job for {Path.GetFileName(jobIOProcessor.Directory)} is done.", $"Your redirect job for {jobIOProcessor.Directory} is done. Please retrieve it within 24 hours");
        }

        private void exportListsToFiles()
        {
            jobIOProcessor.ExportNewCSVs(urlDtos);
            jobIOProcessor.ExportToCSV(existing301Utils.ExportCatchAllsToCSV(), jobIOProcessor.Output301CatchAllFile);
        }

        private void importListsFromFiles()
        {
            importExisting301s();
            importUrlHeaderMaps();
            importNewUrlsIntoList();
            createListOfDtos();
        }

        private void importNewUrlsIntoList()
        {
            List<Tuple<string, string>> list = jobIOProcessor.ImportNewUrlsIntoList();
            foreach (var tuple in list)
            {
                newUrlSiteMap.Add(tuple);
            }
        }

        private void createListOfDtos()
        {
            List<string> list = jobIOProcessor.ImportOldUrlsIntoList();
            foreach (var line in list)
            {
                urlDtos.Add(createUrlDto(line));
            }
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
            redirectFinder.Run(urlDtos, newUrlSiteMap, existing301Utils);
        }

        /// <summary>
        /// For Every line in CSV, read line and check if line belongs in a catchAll. If not, create new RedirectUrl Object.
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportOldUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string url = reader.ReadLine().ToLower();
                    //if (!catchAllUtilObject.CheckExistingCatchallParams(url) && !existingRedirects.Contains(url))
                    //    urlDtos.Add(createUrlDto(url));
                }
            }
            Console.WriteLine("Done importing old urls into list");
        }


        /// <summary>
        /// return a UrlDto object with all the props from UrlUtils class
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private UrlDto createUrlDto(string url)
        {
            UrlDto urlDto = new UrlDto(url);
            urlDto.UrlParentDir = utils.TruncateStringHead(url);
            urlDto.UrlResourceDir = utils.TruncateString(url, 48);
            urlDto.UrlResourceDirChunks = utils.ReturnUrlChunks(urlDto.UrlResourceDir);
            urlDto.UrlAllChunks = utils.ReturnUrlChunks(url);
            urlDto.RemappedParentDir = utils.ReturnRemappedUrlParentDir(url, urlHeaderMaps);
            return urlDto;
        }
    }
}