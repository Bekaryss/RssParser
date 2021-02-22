using Newtonsoft.Json;
using RssParser.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RssParser
{
    class Program
    {
        static void Main(string[] args)
        {
            RssFactory rf = new RssFactory();
            Console.WriteLine("Press ESC to stop");
            int n = 0;
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                Console.WriteLine("Iteration in 10 minute - " + n);
                rf.ParseAndAddFilms();
                rf.ParseAndAddSeries();
                Console.WriteLine("Press ESC to stop");
                Thread.Sleep(600000);
                n++;
            }
        }
        static void ParseRssFile()
        {
            string url = "https://hdslon.club/films/rss.xml";
            RssParserHelper help = new RssParserHelper();
            help.ParseRssFile(url);
            // Return the string that contain the RSS items

        }
    }
}
