using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RssParser.Models.bson
{
    public class Guid
    {
        [BsonElement("IsPermaLink")]
        public string IsPermaLink { get; set; }
        [BsonElement("Text")]
        public string Text { get; set; }
    }
    public class FSItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }
        [BsonElement("Guid")]
        public Guid Guid { get; set; }
        [BsonElement("Link")]
        public string Link { get; set; }
        [BsonElement("ImageId")]
        [BsonIgnore]
        public string imgUrl { get; set; }
        public ObjectId ImageId { get; set; }
        [BsonElement("Description")]
        public string Description { get; set; }
        [BsonElement("Country")]
        public string Country { get; set; }
        [BsonElement("KinopoiskLink")]
        public string KinopoiskLink { get; set; }
        [BsonElement("SoundEpisode")]
        public string SoundEpisode { get; set; }
        [BsonElement("OriginalName")]
        public string OriginalName { get; set; }
        [BsonElement("Content")]
        public string Content { get; set; }
        [BsonElement("Category")]
        public string Category { get; set; }
        [BsonElement("Creator")]
        public string Creator { get; set; }
        [BsonElement("PubDate")]
        public string PubDate { get; set; }
        [BsonElement("Turbo")]
        public string Turbo { get; set; }
    }
}
