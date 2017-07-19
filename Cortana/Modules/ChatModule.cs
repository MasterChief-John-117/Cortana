using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cortana.Modules
{
    public class ChatModule : ModuleBase
    {
        [Command("clear")]
        [Alias("c")]
        public async Task Utility_Clear(int count = 99)
        {
            if (count < 1)
                return;

            int limit = (count < 99) ? count : 99;

            var enumerable = (await Context.Channel.GetMessagesAsync(limit: limit + 1).Flatten())
                .Where(m => m.Author.Id == Context.Client.CurrentUser.Id);
            
            foreach (IMessage m in enumerable)
            {
                await m.DeleteAsync();
            }
            var msg = await ReplyAsync($"Deleted `{enumerable.Count()}` messages!\n_\\*This message will self-destruct in 5 seconds*_");
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }
    }
}