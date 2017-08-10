using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cortana.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            
            var markovGenerator = new MarkovGenerator(Context.Channel.GetMessagesAsync(fetch).Flatten().Result.Reverse().Select(msg => msg.Content).Aggregate((i, j) => i + ". " + j));
            //var markovGenerator = new MarkovGenerator(new ChannelUtils().GetMessagesHugeAsync(Context.Channel, fetch).Result.Select(msg => msg.Content).Aggregate((i, j) => i + " " + j));

            await ReplyAsync(markovGenerator.GenerateSentence(minLength));
        }
        [Command("archive")]
        public async Task GetMessages(ulong channelId = 0)
        {
            if (channelId == 0)
            {
                channelId = Context.Channel.Id;
            }
            var channel = Context.Client.GetChannelAsync(channelId).Result;
            var msgs = (channel as IMessageChannel).GetMessagesAsync().Flatten().Result;

            while (true)
            {
                var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result;
                msgs = msgs.Concat(newmsgs);
                Console.WriteLine(msgs.Count());
                if(newmsgs.Count() < 100) break;
            }

            string str = $"{(channel as IGuildChannel).Guild.Name} | {(channel as IGuildChannel).Guild.Id}\n";
            str += $"{channel.Name} | {channel.Id}\n";
            str += $"{DateTime.Now}\n\n";    
            foreach (var msg in msgs.Reverse())
            {
                string msgstr = "";
                msgstr += $"{msg.Author} | {msg.Author.Id}\n";
                msgstr += $"{msg.Timestamp}\n";
                msgstr += $"{msg.Content}\n";
                foreach (var a in msg.Attachments)
                {
                    msgstr += $"{a.Url}\n";
                }
                str += msgstr + "\n";
            }
            string filename = $"{channel.Name}.txt";
            File.WriteAllText("files/" + filename, str);
            await Context.Channel.SendFileAsync("files/" + filename, $"Here you go! I saved {msgs.Count()} messages");
            File.Delete("files/" + filename);
            msgs.ToList().Clear();
        }
        [Command("pingRole")][Alias("mention")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Server_Ping(string roleName, [Remainder] string message)
        {
            try
            {
                await Context.Message.DeleteAsync();
                var role = (Context.Guild as SocketGuild).Roles.First(
                    r => r.Name.ToLower().Contains(roleName.ToLower()));
                bool original = role.IsMentionable;
                await role.ModifyAsync(r => r.Mentionable = true);
                await ReplyAsync(message.Replace(roleName, role.Mention));
                await role.ModifyAsync(r => r.Mentionable = original);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("repeat")]
        [Alias("re")]
        public async Task RepeatLastMessage()
        {
            await Context.Message.DeleteAsync();
            var msg = Context.Channel.GetMessagesAsync(1).Flatten().Result.First();
            await ReplyAsync(msg.Content);
        }
    }
}