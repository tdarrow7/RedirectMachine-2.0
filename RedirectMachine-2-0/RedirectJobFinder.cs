using Gizmo;
using System;
using System.Collections.Generic;
using System.IO;

namespace RedirectMachine_2_0
{
    internal class RedirectJobFinder
    {
        private string root;
        private List<RedirectJob> jobList = new List<RedirectJob>();

        public RedirectJobFinder(string root)
        {
            this.root = root;
        }

        internal void Run()
        {
            jobList = setUpJobs();
            Console.WriteLine($"number of jobs: {jobList.Count}");
            startJobs(jobList);

            if (jobList.Count > 0)
            {
                string combineEmails = "";
                foreach (var job in jobList)
                {
                    combineEmails += job.EmailAddresses + ";";
                }
                Gremlin.SendEmail("timothy.darrow@scorpion.co", $"there are ({jobList.Count}) new jobs", $"New jobs have been dispatched. emails have been sent to {combineEmails}");
            }
        }

        public int returnJobCount()
        {
            return jobList.Count;
        }


        private List<RedirectJob> setUpJobs()
        {
            List<RedirectJob> jobList = new List<RedirectJob>();
            string[] subdirectoryEntries = Directory.GetDirectories(root);
            foreach (string subDirectory in subdirectoryEntries)
            {
                if (checkIfJobIsNeeded(subDirectory))
                {
                    jobList.Add(new RedirectJob(subDirectory));
                }
            }
            return jobList;
        }

        private static bool checkIfJobIsNeeded(string directory)
        {
            string[] subDirectories = Directory.GetDirectories(directory);
            return (subDirectories.Length < 1) ? true : false;
        }

        private void startJobs(List<RedirectJob> jobList)
        {
            foreach (var job in jobList)
            {
                job.Start();
            }
        }
    }
}