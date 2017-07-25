using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cortana.Utilities;
using Discord;
using Discord.Commands;
using MarkVSharp;

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
            var msg = await ReplyAsync($"Deleted `{enumerable.Count() - 1}` messages!\n_\\*This message will self-destruct in 5 seconds*_");
            await Task.Delay(4000);
            await msg.ModifyAsync(m => m.Content = "**BOOM!**");
            await Task.Delay(1000);
            await msg.DeleteAsync();
        }

        [Command("markov")][Summary("Markov chain generator for the channel\nOptions: [messages to grab] {minimum word count}")]
        public async Task ChatMarkov([Remainder] string options = "[300] {5}")
        {
            await Context.Message.DeleteAsync();
            int fetch = (Regex.IsMatch(options, @"\[(.+?)\]")) ? Convert.ToInt32(Regex.Matches(options, @"\[(.+?)\]")[0].Groups[1].Value) : 100;
            int minLength = (Regex.IsMatch(options, @"\{(.+?)\}")) ? Convert.ToInt32(Regex.Matches(options, @"\{(.+?)\}")[0].Groups[1].Value) : 5;
            
            var markovGenerator = new MarkovGenerator(Context.Channel.GetMessagesAsync(fetch).Flatten().Result.Reverse().Select(msg => msg.Content).Aggregate((i, j) => i + " " + j));

            await ReplyAsync(markovGenerator.GenerateSentence(minLength));
        }
    }
}