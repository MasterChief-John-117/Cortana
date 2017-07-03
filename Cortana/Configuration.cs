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
            /*
            Configuration config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("files/config.json"));
            this.Token = config.Token;
            this.TokenType = config.TokenType;
            this.Prefix = config.Prefix;
            this.execEdits = config.execEdits;
            */
        }

        public Configuration createConfiguration()
        {
            //needed variables
            TokenType tType;
            bool eEdits;
            
            Console.WriteLine($"You need to create a new configuration! Follow the below steps to begin");
            //token
            Console.WriteLine($"Enter the token you wish to use: ");
            string token = Console.ReadLine();
            //account type in case bot is needed 
            Console.WriteLine($"Enter the account type you are using (\"bot\"/\"user\")");
            string type = Console.ReadLine().ToLower();
            if(type == "bot") tType = TokenType.Bot;
            else if (type == "user") tType = TokenType.User;
            else
            {
                Console.WriteLine($"Your input of {type} could not be understood. The type has been set to the default of User");
                tType = TokenType.User;
            }
            //prefix
            Console.WriteLine($"Enter the prefix you wish to use: ");
            string prefix = Console.ReadLine();
            //execute edits?
            Console.WriteLine($"Do you want edited messages to be treated as commands? (y/n)");
            string edits = Console.ReadLine().ToLower();
            if (edits == "y") eEdits = true;
            else if (edits == "n") eEdits = false;
            else
            {
                Console.WriteLine($"Your input of {edits} could not be understood. Edits will be treated as commands");
                eEdits = true;
            } 
            
            return new Configuration(token, tType, prefix, eEdits);
        }

    }
}
