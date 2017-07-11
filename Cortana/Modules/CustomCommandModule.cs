using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace Cortana.Modules
{
    [Group("commands")]
    [Alias("command")]
    public class CustomCommandModule : ModuleBase
    {
        [Command("add")]
        [Summary("Add a new custom command")]
        [Remarks("commands")]
        public async Task addCommand(string name, [Remainder] string text)
        {
            if (CommandHandler.CustomCommands.Any(c => c.Command == name.ToLower()) ||
                CommandHandler.CustomCommands.Any(c => c.Aliases.Any(a => a == name.ToLower())))
            {
                await ReplyAsync($"There already exists a command with the name or alias `{name}`");
                return;
            }
            var cmd = new CustomCommand(name, text);
            CommandHandler.CustomCommands.Add(cmd);

            var em = new EmbedBuilder();
            em.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(cmd.Command).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Aliases").WithValue(cmd.GetAliases()).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Value").WithValue(cmd.Value).WithIsInline(false));
            em.AddField(new EmbedFieldBuilder().WithName("Delete").WithValue(cmd.Delete).WithIsInline(true));
            File.WriteAllText("files/customCommands.json", JsonConvert.SerializeObject(CommandHandler.CustomCommands, Formatting.Indented));

            await ReplyAsync($"The command `{cmd.Command}` has been created!", embed: em.Build());
        }
        [Command("remove")][Alias("delete")]
        [Summary("removes a custom command")]
        [Remarks("commands")]
        public async Task RemoveCommand(string name)
        {
            if (CommandHandler.CustomCommands.All(c => c.Command != name.ToLower()))
            {
                await ReplyAsync($"There is no command named `{name}`");
                return;
            }
            var cmd = CommandHandler.CustomCommands.First(c => c.Command.Equals(name.ToLower()));
            CommandHandler.CustomCommands.Remove(cmd);
            
            File.WriteAllText("files/customCommands.json", JsonConvert.SerializeObject(CommandHandler.CustomCommands, Formatting.Indented));

            await ReplyAsync($"The command `{name}`has been removed");
        }
        [Command("list")]
        public async Task Custom_List()
        {
            string msg = "Custom Commands: \n```\n";
            int i = 1;
            foreach (var command in CommandHandler.CustomCommands)
            {
                if (i % 4 == 0 && i != 0) msg += command.Command + "\n";
                else
                {
                    msg += command.Command;
                    for (int j = command.Command.Length; j < 15; j++) msg += " ";
                }
                i++;
                if (msg.Length > 1500)
                {
                    msg += $"{command.Command}\n```";
                    await ReplyAsync(msg);
                    msg = "```\n";
                }
            }
            await ReplyAsync(msg + "\n```");
            await Context.Message.DeleteAsync();
        }

    }
}