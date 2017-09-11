using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Cortana.Utilities;
using System;
using System.Linq;
using Discord;
using Microsoft.SqlServer.Server;

namespace Cortana.Modules
{
    public class HackyAFModule : ModuleBase
    {
        [Command("suppressRoleMentions")]
        [Summary("Supresses role mentions for a given guild")]
        [Remarks("no-help")]
        public async Task AddGuildRoleSuppression(ulong guildId)
        {
            var guild = Context.Client.GetGuildAsync(guildId).Result;

            if (!CommandHandler._config.GuildSuppressionList.Contains(guild.Id))
            {
                CommandHandler._config.GuildSuppressionList.Add(guild.Id);
            }
            File.WriteAllText("files/config.json", JsonConvert.SerializeObject(CommandHandler._config, Formatting.Indented));
            await ReplyAsync($"{guild.Name} has been added to the Role Mention Suppression list");
        }
        [Command("removesuppressRoleMentions")]
        [Summary("un-suprresses role mentions for a given guild")]
        [Remarks("no-help")]
        public async Task RemoveGuildRoleSuppression(ulong guildId)
        {
            var guild = Context.Client.GetGuildAsync(guildId).Result;

            if (CommandHandler._config.GuildSuppressionList.Contains(guild.Id))
            {
                CommandHandler._config.GuildSuppressionList.Remove(guild.Id);
            }
            File.WriteAllText("files/config.json", JsonConvert.SerializeObject(CommandHandler._config, Formatting.Indented));
            await ReplyAsync($"{guild.Name} has been removed from the Role Mention Suppression list");
        }

        [Command("emoteservers")]
        public async Task EmoteServers()
        {
            string emotes = "Guilds with Emotes:\n";
            string normals = "Guilds without emotes:\n";
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                if (guild.Emotes.Any(e => e.IsManaged))
                    emotes += $"{guild.Name}: {guild.Emotes.Count(e => e.IsManaged)}\n";
                else normals += $"{guild.Name}\n";
            }
            await ReplyAsync(emotes);
            await ReplyAsync(normals);
        }
    }
}
