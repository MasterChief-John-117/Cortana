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
        }
    }
}