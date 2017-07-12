using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Cortana.Utilities
{
    public class HackyAfUtils
    {
        public async Task Acknowledge(Configuration config, ICommandContext context)
        {
            try
            {
                Console.WriteLine($"https://discordapp.com/api/channels/{context.Channel.Id}/messages/{context.Message.Id}/ack");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", config.Token);
          
                var result = await client.PostAsync($"https://discordapp.com/api/channels/{context.Channel.Id}/messages/{context.Message.Id}/ack",
                    new StringContent("{}", Encoding.UTF8, "application/json"));
                Console.WriteLine($"Success? {result.IsSuccessStatusCode} Code: {result.StatusCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}