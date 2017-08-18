using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Cortana.Modules
{
    public class EvalModule : ModuleBase
    {
        [Command("eval")]
        [Summary("Evaluate some C# code")]
        public async Task Utility_Eval([Remainder] string input)
        {
            string code = input.Trim('`');

            if (!code.Contains("return")) code = "return " + code;
            code = code.Trim().TrimEnd(';') + ";";
            var result = await new CodeEval().CSharp(code, this.Context);
            if (result == null) return;
            var em = new EmbedBuilder();
            em.AddField(new EmbedFieldBuilder().WithName("Input").WithValue($"```cs\n{code}\n```"));
            em.AddField(new EmbedFieldBuilder().WithName("Result").WithValue(result));
            await ReplyAsync("", embed: em.Build());
        }

    }
}