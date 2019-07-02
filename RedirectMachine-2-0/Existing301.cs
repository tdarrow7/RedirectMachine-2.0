using System;
using System.Collections.Generic;
using System.Text;

namespace RedirectMachine_2_0
{
    class Existing301
    {
        public string OriginalUrl { get; set; }
        public int Count { get; set; } = 1;


        public Existing301(string originalUrl)
        {
            OriginalUrl = originalUrl;
        }

        public Existing301()
        {
        }

        internal void IncreaseCount()
        {
            int count = Count;
            Count = count++;
        }
    }
}
