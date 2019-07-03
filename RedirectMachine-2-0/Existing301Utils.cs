using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine_2_0
{
    internal class Existing301Utils
    {
        internal List<Tuple<string, string>> existing301Params;
        Dictionary<string, int> catchAllList;
        internal int CatchAllCount = 0;

        /// <summary>
        /// default working constructor
        /// </summary>
        public Existing301Utils()
        {
            catchAllList = new Dictionary<string, int>();
            existing301Params = new List<Tuple<string, string>>();
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool checkExisting301Redirects(UrlDto urlDto)
        {
            string url = urlDto.OriginalUrl;
            if (url.Contains("?"))
            {
                CatchAllCount++;
                string urlSub = url.Split('?')[0] + "?";
                //existing301Params.Add(new Tuple<string, string>(urlSub, "(needs to be determined)"));
                checkIf301IsCreated(urlSub);
                urlDto.Is301 = false;
                return true;
            }
            foreach (var tuple in existing301Params)
            {
                if (url.StartsWith(tuple.Item1))
                {
                    CatchAllCount++;
                    urlDto.Is301 = true;
                    checkIf301IsCreated(url);
                    return true;
                }

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
                catchAllList.Add(url, 1);
            else
            {
                int count = catchAllList[url] + 1;
                catchAllList[url] = count;

            }
        }

        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        public List<string> ExportCatchAllsToList()
        {
            List<string> listOf301s = new List<string>();
            listOf301s.Add("Potential 301 catchall redirect,Number of times seen,Notes");
            foreach (var keyValuePair in catchAllList)
            {
                if (keyValuePair.Value > 1)
                {
                    if (keyValuePair.Key.Contains("?"))
                        listOf301s.Add($"{keyValuePair.Key},{keyValuePair.Value},Query Parameter");
                    else
                        listOf301s.Add($"{keyValuePair.Key},{keyValuePair.Value}");
                }
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