using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Web;
using System.IO;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Timers;

namespace eLublin.HTMLCrawl
{
    class Program
    {
        private static Timer TimerLublin112;
        static void Main(string[] args)
        {
            TimerLublin112 = new Timer(60 * 1000);
            TimerLublin112.AutoReset = true;
            TimerLublin112.Elapsed += OnTimedLublin112Event;
            TimerLublin112.Enabled = true;
            OnTimedLublin112Event(null, null);
            Console.ReadLine();
        //Console.ReadLine();
        }

        private static void OnTimedLublin112Event(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Lublin112");
            Lublin112Crawl.Migrate();
        }
    }
}
