using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cortana.Modules
{
    public class BotModule : ModuleBase
    {
        [Command("ping")][Summary("Gets the time to Discord's servers")]
        public async Task Ping()
        {
            var rand = new Random();
            var watch = Stopwatch.StartNew();
            await Context.Message.ModifyAsync(msg => msg.Content = "͏");
            watch.Stop();
            await Context.Message.ModifyAsync(msg => msg.Embed = new EmbedBuilder()
                .WithDescription($":ping_pong: Ping took `{watch.ElapsedMilliseconds}`ms")
                .WithColor(new Color(rand.Next(255), rand.Next(255), rand.Next(255)))
                .Build());
        }

        [Command("info")]
        [Summary("Displays information about this bot")]
        public async Task Info()
        {
            await Context.Message.DeleteAsync();
            
            var em = new EmbedBuilder();
            em.WithTitle("Info About you and the Cortana Self-Bot");
            em.WithAuthor(new EmbedAuthorBuilder().WithName("Bot Author: MasterChief_John-117").WithIconUrl("https://mcjohn117.duckdns.org/images/MCUSFlags.jpg"));
            em.WithColor(new Color(41, 82, 145));
            em.WithImageUrl(Context.User.GetAvatarUrl());
            em.WithCurrentTimestamp();

            em.AddField(new EmbedFieldBuilder().WithName("Guilds")
                .WithValue(Context.Client.GetGuildsAsync().Result.Count).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Channels")
                .WithValue(Context.Client.GetGuildsAsync().Result.Sum(g => g.GetChannelsAsync().Result.Count)).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Users")
                .WithValue(Context.Client.GetGuildsAsync().Result.Sum(g => g.GetUsersAsync().Result.Count)).WithIsInline(true));

            em.AddField(new EmbedFieldBuilder().WithName("Text Channels")
                .WithValue(Context.Client.GetGuildsAsync().Result.Sum(g => g.GetTextChannelsAsync().Result.Count)).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("NSFW Channels")
                .WithValue(Context.Client.GetGuildsAsync()
                .Result.Sum(g => g.GetChannelsAsync().Result.Count(c => c.IsNsfw))).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Open DMs")
                .WithValue(Context.Client.GetPrivateChannelsAsync().Result.Count).WithIsInline(true));
            
            em.AddField(new EmbedFieldBuilder().WithName("Emotes")
                .WithValue(Context.Client.GetGuildsAsync().Result.Sum(g => g.Emotes.Count)).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Global Emotes")
                .WithValue(Context.Client.GetGuildsAsync().Result.Sum(g => g.Emotes.Count(e => e.IsManaged))).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Emote Servers")
                .WithValue(Context.Client.GetGuildsAsync().Result.Count(g => g.Emotes.Any(e => e.IsManaged))).WithIsInline(true));
            
            em.AddField(new EmbedFieldBuilder().WithName("Guilds Owned")
                .WithValue(Context.Client.GetGuildsAsync().Result.Count(g => g.OwnerId == Context.User.Id)).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Verified Email")
                .WithValue(Context.Client.CurrentUser.IsVerified).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("2FA Enabled")
                .WithValue(Context.Client.CurrentUser.IsMfaEnabled).WithIsInline(true));
            
            
            
            em.AddField(new EmbedFieldBuilder().WithName("Memory")
                .WithValue($"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)} MB").WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("Uptime")
                .WithValue((DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")).WithIsInline(true));
            em.AddField(new EmbedFieldBuilder().WithName("API Library")
                .WithValue($"Discord.Net {DiscordConfig.Version.Substring(0, DiscordConfig.Version.Length - 6)}").WithIsInline(true));
            
            
            await ReplyAsync("", embed: em);
        }

        [Command("cleanup")]
        public async Task Cleanup1()
        {
            GC.Collect();
        }
    }
}