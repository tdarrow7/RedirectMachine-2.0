using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RedirectMachine_2_0
{
    internal class RedirectFinder
    {
        // declare all universally needed variables 
        //public List<CatchAllUtils> catchalls = new List<CatchAllUtils>();
        public static List<Tuple<string, string>> newUrlSiteMap = new List<Tuple<string, string>>();
        public static List<UrlDto> urlDtos = new List<UrlDto>();
        public static CatchAllUtils catchAllUtilObject = new CatchAllUtils();
        UrlUtils utils = new UrlUtils();
        RedirectUrl redirectUrlUtils = new RedirectUrl();
        List<string> lostList = new List<string>();
        List<string> foundList = new List<string>();
        HashSet<string> existingRedirects = new HashSet<string>();

        public string InputOldUrlFile { get; set; }
        public string InputNewUrlFile { get; set; }
        public string InputExisting301File { get; set; }
        public string OutputLostUrlFile { get; set; }
        public string OutputFoundUrlFile { get; set; }
        public string OutputCatchAllFile { get; set; }
        public List<Tuple<string, string>> urlHeaderMaps = new List<Tuple<string, string>>();

        public int FoundCount = 0;
        public int LostCount = 0;

        internal object GetCatchallCount()
        {
            return catchAllUtilObject.CatchAllCount;
        }

        /// <summary>
        /// default working constructor
        /// </summary>
        public RedirectFinder()
        {
        }

        /// <summary>
        /// Start the finder program
        /// Initialize stopwatch.
        /// Import both the old urls and new urls into lists
        /// compare the new urls to the old urs using FindUrlMatches
        /// Export all found catchalls to a CSV file
        /// Export the lost url list and found url list to their respective CSVs
        /// Stop and display the recorded time on the stopwatch.
        /// </summary>
        internal void Run()
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            Console.WriteLine("begin search: ");
            //catchAllUtilObject.GenerateCatchAllParams(existingRedirectsFile);
            ImportExisting301s(InputExisting301File);
            ImportNewUrlsIntoList(InputNewUrlFile);
            ImportOldUrlsIntoList(InputOldUrlFile);
            FindUrlMatches(urlDtos);
            ExportNewCSVs();
            catchAllUtilObject.ExportCatchAllsToCSV(OutputCatchAllFile);
            Console.WriteLine("end of exports");
            //stopwatch.Stop();
            //TimeSpan ts = stopwatch.Elapsed;
            //string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            //Console.WriteLine($"Run time: {elapsedTime}");
        }



        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportNewUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] tempArray = reader.ReadLine().ToLower().Split(',');
                    newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
                }
            }
            Console.WriteLine("Done importing new urls into list");
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
                    if (!catchAllUtilObject.CheckExistingCatchallParams(url) && !existingRedirects.Contains(url))
                        urlDtos.Add(createUrlDto(url));
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
                        catchAllUtilObject.AddNewCatchAllParam(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
                    else
                        urlHeaderMaps.Add(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
                }
            }
            Console.WriteLine("Done Importing");
        }

        /// <summary>
        /// check every item in List<RedirectUrl> redirectUrls and compare with items in List<> newUrlSiteMap.
        ///     Try the BasicUrlFinder() method
        ///     Try the AdvancedUrlFinder() method
        ///     Try the ReverseAdvancedUrlFinder() method
        ///     Try the UrlChunkFinder() method
        ///     If a match still wasn't found, add to catchalls
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        public void FindUrlMatches(List<UrlDto> chunkOfRedirects)
        {
            Console.WriteLine("within url finder");
            foreach (var urlDto in chunkOfRedirects)
            {
                if (!redirectUrlUtils.findMatching301(urlDto, newUrlSiteMap))
                {
                    urlDto.Flag = "no match";
                    if (!urlDto.OriginalUrl.Contains("."))
                    {
                        catchAllUtilObject.checkIfCatchAllIsCreated(urlDto.OriginalUrl);
                        catchAllUtilObject.CatchAllCount++;
                    }

                }
            }
            Console.WriteLine("Done Finding Url Matches");
        }

        /// <summary>
        /// Scan all objects in redirectUrls list and put them in either the foundList or lostList, depending on their score
        /// Send both temporary lists to buildCSV method to print both found and lost lists
        /// </summary>
        internal void ExportNewCSVs()
        {
            List<string> foundList = new List<string>();
            List<string> lostList = new List<string>();

            foundList.Add("Old Site Url,Redirected Url,Flag");
            lostList.Add("Old Site Url, Potential Redirected Url");
            foreach (var urlDto in urlDtos)
            {
                if (urlDto.Score == true)
                {
                    FoundCount++;
                    foundList.Add($"{urlDto.OriginalUrl},{urlDto.NewUrl}, {urlDto.Flag}");
                }

                else
                {
                    LostCount++;
                    if (urlDto.matchedUrls.Count > 0)
                    {

                        string[] arrayOfMatches = urlDto.matchedUrls.ToArray();
                        for (int i = 0; i < arrayOfMatches.Length; i++)
                        {
                            if (i == 0)
                                lostList.Add($"{urlDto.OriginalUrl},{arrayOfMatches[i]}");
                            else
                                lostList.Add($",{arrayOfMatches[i]}");
                        }
                    }
                    else
                        lostList.Add($"{urlDto.OriginalUrl}");
                }
            }
            ExportToCSV(foundList, OutputFoundUrlFile);
            ExportToCSV(lostList, OutputLostUrlFile);
            Console.WriteLine($"number of found urls: {FoundCount}");
            Console.WriteLine($"number of lost urls: {LostCount}");
        }

        /// <summary>
        /// build CSV from specified list of strings and export to specified filePath
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        internal void ExportToCSV(List<string> list, string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
        }
    }
}