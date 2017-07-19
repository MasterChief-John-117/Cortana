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
        [Command("help")]
        public async Task Help([Optional] string module)
        {
            if (!string.IsNullOrEmpty(module))
            {
                await GroupHelp(module);
                return; 
            }
            var em = new EmbedBuilder();
            em.WithAuthor(new EmbedAuthorBuilder().WithName($"General Help").WithIconUrl(Context.User.GetAvatarUrl()));
            foreach (var cmd in CommandHandler._commands.Commands.Where(cmd => string.IsNullOrEmpty(cmd.Remarks)))
            {
                em.AddField(new EmbedFieldBuilder().WithName(cmd.Name).WithValue(!string.IsNullOrEmpty(cmd.Summary) ? cmd.Summary : "No Summary"));
            }
            var cmgs = new List<string>();
            foreach (var cmg in CommandHandler._commands.Commands.Where(cmd => !string.IsNullOrEmpty(cmd.Remarks)))
            {
                try
                {
                    if(!cmgs.Contains(cmg.Remarks)) cmgs.Add(cmg.Remarks);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            foreach (var cmg in cmgs)
            {
                em.AddField(new EmbedFieldBuilder().WithName(cmg).WithValue($"This is a group. Do {CommandHandler._config.Prefix}help {cmg} for more info"));
            }
            
            em.WithCurrentTimestamp();
            try
            {
                var color = (Context.User as SocketGuildUser).Roles.OrderByDescending(r => r.Position)
                    .FirstOrDefault(r => r.Color.RawValue != new Color(0, 0, 0).RawValue).Color;

                if (color.R > 20 || color.G > 20 || color.B > 20)
                {
                    em.WithColor(color);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            await ReplyAsync("", embed: em.Build());
        }

        public async Task GroupHelp(string mod)
        {
            var em = new EmbedBuilder();
            try
            {
                var color = (Context.User as SocketGuildUser).Roles.OrderByDescending(r => r.Position)
                    .FirstOrDefault(r => r.Color.RawValue != new Color(0, 0, 0).RawValue).Color;

                if (color.R > 20 || color.G > 20 || color.B > 20)
                {
                    em.WithColor(color);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            em.WithAuthor(new EmbedAuthorBuilder().WithName($"Module Help").WithIconUrl(Context.User.GetAvatarUrl()));
            foreach(var cmg in CommandHandler._commands.Commands.Where(cmd => !string.IsNullOrEmpty(cmd.Remarks) && cmd.Remarks == mod.ToLower()))
            {
                em.AddField(new EmbedFieldBuilder().WithName(cmg.Name).WithValue(!string.IsNullOrEmpty(cmg.Summary) ? cmg.Summary : "No Summary"));
            }
            em.WithCurrentTimestamp();
            if (!CommandHandler._commands.Commands.Any(cmd =>
                !string.IsNullOrEmpty(cmd.Remarks) && cmd.Remarks == mod.ToLower()))
            {
                em.WithDescription($"The module `{mod}` was not found");
            }

            await ReplyAsync("", embed: em.Build());
        }
        [Command("halp")]
        public async Task Halp(string command = "")    
        {
            var commandList = CommandHandler._commands.Commands.Where(cmd => Regex.IsMatch(cmd.Name, command, RegexOptions.IgnoreCase));
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