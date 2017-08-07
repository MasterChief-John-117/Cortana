using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace UrbanDictionary
{
    public class UrbanClientResponse
    {
        [JsonProperty("tags")]
        public List<string> Tags = new List<string>();
        [JsonProperty("result_type")]
        public string ResultType;
        [JsonProperty("list")]
        public List<UrbanResult> List = new List<UrbanResult>();
        [JsonProperty("sounds")]
        public List<string> Sounds = new List<string>();

        [JsonConstructor]
        public UrbanClientResponse()
        {
            
        }

        public UrbanClientResponse(string word)
        {
            string responseString;
            using (var client = new WebClient())
            {
                responseString = client.DownloadStringTaskAsync(new Uri($"http://api.urbandictionary.com/v0/define?term={word}")).Result;
            }

            UrbanClientResponse tempResponse = JsonConvert.DeserializeObject<UrbanClientResponse>(responseString);

            Tags = tempResponse.Tags;
            ResultType = tempResponse.ResultType;
            List = tempResponse.List;
            Sounds = tempResponse.Sounds;
        }
    }
}