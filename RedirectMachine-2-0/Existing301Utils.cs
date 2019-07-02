using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine_2_0
{
    internal class Existing301Utils
    {
        internal List<Tuple<string, string>> existing301Params;

        Dictionary<string, Existing301> catchAllList;
        internal int CatchAllCount = 0;

        /// <summary>
        /// default working constructor
        /// </summary>
        public Existing301Utils()
        {
            catchAllList = new Dictionary<string, Existing301>();
            existing301Params = new List<Tuple<string, string>>();
        }


        /// <summary>
        /// Create working list of all pararameters that need to be checked.
        /// </summary>
        /// <param name="urlFile"></param>
        public void GenerateCatchAllParams(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] tempArray = reader.ReadLine().ToLower().Split(',');
                    existing301Params.Add(new Tuple<string, string>(tempArray[0], tempArray[1]));
                }
            }
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool checkExisting301Redirects(UrlDto urlDto)
        {
            string url = urlDto.OriginalUrl;
            foreach (var tuple in existing301Params)
            {
                if (url.StartsWith(tuple.Item1))
                {
                    CatchAllCount++;
                    urlDto.Is301 = true;
                    return true;
                }

            }
            if (url.Contains("?"))
            {
                CatchAllCount++;
                string urlSub = url.Split('?')[0] + "?";
                existing301Params.Add(new Tuple<string, string>(urlSub, "(needs to be determined)"));
                checkIf301IsCreated(urlSub);
                urlDto.Is301 = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// check to see if we already have the catchall
        /// </summary>
        /// <param name="url"></param>
        internal void checkIf301IsCreated(string url)
        {
            if (!catchAllList.ContainsKey(url))
                catchAllList.Add(url, new Existing301(url));
            else
                catchAllList[url].IncreaseCount();
        }


        ///// <summary>
        ///// Sort catchAllList and then export catchAllList to CSV to specified filepath
        ///// </summary>
        ///// <param name="filePath"></param>
        //public void ExportCatchAllsToCSV(string filePath)
        //{
        //    using (TextWriter tw = new StreamWriter(@"" + filePath))
        //    {
        //        tw.WriteLine("Potential Probability,Number of times seen");
        //        foreach (var keyValuePair in catchAllList)
        //        {
        //            tw.WriteLine($"{keyValuePair.Key},{keyValuePair.Value.Count}");
        //        }
        //        foreach (var tuple in existing301Params)
        //        {
        //            tw.WriteLine($"{tuple.Item1}*,{tuple.Item2}");
        //        }
        //    }
        //    Console.WriteLine($"Number of urls turned to catchalls: {CatchAllCount}");
        //}

        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        public List<string> ExportCatchAllsToCSV()
        {
            List<string> listOf301s = new List<string>();
            listOf301s.Add("Potential 301 catchall,Number of times seen");
            foreach (var keyValuePair in catchAllList)
            {
                listOf301s.Add($"{keyValuePair.Key},{keyValuePair.Value.Count}");
            }
            return listOf301s;
        }

        /// <summary>
        /// add tuple 
        /// </summary>
        /// <param name="tuple"></param>
        internal void AddNewCatchAllParam(Tuple<string, string> tuple)
        {
            existing301Params.Add(tuple);
        }
    }
}