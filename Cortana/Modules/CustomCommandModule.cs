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
        public async Task Custom_Commands(string name, [Remainder] string text)
        {
            if (CommandHandler.CustomCommands.Any(c => c.Command == name.ToLower()) ||
                CommandHandler.CustomCommands.Any(c => c.Aliases.Any(a => a == name.ToLower())))
            {
                await ReplyAsync($"There already exists a command with the name or alias `{name}`");
                return;
            }
            var cmd = new CustomCommand(name, text);
            CommandHandler.CustomCommands.Add(cmd);

            var em = new EmbedBuilder().WithTitle("A new command has been added!").WithDescription("");
            em.AddField(new EmbedFieldBuilder().WithName("Name").WithValue(cmd.Command).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Aliases").WithValue(cmd.GetAliases()).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Value").WithValue(cmd.Value).WithIsInline(false));
            em.AddField(new EmbedFieldBuilder().WithName("Delete").WithValue(cmd.Delete).WithIsInline(true));
            File.WriteAllText("files/customCommands.json", JsonConvert.SerializeObject(CommandHandler.CustomCommands, Formatting.Indented));

            await ReplyAsync("", embed: em.Build());
        }
    }
}