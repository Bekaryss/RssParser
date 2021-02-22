using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RssParser
{
    [XmlRoot(ElementName = "guid")]
    public class Guid
    {
        [XmlAttribute(AttributeName = "isPermaLink")]
        public string IsPermaLink { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "guid")]
        public Guid Guid { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        
        private string description;
        [XmlElement(ElementName = "description")]
        public XmlCDataSection Description {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(description);
            }
            set
            {
                description = value.Value;
            }
        }
        [XmlElement(ElementName = "content")]
        public string Content { get; set; }
        [XmlElement(ElementName = "category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "creator")]
        public string Creator { get; set; }
        [XmlElement(ElementName = "pubDate")]
        public string PubDate { get; set; }
        [XmlAttribute(AttributeName = "turbo")]
        public string Turbo { get; set; }
    }
}
