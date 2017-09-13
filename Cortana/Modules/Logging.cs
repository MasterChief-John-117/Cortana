using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Win32.SafeHandles;

namespace Cortana.Modules
{
    public class Logging
    {
        public async Task LogEditedMessage(Cacheable<IMessage, ulong> before, IMessage after, ISocketMessageChannel channel)
        {
            try
            {
                var guild =  ((after.Channel as IGuildChannel).Guild);
            }
            catch (NullReferenceException e)
            {
                string message = "";
                try{message += $"Author: {after.Author}";}catch (Exception ex){}
                try{message += $"({after.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Channel: {after.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Before: {before.Value.Content}\n";}catch (Exception ex){}
                try{message += $"After: {after.Content}";}catch (Exception ex){return;}
                if(!string.IsNullOrEmpty(message))Console.WriteLine("Message Edited ---------------------------------------------------\n" + message);

            }
            try
            {
                if (!CommandHandler._config.LogServers.Contains((after.Channel as SocketGuildChannel).Guild.Id)) return;

                if (string.IsNullOrEmpty(before.Value.Content) || string.IsNullOrEmpty(after.Content)) return;
                if (before.Value.Content == "?" || after.Content == "?") return;

                string message = "";
                try{message += $"Author: {after.Author}";}catch (Exception ex){}
                try{message += $"({after.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Server: {(after.Channel as SocketGuildChannel).Guild.Name} ";}catch (Exception ex){}
                try{message += $"Channel: #{after.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Before: {before.Value.Content}\n";}catch (Exception ex){}
                try{message += $"After: {after.Content}";}catch (Exception ex){return;}
                if(before.Value.Content != after.Content)Console.WriteLine("Message Edited ---------------------------------------------------\n" + message);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }

        public async Task LogDeletedMessage(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            try
            {
                var guild =  ((msg.Value.Channel as IGuildChannel).Guild);
            }
            catch (NullReferenceException e)
            {
                string message = "Message Deleted ---------------------------------------------------\n";
                try{message += $"Author: {msg.Value.Author}";}catch (Exception ex){}
                try{message += $"({msg.Value.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Channel: {msg.Value.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Message: {msg.Value.Content}";}catch (Exception ex){return;}
                if(!string.IsNullOrEmpty(message))Console.WriteLine(message);

            }
            try
            {
                if (!CommandHandler._config.LogServers.Contains((msg.Value.Channel as SocketGuildChannel).Guild.Id)) return;

                string message = "Message Deleted ---------------------------------------------------\n";
                try{message += $"Author: {msg.Value.Author}";}catch (Exception ex){}
                try{message += $"({msg.Value.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Server: {(msg.Value.Channel as SocketGuildChannel).Guild.Name} ";}catch (Exception ex){}
                try{message += $"Channel: #{msg.Value.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Message: {msg.Value.Content}";}catch (Exception ex){return;}
                Console.WriteLine(message);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }
    }
}
