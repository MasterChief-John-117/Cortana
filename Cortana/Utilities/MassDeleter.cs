using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cortana.Utilities
{
    public class MassDeleter
    {
        public async Task Individual(ulong channelId, int count)
        {
            var dsClient = Program.Client;
            int grab = count > 100 ? 100 : count;
            var channel = (dsClient as IDiscordClient).GetChannelAsync(channelId).Result;
            var msgs = (channel as IMessageChannel).GetMessagesAsync(grab).Flatten().Result.Where(m => m.Author.Id == dsClient.CurrentUser.Id);
            await Task.Yield();

            while (count > 0)
            {
                grab = count > 100 ? 100 : count;
                var newmsgs = (channel as IMessageChannel).GetMessagesAsync(msgs.Last(), Direction.Before).Flatten().Result.Where(m => m.Author.Id == dsClient.CurrentUser.Id);
                msgs = msgs.Concat(newmsgs);
                if (newmsgs.Count() < 100 || msgs.Count() >= count) break;
                await Task.Delay(200);
            }

            foreach(var msg in msgs)
            {
                await msg.DeleteAsync();
                await Task.Delay(100);
            }

            Console.WriteLine($"Deleted {msgs.Count()} messages from {channel.Name} on {(channel as IGuildChannel).Guild.Name}");
        }
    }
}
