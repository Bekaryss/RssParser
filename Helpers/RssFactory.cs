using HtmlAgilityPack;
using RssParser.Models.bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace RssParser.Helpers
{
    public class RssFactory
    {
        public void ParseAndAddFilms()
        {
            RssParserHelper rph = new RssParserHelper();
            DbHelper db = new DbHelper();
            Task<List<Item>> task = rph.ParseRssFile("https://hdslon.club/films/rss.xml");
            task.Wait();
            List<FSItem> films = ConvertToFSItem(task.Result);
            films = db.UpdateAndReturnNewFilms(films);
            if (films.Count != 0)
            {
                var curT = db.AddFilms(films);
                curT.Wait();
            }

            Console.WriteLine("Done Films");
        }

        public void ParseAndAddSeries()
        {
            RssParserHelper rph = new RssParserHelper();
            DbHelper db = new DbHelper();
            Task<List<Item>> task = rph.ParseRssFile("https://hdslon.club/serialy/rss.xml");
            task.Wait();
            List<FSItem> series = ConvertToFSItem(task.Result);
            series = db.UpdateAndReturnNewSereis(series);
            if (series.Count != 0)
            {
                var curT = db.AddSeriesList(series);
                curT.Wait();
            }

            Console.WriteLine("Done Sereis");
        }

        public List<FSItem> ConvertToFSItem(List<Item> items)
        {
            List<FSItem> films = new List<FSItem>();
            foreach (var item in items)
            {
                FSItem fsItem = new FSItem();

                fsItem.Guid = new Models.bson.Guid
                {
                    IsPermaLink = item.Guid.IsPermaLink,
                    Text = item.Guid.Text
                };
                fsItem.Title = item.Title;
                fsItem.Link = item.Link;
                fsItem.PubDate = item.PubDate;
                fsItem.Turbo = item.Turbo;
                fsItem.Description = item.Description.InnerText;
                var pageContents = item.Description.Data;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var imgSrc = pageDocument.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
                var ahref = pageDocument.DocumentNode.SelectSingleNode("//a[@*[contains(., 'country')]]");

                var kinoP = pageDocument.DocumentNode.SelectSingleNode("//a[@*[contains(., 'kinopoisk')]]");
                var aKino = "";
                if (kinoP != null)
                    aKino = kinoP.Attributes["href"].Value;
                var fonts = pageDocument.DocumentNode.SelectNodes("//font");

                DbHelper db = new DbHelper();

                string url = "";
                if (imgSrc.Contains("https://hdslon.club"))
                {
                    url = imgSrc;
                }
                else
                {
                    url = "https://hdslon.club" + imgSrc;
                }
                fsItem.imgUrl = url;
                if (ahref != null)
                    fsItem.Country = ahref.InnerText;
                fsItem.KinopoiskLink = aKino;
                int n = 0;
                if (fonts != null)
                    foreach (var font in fonts)
                    {
                        if (n == 0)
                            fsItem.SoundEpisode = font.FirstChild.InnerText;
                        else
                            fsItem.OriginalName = font.LastChild.InnerText;
                        n++;
                    }

                fsItem.Content = item.Content;
                fsItem.Category = item.Category;
                fsItem.Creator = item.Creator;
                films.Add(fsItem);
            }
            return films;
        }
    }
}
