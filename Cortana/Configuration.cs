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
        public TokenType TokenType;
        [JsonProperty("prefix")]
        public string Prefix;
        [JsonProperty("executeEdits")]
        public bool ExecEdits;

        [JsonConstructor]
        public Configuration(string token, TokenType tType, string prefix, bool edits)
        {
            this.Token = token;
            this.TokenType = tType;
            this.Prefix = prefix;
            this.ExecEdits = edits;
        }

        public Configuration(ConfigurationBuilder config)
        {
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.ExecEdits = config.execEdits;   
        }
        {
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.execEdits = config.execEdits;   
        }
    }
}
