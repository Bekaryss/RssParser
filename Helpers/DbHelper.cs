using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using RssParser.Models.bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RssParser.Helpers
{
    public class DbHelper
    {
        IMongoDatabase database;

        public IMongoCollection<FSItem> Films
        {
            get { return database.GetCollection<FSItem>("Film"); }
        }

        public IMongoCollection<FSItem> Series
        {
            get { return database.GetCollection<FSItem>("Series"); }
        }
        public DbHelper()
        {
            var client = new MongoClient("mongodb://localhost");
            database = client.GetDatabase("RssNews");
        }

        public List<FSItem> UpdateAndReturnNewFilms(List<FSItem> films)
        {
            List<FSItem> newF = new List<FSItem>();
            foreach(var item in films)
            {
                FSItem curF = GetFilm(item);
                if(curF != null)
                {
                    if (curF.PubDate != item.PubDate)
                    {
                        if (curF.ImageId == ObjectId.Empty)
                        {
                            curF.ImageId = SaveImage(item.imgUrl);
                        }
                        var task = UpdateFilm(curF, item);
                        task.Wait();
                        Console.WriteLine("Update Film " + item.Title);
                    }
                }
                else
                {
                    item.ImageId = SaveImage(item.imgUrl);
                    newF.Add(item);
                    Console.WriteLine("Add Film " + item.Title);
                }
            }
            return newF;
        }

        public List<FSItem> UpdateAndReturnNewSereis(List<FSItem> series)
        {
            List<FSItem> newS = new List<FSItem>();
            foreach (var item in series)
            {
                FSItem curs = GetSeries(item);
                if (curs != null)
                {
                    if (curs.PubDate != item.PubDate)
                    {
                        if(curs.ImageId == ObjectId.Empty)
                        {
                            curs.ImageId = SaveImage(item.imgUrl);
                        }
                        var task = UpdateSeries(curs, item);
                        task.Wait();
                        Console.WriteLine("Update Series " + item.Title);
                    }
                }
                else
                {
                    item.ImageId = SaveImage(item.imgUrl);
                    newS.Add(item);
                    Console.WriteLine("Add Series " + item.Title);
                }
            }
            return newS;
        }

        public FSItem GetFilm(FSItem film)
        {
            return Films.Find(new BsonDocument { { "Guid.Text", film.Guid.Text } }).FirstOrDefault();
        }
        public async Task AddFilm(FSItem film)
        {
            await Films.InsertOneAsync(film);
        }
        public async Task AddFilms(List<FSItem> films)
        {
            await Films.InsertManyAsync(films);
        }
        public async Task UpdateFilm(FSItem curFilm, FSItem film)
        {
            film.Id = curFilm.Id;
            film.ImageId = curFilm.ImageId;
            await Films.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(curFilm.Id)), film);
        }

        public FSItem GetSeries(FSItem sfItem)
        {
            return Series.Find(new BsonDocument { { "Guid.Text", sfItem.Guid.Text } }).FirstOrDefault();
        }
        public async Task AddSeries(FSItem sfItem)
        {
            await Series.InsertOneAsync(sfItem);
        }
        public async Task AddSeriesList(List<FSItem> sfItems)
        {
            await Series.InsertManyAsync(sfItems);
        }
        public async Task UpdateSeries(FSItem curSfItem, FSItem sfItem)
        {
            sfItem.Id = curSfItem.Id;
            sfItem.ImageId = curSfItem.ImageId;
            await Series.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(curSfItem.Id)), sfItem);
        }


        public ObjectId SaveImage(string url)
        {
            IGridFSBucket gridFS = new GridFSBucket(database);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (var resp = req.GetResponse())
            {
                using (var stream = resp.GetResponseStream())
                {
                    int pos = url.LastIndexOf("/") + 1;
                    ObjectId id = gridFS.UploadFromStream(url.ToString().Substring(pos, url.Length - pos), stream);
                    return id;
                }
            }
        }
    }
}
