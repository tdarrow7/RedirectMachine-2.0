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

        public RedirectJob(string directory)
        {
            jobIOProcessor = new RedirectJobIOProcessor(directory);
            EmailAddresses = jobIOProcessor.getEmailAddresses();
        }

        public void Start()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string currentTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            Console.WriteLine(DateTime.Now);
            jobIOProcessor.addToLogDump($"job started at {currentTime}");
            jobIOProcessor.CreateOutputDirectory();
            Console.WriteLine("starting");
            startRedirectFinder();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            jobIOProcessor.addToLogDump(Environment.NewLine + $"Run time: {elapsedTime}");
            jobIOProcessor.addToLogDump($"Number of found redirects: {redirectFinder.FoundCount}");
            jobIOProcessor.addToLogDump($"Number of lost redirects: {redirectFinder.LostCount}");
            jobIOProcessor.addToLogDump($"Number of catchalls: {redirectFinder.GetCatchallCount()}");

            jobIOProcessor.writeToLogDump();
            Console.WriteLine($"Sending email to {EmailAddresses}");
            //Gremlin.Info($"Your redirect job is done. Go get it from {Directory}", sendEmail: true);
            Gremlin.SendEmail(EmailAddresses, $"Your redirect job for {Path.GetFileName(jobIOProcessor.Directory)} is done.", $"Your redirect job for {jobIOProcessor.Directory} is done. Please retrieve it within 24 hours");
        }

        private void startRedirectFinder()
        {
            redirectFinder = new RedirectFinder();
            redirectFinder.InputOldUrlFile = jobIOProcessor.InputOldUrlFile;
            redirectFinder.InputNewUrlFile = jobIOProcessor.InputNewUrlFile;
            redirectFinder.InputExisting301File = jobIOProcessor.InputExisting301File;
            redirectFinder.OutputCatchAllFile = jobIOProcessor.OutputCatchAllFile;
            redirectFinder.OutputFoundUrlFile = jobIOProcessor.OutputFoundUrlFile;
            redirectFinder.OutputLostUrlFile = jobIOProcessor.OutputLostUrlFile;
            redirectFinder.Run();
        }
    }
}