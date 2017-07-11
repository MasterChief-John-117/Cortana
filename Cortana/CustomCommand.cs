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
        public List<string> Aliases = new List<string>(new string[]{""});
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("delete")]
        public bool Delete= true;

        public CustomCommand(string c, string v)
        {
            this.Command = c.ToLower().Trim();
            this.Value = v;
        }

        [JsonConstructor]
        public CustomCommand(string c, List<string> a, string v, bool d)
        {
            this.Command = c;
            this.Aliases = a;
            this.Value = v;
            this.Delete = d;
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