using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using Cortana.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MarkVSharp;
using System.Net;

namespace Cortana.Modules
{
    public class ChatModule : ModuleBase
    {
        [Command("clear")]
        [Alias("c")]
        [Summary("Delete all messages by the user in the last `n` (defaults to 100) messages")]
        public async Task Utility_Clear(int count = 99)
        {
            new MassDeleter().Individual(Context.Channel.Id, count).FireAndForget();
            var msg = await ReplyAsync($"Starting to delete a bunch of messages! Check the console window to see when this is done!\n_\\*This message will self-destruct in 2.5 seconds*_");
            await Task.Delay(2000);
            await msg.ModifyAsync(m => m.Content = "**BOOM!**");
            await Task.Delay(500);
            await msg.DeleteAsync();
        }

        [Command("reply")]
        [Summary("Quotes a message in the last 1000 messages in the channel given the ID, and then allows you to reply to it")]
        public async Task ReplyToMessage(ulong messageID, [Remainder] string reply)
        {
            await Context.Message.DeleteAsync();
            string finalMessage = "";
            var channel = Context.Channel as IMessageChannel;

            var msgs = (channel as IMessageChannel).GetMessagesAsync().Flatten().Result;

            for (int i = 0; i < 10; i++)
            {
                var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result;
                msgs = msgs.Concat(newmsgs);
                if (msgs.Any(m => m.Id.Equals(messageID))) break;
                else
                {
                    await ReplyAsync("The requested message could not be found");
                    return;
                }
            }
            if (!msgs.Any(m => m.Id.Equals(messageID)))
            {            
                await ReplyAsync("The requested message could not be found");
                return;
            }

            var replyToMessage = msgs.First(m => m.Id.Equals(messageID));

            new WebClient().DownloadFile(new Uri(replyToMessage.Author.GetAvatarUrl()), "files/tempImg.png");

            System.Drawing.Color clr;

            using (Bitmap bitmap = new Bitmap("files/tempImg.png"))
            {
                clr = new ImageUtils().GetAverageColor(bitmap, new CommandHandler(), 10);
            }

            var emb = new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(replyToMessage.Author.GetAvatarUrl())
                .WithName(replyToMessage.Author.ToString()))
            .WithDescription(replyToMessage.Content)
            .WithColor(new Discord.Color(clr.R, clr.G, clr.B))
            .WithFooter(new EmbedFooterBuilder().WithText(replyToMessage.CreatedAt.ToString()));

            await ReplyAsync("", embed: emb.Build());
            await ReplyAsync(reply);
        }

        [Command("markov")]
        [Summary("Markov chain generator for the channel\nOptions: [messages to grab] {minimum word count}")]
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
        [Summary("Saves all messages in a text channel to a file for download")]
        public async Task GetMessages(ulong channelId = 0)
        {
            if (channelId == 0)
            {
                channelId = Context.Channel.Id;
            }
            var channel = Context.Client.GetChannelAsync(channelId).Result;

            new ChannelArchiver().FullArchive(channelId, Context.Channel.Id).FireAndForget();

            await ReplyAsync($"Starting to archive messages from #{channel.Name}! \n" +
                $"This might take some time, check back in a bit to see when it's done");
        }

        [Command("archiveimages")]
        public async Task GetImages(ulong channelId = 0)
        {
            if (channelId == 0)
            {
                channelId = Context.Channel.Id;
            }
            var channel = Context.Client.GetChannelAsync(channelId).Result;
            
            var msgs = (channel as IMessageChannel).GetMessagesAsync().Flatten().Result;

            for(int i = 0; i < 10; i++)
            {
                var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result;
                msgs = msgs.Concat(newmsgs);
                if (newmsgs.Count() < 100) break;
            }

            int ats = 0;

            Directory.CreateDirectory($"files/{channel.Name}");
            using (var client = new WebClient())
            {
                foreach (var msg in msgs.Where(m => m.Attachments.Any()))
                {
                    foreach (var a in msg.Attachments)
                    {
                        ats++;
                        client.DownloadFile(a.Url, $"files/{channel.Name}/{a.Filename}");
                    }
                }
            }
            
            await ReplyAsync($"Downloaded {ats} files from {msgs.Count(m => m.Attachments.Any())} messages");
        }
        
        [Command("pingRole")]
        [Alias("mention")]
        [Summary("Allows you to mention a non-pingable role if you have the needed permissions")]
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
        [Summary("repeats the last message in chat")]
        public async Task RepeatLastMessage()
        {
            await Context.Message.DeleteAsync();
            var msg = Context.Channel.GetMessagesAsync(1).Flatten().Result.First();
            await ReplyAsync(msg.Content);
        }
    }
}