using System;
using System.Collections.Generic;
using Discord;

namespace Cortana
{
    public class ConfigurationBuilder
    {
        public string Token;
        public TokenType TokenType = TokenType.User;
        public string Prefix;
        public bool execEdits;

        public ConfigurationBuilder()
        {
        }

        public ConfigurationBuilder(string token, TokenType tType, string prefix, bool edits)
        {
            this.Token = token;
            this.TokenType = tType;
            this.Prefix = prefix;
            this.execEdits = edits;
        }
        
        public ConfigurationBuilder createConfiguration()
        {
            //needed variables
            TokenType tType;
            bool eEdits;
            
            Console.WriteLine($"You need to create a new configuration! Follow the below steps to begin");
            //token
            Console.WriteLine($"Enter the token you wish to use: ");
            string token = Console.ReadLine().Trim(' ', '"');
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
            string prefix = Console.ReadLine().TrimStart();
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
            
            return new ConfigurationBuilder(token, tType, prefix, eEdits);
        }

    }
}