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
            string root = @"S:\S-Z\Timothy Darrow\Redirect Machine";
            RedirectJobFinder jobs = new RedirectJobFinder(root);
            jobs.Run();
        }
    }
}
