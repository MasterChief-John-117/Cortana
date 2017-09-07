using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace xkcd
{
    public class xkcdComicStore
    {
        [JsonProperty("comicList")]
        public List<xkcdComic> ComicList;

        public xkcdComicStore()
        {
            try
            {
                ComicList = JsonConvert.DeserializeObject<List<xkcdComic>>(File.ReadAllText("files/xkcd.json"));
            }
            catch(Exception ex)
            {
                ComicList = new List<xkcdComic>();
            }
        }

        public List<xkcdComic> update()
        {
            try
            {
                var newestComic =
                    JsonConvert.DeserializeObject<xkcdComic>(
                        new WebClient().DownloadString(new Uri("http://xkcd.com/info.0.json")));

                while (ComicList.Last().num != newestComic.num)
                {
                    int last = ComicList.Last().num;
                    if (ComicList.Last().num == 403)
                    {
                        last = 404;
                    }
                    Console.WriteLine($"{last} < {newestComic.num}");
                    var latestComicGrabbed = JsonConvert.DeserializeObject<xkcdComic>(
                        new WebClient().DownloadString(new Uri($"http://xkcd.com/{last + 1}/info.0.json")));
                    ComicList.Add(latestComicGrabbed);
                }
            }
            catch (Exception ex)
            {
                
            }
            return ComicList;
        }

        public List<xkcdComic> create()
        {
            ComicList.Add(JsonConvert.DeserializeObject<xkcdComic>(new WebClient().DownloadString(new Uri("http://xkcd.com/1/info.0.json"))));
            update();
            return ComicList;
        }
    }
}
