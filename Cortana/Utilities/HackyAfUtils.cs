using System;
using System.Collections.Generic;
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
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("authorization", config.Token);
          
                var result = await client.PostAsync($"https://discordapp.com/api/channels/{context.Channel.Id}/messages/{context.Message.Id}/ack",
                    new StringContent("{}", Encoding.UTF8, "application/json"));

                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"One role mention by {context.User} in {context.Guild.Name} sucessfully killed!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public string SanitizeFileName(string fileName, char replacementChar = '_')
        {
            var blackList = new HashSet<char>(System.IO.Path.GetInvalidFileNameChars());
            var output = fileName.ToCharArray();
            for (int i = 0, ln = output.Length; i < ln; i++)
            {
                if (blackList.Contains(output[i]))
                {
                    output[i] = replacementChar;
                }
            }
            return new String(output);
        }
    }
}