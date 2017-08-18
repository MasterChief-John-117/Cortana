using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cortana.Utilities
{
    public class ChannelArchiver
    {
        public async Task FullArchive(ulong channelId, ulong originChannel)
        {
            var dsClient = Program.Client;

            var channel = (dsClient as IDiscordClient).GetChannelAsync(channelId).Result;
            var msgs = (channel as IMessageChannel).GetMessagesAsync().Flatten().Result;
            await Task.Yield();

            while (true)
            {
                var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result;
                msgs = msgs.Concat(newmsgs);
                if (newmsgs.Count() < 100) break;
            }

            string str = $"{(channel as IGuildChannel).Guild.Name} | {(channel as IGuildChannel).Guild.Id}\n";
            str += $"{channel.Name} | {channel.Id}\n";
            str += $"{DateTime.Now}\n\n";
            IMessage lastMsg = null;
            foreach (var msg in msgs.Reverse())
            {
                string msgstr = "";
                if(lastMsg != null && msg.Author.Id != lastMsg.Author.Id) msgstr += $"{msg.Author} | {msg.Author.Id}\n";
                if (lastMsg != null && msg.Author.Id != lastMsg.Author.Id) msgstr += $"{msg.Timestamp}\n";
                msgstr += $"{msg.Content}\n";
                foreach (var a in msg.Attachments)
                {
                    msgstr += $"{a.Url}\n";
                }
                str += msgstr + "\n";
                lastMsg = msg;
            }
            string filename = $"{channel.Name}.txt";
            File.WriteAllText("files/" + filename, str);
            await ((dsClient as IDiscordClient).GetChannelAsync(originChannel).Result as IMessageChannel).SendFileAsync("files/" + filename, $"Here you go! I saved {msgs.Count()} messages");
            File.Delete("files/" + filename);
            msgs.ToList().Clear();
            Console.WriteLine($"Done archiving #{channel.Name}! Check the channel you used the command in for the uploaded file.");
        }
    }
}
