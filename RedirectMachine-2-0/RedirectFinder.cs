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
        public static Existing301Utils catchAllUtilObject = new Existing301Utils();
        UrlUtils utils = new UrlUtils();
        RedirectUrl redirectUrlUtils = new RedirectUrl();

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
            Console.WriteLine("begin search: ");
            //FindUrlMatches(urlDtos);
            //StartThreads();
            //catchAllUtilObject.ExportCatchAllsToCSV(catchAllFile);
            //ExportNewCSVs();
            Console.WriteLine("end of exports");
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
                    //newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
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
            //urlDto.RemappedParentDir = utils.ReturnRemappedUrlParentDir(url, urlHeaderMaps);
            return urlDto;
        }

        /// <summary>
        /// using the Existing301Redirects file, determine what lines are catchalls and what are redirected site maps
        /// if the line ends with a true bool or is null, add the line as a catchall redirect
        /// if the line ends with false, add the line as a headerMap tuple
        /// </summary>
        /// <param name="urlFile"></param>
        //private void ImportExisting301s(string urlFile)
        //{
        //    using (var reader = new StreamReader(@"" + urlFile))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string[] redirectLine = reader.ReadLine().ToLower().Split(',');
        //            if (redirectLine[2] == "true" || redirectLine[2] == null)
        //                catchAllUtilObject.AddNewCatchAllParam(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
        //            else
        //                urlHeaderMaps.Add(new Tuple<string, string>(redirectLine[0], redirectLine[1]));
        //        }
        //    }
        //    Console.WriteLine("Done Importing");
        //}

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
            //Console.WriteLine("within url finder");
            //foreach (var urlDto in chunkOfRedirects)
            //{
            //    if (!redirectUrlUtils.findMatching301(urlDto, newUrlSiteMap))
            //    {
            //        urlDto.Flag = "no match";
            //        if (!urlDto.OriginalUrl.Contains("."))
            //        {
            //            catchAllUtilObject.checkIfCatchAllIsCreated(urlDto.OriginalUrl);
            //            catchAllUtilObject.CatchAllCount++;
            //        }

            //    }
            //}
            Console.WriteLine("Done Finding Url Matches");
        }

        //public void StartThreads()
        //{
        //    int count = 1000,
        //        i = redirectUrls.Count / count,
        //        j = 0,
        //        modulus = redirectUrls.Count % count,
        //        length;
        //    List<RedirectUrl> chunkOfRedirects;
        //    Thread[] threads = new Thread[i];
        //    for (int k = 0; k < threads.Length; k++)
        //    {
        //        length = (k == threads.Length - 1) ? count + modulus : count;
        //        chunkOfRedirects = GetChunkOfRedirects(j, length);
        //        Console.WriteLine($"j: {j}");
        //        Console.WriteLine($"length: {length}");
        //        threads[k] = new Thread(() => this.FindUrlMatches(chunkOfRedirects));
        //        j += length;
        //    }
        //    for (int k = 0; k < threads.Length; k++)
        //    {
        //        threads[k].Start();
        //    }

        // }

        //internal List<RedirectUrl> GetChunkOfRedirects(int index, int length)
        //{
        //    List<RedirectUrl> result = redirectUrls.GetRange(index, length);
        //    return result;
        //}

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