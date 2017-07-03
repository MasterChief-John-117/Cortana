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

        public Configuration()
        {
            Configuration config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("files/config.json"));
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.ExecEdits = config.ExecEdits; 
        }

        public Configuration UpdateToken()
        {
            Console.WriteLine($"There's an issue with your token! Please put an updated one here: ");
            this.Token = Console.ReadLine().Trim(' ', '"');
            File.WriteAllText("files/config.json", JsonConvert.SerializeObject(this, Formatting.Indented));
            return this;
        }
    }
}
