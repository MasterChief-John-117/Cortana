using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cortana.Utilities;
using Discord.Commands;

namespace Cortana.Modules
{
    public class UserModule : ModuleBase
    {
        [Command("findUser")]
        [Alias("find", "f")]
        public async Task Utility_FindUser([Remainder] string user)
        {
            var result = new UserUtils().FindUserFromString(Program.Client, user);
            if (result.Sum(m => m.Length) < 1950)
            {
                string delim = "\n";
                await Context.Message.ModifyAsync(msg => msg.Content = (result.Aggregate((i, j) => i + delim + j)));
            }
            else
            {
                await Context.Message.ModifyAsync(msg => msg.Content = (result.First()));
                result.RemoveAt(0);
                foreach (var str in result)
                {
                    await ReplyAsync(str);
                }
            }
        }
    }
}