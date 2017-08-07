using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cortana.Modules
{
    public class WebModule : ModuleBase
    {
        [Command("curl")]
        [Summary("Get a string from a URL")]
        public async Task Curl(string url)
        {
            using (var client = new WebClient())
            {
                await ReplyAsync($"{client.DownloadString(url)}");
            }
        }

        [Command("ud")]
        public async Task UrbanDefine([Remainder] string word)
        {
            await Context.Message.DeleteAsync();
            var urbanclient = new UrbanDictionary.UrbanClient();
            var urbanResponse = urbanclient.GetClientResponse(word);

            var em = new EmbedBuilder();    
                  
            em.WithThumbnailUrl("http://marcmarut.com/wp-content/uploads/2013/12/Urban-Dictionary-Icon3.png");            

            if (urbanResponse.ResultType.Equals("no_results"))
            {
                em.WithTitle(word);
                em.WithDescription("No matches found");
                em.WithColor(new Color(250, 0, 0));
            }
            else
            {
                var pos = new Random().Next(urbanResponse.List.Where(w => (w.ThumbsUp - w.ThumbsDown) > 0).ToList().Count);
                var wordToUse = urbanResponse.List.Where(w => (w.ThumbsUp - w.ThumbsDown) > 0).ToList()[pos];

                em.WithTitle(wordToUse.Word);
                em.WithUrl(wordToUse.Permalink);
                em.AddField("Definition", wordToUse.Definition);
                em.AddField("Example", wordToUse.Example);
                em.AddField("Tags", urbanResponse.Tags.Aggregate((i, j) => i + ", " + j));
                em.AddInlineField("Author", wordToUse.Author);
                em.AddInlineField("Rating", wordToUse.ThumbsUp - wordToUse.ThumbsDown);
                em.WithFooter(new EmbedFooterBuilder().WithText($" Definition {pos + 1}/{urbanResponse.List.Count} ({urbanResponse.List.Where(w => (w.ThumbsUp - w.ThumbsDown) > 0).ToList().Count})"));
                em.WithColor(new Color(19, 79, 230));
            }

            await ReplyAsync("", embed: em.Build());
        }
    }
}