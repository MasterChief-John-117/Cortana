using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cortana.Modules
{
    public class HelpModule : ModuleBase
    {
        [Command("halp")][Alias("help")]
        public async Task Halp(string command = "")    
        {
            var commandList = CommandHandler._commands.Commands.Where(cmd => Regex.IsMatch(cmd.Name, command, RegexOptions.IgnoreCase)).Where(cmd => string.IsNullOrEmpty(cmd.Remarks)
                                                                                                                                                           || !cmd.Remarks.Contains("no-help"));
            EmbedBuilder em = new EmbedBuilder();
            em.WithTitle(string.IsNullOrEmpty(command) ? "Commands" : $"Commands matching {command} ({commandList.Count()})");
            if(commandList.Count() > 5) em.WithDescription(string.Join(", ", commandList.Select(cmd => cmd.Name).ToList()));
            else
            {
                em.WithDescription(string.Join("\n", commandList.Select(cmd => cmd.Name + ": " + (!string.IsNullOrEmpty(cmd.Summary) ? cmd.Summary : "No Summary")).ToArray()));
            }
            await ReplyAsync("", embed:em);
        }
    }
}