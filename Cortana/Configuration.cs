using System;
using System.IO;
using Discord;
using Newtonsoft.Json;

namespace Cortana
{
    class Configuration
    {
        [JsonProperty("token")]
        public string Token;
        [JsonProperty("tokenType")]
        public TokenType TokenType = TokenType.User;
        [JsonProperty("prefix")]
        public string Prefix;
        [JsonProperty("executeEdits")]
        public bool execEdits;

        public Configuration(string token, TokenType tType, string prefix, bool edits)
        {
            this.Token = token;
            this.TokenType = tType;
            this.Prefix = prefix;
            this.execEdits = edits;
        }

        public Configuration()
        {
            Configuration config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("files/config.json"));
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.execEdits = config.execEdits;
        }


    }
}
