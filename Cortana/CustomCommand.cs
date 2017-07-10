using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cortana
{
    public class CustomCommand
    {
        [JsonProperty("name")]
        public string Command;
        [JsonProperty("aliases")]
        public List<string> Aliases = new List<string>();
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("delete")]
        public bool Delete= true;

        public CustomCommand(string c, string v)
        {
            Command = c.ToLower().Trim();
            Value = v;
        }

        public string GetAliases()
        {
            if (!Aliases.Any()) return "None";
            else
            {
                StringBuilder als = new StringBuilder();
                foreach (string str in Aliases)
                {
                    als.AppendLine(str);
                }
                return als.ToString();
            }
        }
    }
}