using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;
using Cortana.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
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

        [Command("test")][Remarks("no-help")]
        public async Task Rhea( string discrim)
        {
            string res = "";
            foreach (var u in Context.Client.GetGuildsAsync().Result.SelectMany(g => g.GetUsersAsync().Result).Where(u => u.Discriminator == discrim).Select(u => u.Id).Distinct().Take(25))
            {
                res += $"{Context.Client.GetUserAsync(u).Result.Username}\n";
            }
            await ReplyAsync(res);
        }

        [Command("massban")]
        [Summary("Ban a lot of people. Usage: ID0 ID1 ID2 (etc.) | reason")]
        public async Task MassBan([Remainder] string input)
        {

            await Context.Message.DeleteAsync();
            string idsFromInput = input.Split('|')[0].Trim();
            string reason = input.Split('|')[1].Trim();
            List<ulong> ids = new List<ulong>();

            foreach (var id in idsFromInput.Split())
            {
                ids.Add(Convert.ToUInt64(id));
            }
            ids.Sort();
            foreach (var id in ids)
            {
                await Context.Guild.AddBanAsync(id, 0, reason);
            }
            var bans = Context.Guild.GetBansAsync().Result;
            foreach (var id in ids)
            {
                var user = (bans.First(b => b.User.Id == id).User);
                string banMessage = "";
                try
                {
                    banMessage += $"Banned `{user}`(`{user.Id}`)";
                }
                catch(Exception e)
                {
                    banMessage += $"Banned `{id}`";
                }
                if (!string.IsNullOrEmpty(reason))
                {
                    banMessage += $" (Reason: `{reason}`)";
                }
                await ReplyAsync(banMessage);
            }
        }
    }
}
