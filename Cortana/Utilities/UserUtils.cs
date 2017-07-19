using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace Cortana.Utilities
{
    public class UserUtils
    {
        public List<string> FindUserFromString(DiscordSocketClient client, string user)
        {
            Stopwatch Stopwatch = new Stopwatch();
            Stopwatch.Restart();
            int max = 30;

            IEnumerable<SocketGuildUser> allusers = client.Guilds.SelectMany(x => x.Users);
            List<ulong> foundIds = new List<ulong>();
            string foundNames = $"Usernames matching `{user.ToLower()}`\n";
            string foundNicks = "Matching Nicknames\n";


            foreach (SocketGuild guild in client.Guilds)
            {
                foreach (SocketGuildUser u in guild.Users.Where(us => !foundIds.Contains(us.Id)))
                {
                    if (!foundIds.Contains(u.Id) && !String.IsNullOrEmpty(u.Username) && Regex.IsMatch(u.Username, user, RegexOptions.IgnoreCase))
                    {
                        foundNames += $"{u.Username} : `{u.Id}`\n";
                        foundIds.Add(u.Id);
                    }
                    if(!String.IsNullOrEmpty(u.Nickname) && Regex.IsMatch(u.Nickname, user, RegexOptions.IgnoreCase))
                    {
                        foundNicks += $"{u.Nickname} ({u.Username}) : `{u.Id}`\n";
                        foundIds.Add(u.Id);    
                    }
                }
            }

            if (foundNames.Length > 2000 || foundNicks.Length > 2000)
                return new List<string>{$"I found `{foundIds.Count}` users :sweat_smile: Try again with more strict parameters (Search took `{Stopwatch.ElapsedMilliseconds}`ms)"};
            else if (foundIds.Count == 0)
                return new List<string>{$"It took me `{Stopwatch.ElapsedMilliseconds}`ms to find no one :("};
            else
            {
                return new List<string>{foundNames, foundNicks, $"(Search took `{Stopwatch.ElapsedMilliseconds}`ms)"};
            }
        }
    }
}