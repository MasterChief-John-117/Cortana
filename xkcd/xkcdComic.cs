using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xkcd
{
    public class xkcdComic
    {
        [JsonProperty("num")]
        public int num;
        [JsonProperty("title")]
        public string title;
        [JsonProperty("safe_title")]
        public string safe_title;
        [JsonProperty("img")]
        public string img;
        [JsonProperty("alt")]
        public string alt;
        [JsonProperty("transcript")]
        public string transcript;
        [JsonProperty("month")]
        public string month;
        [JsonProperty("day")]
        public string day;
        [JsonProperty("year")]
        public string year;
        [JsonProperty("link")]
        public string link;
        [JsonProperty("news")]
        public string news;

        [JsonConstructor]
        public xkcdComic()
        {

        }
    }
}
