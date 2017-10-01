using System;
using System.Collections.Generic;
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
                if(before.Value.Content != after.Content)Console.WriteLine($"Message Edited --------------------------------------{DateTime.Now}\n" + message);
                var emb = new EmbedBuilder()
                    .WithTitle("Message Edited")
                    .WithAuthor(new EmbedAuthorBuilder().WithName($"{after.Author} ({after.Author.Id})")
                        .WithIconUrl(after.Author.GetAvatarUrl()))
                    .WithColor(0, 125, 0)
                    .WithCurrentTimestamp()
                    .WithFooter(after.Id.ToString());


                emb.AddField(new EmbedFieldBuilder().WithName("Channel").WithValue(after.Channel.Name).WithIsInline(true));
                emb.AddField(new EmbedFieldBuilder().WithName("Before").WithValue(before.Value.Content));
                emb.AddField(new EmbedFieldBuilder().WithName("After").WithValue(after.Content));
                if (before.Value.Content == after.Content) return;
                await Program.WHClient.SendMessageAsync("", embeds: new List<Embed> {emb.Build()}.ToArray());
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
                if(before.Value.Content != after.Content)Console.WriteLine($"Message Edited --------------------------------------{DateTime.Now}\n" + message);

                var emb = new EmbedBuilder()
                    .WithTitle("Message Edited")
                    .WithAuthor(new EmbedAuthorBuilder().WithName($"{after.Author} ({after.Author.Id})")
                        .WithIconUrl(after.Author.GetAvatarUrl()))
                    .WithColor(0, 125, 0)
                    .WithCurrentTimestamp()
                    .WithFooter(after.Id.ToString());


                emb.AddField(new EmbedFieldBuilder().WithName("Guild").WithValue((after.Channel as SocketGuildChannel).Guild.Name).WithIsInline(true));
                emb.AddField(new EmbedFieldBuilder().WithName("Channel").WithValue("#" + after.Channel.Name).WithIsInline(true));
                emb.AddField(new EmbedFieldBuilder().WithName("Before").WithValue(before.Value.Content));
                emb.AddField(new EmbedFieldBuilder().WithName("After").WithValue(after.Content));

                if (before.Value.Content == after.Content) return;
                await Program.WHClient.SendMessageAsync("", embeds: new List<Embed> {emb.Build()}.ToArray());


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
                string message = $"Message Deleted ---------------------------------{DateTime.Now}\n";
                try{message += $"Author: {msg.Value.Author}";}catch (Exception ex){}
                try{message += $"({msg.Value.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Channel: {msg.Value.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Message: {msg.Value.Content}";}catch (Exception ex){return;}
                if(!string.IsNullOrEmpty(message))Console.WriteLine(message);

                var emb = new EmbedBuilder()
                    .WithTitle("Message Deleted")
                    .WithAuthor(new EmbedAuthorBuilder().WithName($"{msg.Value.Author} ({msg.Value.Author.Id})")
                    .WithIconUrl(msg.Value.Author.GetAvatarUrl()))
                    .WithColor(255, 0, 0)
                    .WithCurrentTimestamp()
                    .WithFooter(msg.Value.Id.ToString());

                emb.AddField(new EmbedFieldBuilder().WithName("DM Channel").WithValue(msg.Value.Channel.Name));
                emb.AddField(new EmbedFieldBuilder().WithName("Message").WithValue(msg.Value.Content));

                await Program.WHClient.SendMessageAsync("", embeds: new List<Embed> {emb.Build()}.ToArray());
            }
            try
            {
                if (!CommandHandler._config.LogServers.Contains((msg.Value.Channel as SocketGuildChannel).Guild.Id)) return;

                string message = $"Message Deleted ---------------------------------{DateTime.Now}\n";
                try{message += $"Author: {msg.Value.Author}";}catch (Exception ex){}
                try{message += $"({msg.Value.Author.Id})\n";}catch (Exception ex){}
                try{message += $"Server: {(msg.Value.Channel as SocketGuildChannel).Guild.Name} ";}catch (Exception ex){}
                try{message += $"Channel: #{msg.Value.Channel.Name}\n";}catch (Exception ex){}
                try{message += $"Message: {msg.Value.Content}";}catch (Exception ex){return;}
                Console.WriteLine(message);

                var emb = new EmbedBuilder()
                    .WithTitle("Message Deleted")
                    .WithAuthor(new EmbedAuthorBuilder().WithName($"{msg.Value.Author} ({msg.Value.Author.Id})")
                        .WithIconUrl(msg.Value.Author.GetAvatarUrl()))
                    .WithColor(255, 0, 0)
                    .WithCurrentTimestamp()
                    .WithFooter(msg.Value.Id.ToString());


                emb.AddField(new EmbedFieldBuilder().WithName("Guild").WithValue((msg.Value.Channel as SocketGuildChannel).Guild.Name).WithIsInline(true));
                emb.AddField(new EmbedFieldBuilder().WithName("Channel").WithValue("#" + msg.Value.Channel.Name).WithIsInline(true));
                emb.AddField(new EmbedFieldBuilder().WithName("Message").WithValue(msg.Value.Content));

                await Program.WHClient.SendMessageAsync("", embeds: new List<Embed> {emb.Build()}.ToArray());


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
