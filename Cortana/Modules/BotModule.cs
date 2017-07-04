﻿using System;
using System.Diagnostics;
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
    }
}