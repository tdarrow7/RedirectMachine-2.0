using Gizmo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedirectMachine_2_0
{
    class Program
    {
        /// <summary>
        /// Let the games begin
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Gremlin.Init(Environment.MachineName);
            Gremlin.EmailTo = "timothy.darrow@scorpion.co";


            string root = @"S:\S-Z\Timothy Darrow\Redirect Machine";
            RedirectJobFinder jobs = new RedirectJobFinder(root);

            int jobCount = jobs.returnJobCount();

            //Gremlin.Info("Starting Redirect Machine. Number of jobs: " + jobCount, sendEmail: true);
            jobs.Run();


            Console.WriteLine("closing gremlin");
            Gremlin.Close();
        }
    }
}
