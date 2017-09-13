using System;
using System.Collections.Generic;
using System.IO;
using Discord;
using Newtonsoft.Json;

namespace Cortana
{
    public class Configuration
    {
        [JsonProperty("token")]
        public string Token;
        [JsonProperty("tokenType")]
        public TokenType TokenType;
        [JsonProperty("prefix")]
        public string Prefix;
        [JsonProperty("executeEdits")]
        public bool ExecEdits;
        [JsonProperty("guildToSuppressRoles")]
        public List<ulong> GuildSuppressionList;

        [JsonProperty("serversToLog")] public List<ulong> LogServers;
        [JsonProperty("file")] public bool DebugLogMode;

        [JsonConstructor]
        public Configuration(string token, TokenType tType, string prefix, bool edits)
        {
            this.Token = token;
            this.TokenType = tType;
            this.Prefix = prefix;
            this.ExecEdits = edits;
            GuildSuppressionList = new List<ulong>(0);
            LogServers = new List<ulong>(0);
            DebugLogMode = false;
        }

        public Configuration(ConfigurationBuilder config)
        {
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.ExecEdits = config.execEdits;
            GuildSuppressionList = new List<ulong>(0);
            LogServers = new List<ulong>(0);
            DebugLogMode = false;
        }

        public Configuration()
        {
            Configuration config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("files/config.json"));
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.ExecEdits = config.ExecEdits;
            this.GuildSuppressionList = config.GuildSuppressionList;
            this.LogServers = config.LogServers;
            this.DebugLogMode = config.DebugLogMode;
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
