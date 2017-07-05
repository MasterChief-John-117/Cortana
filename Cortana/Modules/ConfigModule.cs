using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;

namespace Cortana.Modules
{
    [Group("config")]
    public class ConfigModule : ModuleBase
    {
        [Command("prefix")][Summary("Change the prefix used")][Remarks("config")]
        public async Task Prefix(string pref)
        {
            CommandHandler._config.Prefix = pref;
            File.WriteAllText("files/config.json", JsonConvert.SerializeObject(CommandHandler._config, Formatting.Indented));
            await ReplyAsync($"Your prefix is now `{CommandHandler._config.Prefix}`");
        }
        
        [Command("executeEditedMessages")][Summary("Choose whether or not edited messages are treated as commands")][Remarks("config")]
        public async Task Config_ExecuteEditedMessages(string option)
        {
            if (option.ToLower().Equals("false"))
            {
                CommandHandler._config.ExecEdits = false;
                File.WriteAllText("files/config.json", JsonConvert.SerializeObject(CommandHandler._config, Formatting.Indented));
                await ReplyAsync("Edited messages will not be treated as commands");
            }
            else if (option.ToLower().Equals("true"))
            {
                CommandHandler._config.ExecEdits = true;
                File.WriteAllText("files/config.json", JsonConvert.SerializeObject(CommandHandler._config, Formatting.Indented));
                await ReplyAsync("Edited messages will now be executed as commands");
            }
            else await ReplyAsync("unknown parameter");
        }
    }
}