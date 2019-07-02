using System;
using System.Collections.Generic;
using System.IO;

namespace RedirectMachine_2_0
{
    internal class RedirectJobFinder
    {
        private string root;
        private List<Job> jobList = new List<Job>();

        public RedirectJobFinder(string root)
        {
            this.root = root;
        }

        internal void Run()
        {
            jobList = setUpJobs();
            Console.WriteLine($"number of jobs: {jobList.Count}");
            startJobs(jobList);
        }

        public int returnJobCount()
        {
            return jobList.Count;
        }


        private List<Job> setUpJobs()
        {
            List<Job> jobList = new List<Job>();
            string[] subdirectoryEntries = Directory.GetDirectories(root);
            foreach (string subDirectory in subdirectoryEntries)
            {
                if (checkCreationDate(subDirectory) && checkIfJobIsNeeded(subDirectory))
                {
                    jobList.Add(new Job(subDirectory));
                }
            }
            return jobList;
        }

        private bool checkCreationDate(string subDirectory)
        {
            DateTimeOffset creation = File.GetCreationTime(subDirectory);
            DateTimeOffset currentTime = DateTime.Now;
            Console.WriteLine($"creation date: {creation}");
            Console.WriteLine($"current time: {currentTime}");
            Console.WriteLine($"how long ago the file was created: {currentTime.Subtract(creation).Days}");
            return true;
        }

        private static bool checkIfJobIsNeeded(string directory)
        {
            string[] subDirectories = Directory.GetDirectories(directory);
            return (subDirectories.Length < 1) ? true : false;
            //if (subDirectories.Length < 1)
            //{
            //    Console.WriteLine(subDirectories[0]);
            //    return true;
            //}
            //else
            //{
            //    Console.WriteLine("no subdirectories");
            //    return false;
            //}
        }

        private void startJobs(List<Job> jobList)
        {
            foreach (var job in jobList)
            {
                job.Start();
            }
        }
    }
}