using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RssParser
{
    public class RssParserHelper
    {
        public async Task<XmlDocument> GetRssDocument(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = await req.GetResponseAsync())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = resp.ContentType.Contains("utf-8") ? Encoding.UTF8 : Encoding.GetEncoding("windows-1251");
                using (var stream = new StreamReader(resp.GetResponseStream(), encoding))
                {
                    XmlDocument rssXmlDoc = new XmlDocument();
                    rssXmlDoc.Load(stream);

                    return rssXmlDoc;
                }
            }
        }
        public async Task<List<Item>> ParseRssFile(string url)
        {
            XmlDocument rssXmlDoc = await GetRssDocument(url);
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");
            XmlSerializer serial = new XmlSerializer(typeof(Item));
            var nsmgr = new XmlNamespaceManager(rssXmlDoc.NameTable);
            nsmgr.AddNamespace("turbo", "http://turbo.yandex.ru");
            nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            List<Item> films = new List<Item>();

            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                Item item = (Item)serial.Deserialize(new XmlNodeReader(rssNode));
                XmlNode rssSubNode = rssNode.SelectSingleNode("//turbo:content", nsmgr);
                item.Content = rssSubNode != null ? rssSubNode.InnerText : "";
                rssSubNode = rssNode.SelectSingleNode("//dc:creator", nsmgr);
                item.Creator = rssSubNode != null ? rssSubNode.InnerText : "";
                films.Add(item);
            }
            return films;
        }

    }
}
