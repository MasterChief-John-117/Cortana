using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cortana
{
    public class CustomCommand
    {
        public string Command;
        public List<string> Aliases = new List<string>();
        public string Value;
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