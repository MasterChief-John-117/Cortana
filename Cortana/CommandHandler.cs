using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Cortana
{
    public class CommandHandler
    {        
        public static CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _map;
        public static Configuration _config = new Configuration();

        public List<CustomCommand> CustomCommands = JsonConvert.DeserializeObject<List<CustomCommand>>(File.ReadAllText("files/customCommands.json"));
        
        public async Task Install(IServiceProvider map)
        {
            _map = map;
            // Create Command Service, inject it into Dependency Map
            _client = map.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            _commands = new CommandService();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            
            _client.MessageReceived += HandleCommand;
            _client.MessageUpdated += HandleEditedCommand;
        }
        
        private async Task HandleEditedCommand(
            Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (_config.ExecEdits) await HandleCommand(after);
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {
            // Don't handle the command if it is a system message
            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;
            // Mark where the prefix ends and the command begins
            int argPos = 0;
                        
            //VERY IMPORTANT: Return if the message is not by the Bot Owner
            //Not doing this allows anyone to execute our commands, which
            //will get Discord to ban us very quickly
            if (_config.TokenType == TokenType.User && parameterMessage.Author.Id != _client.CurrentUser.Id) return;

            // Determine if the message has a valid prefix, adjust argPos
            if (!message.HasStringPrefix(_config.Prefix, ref argPos)) return;

            string cmd = parameterMessage.Content.Remove(0, _config.Prefix.Length).ToLower();
            if (CustomCommands.Any(c => c.Command == cmd) ||
                CustomCommands.Any(c => c.Aliases.Any(a => a == cmd)))
            {
                var command = CustomCommands.First(c => c.Command == cmd || c.Aliases.Any(a => a == cmd));
                if (command.Delete) await parameterMessage.DeleteAsync();
                await parameterMessage.Channel.SendMessageAsync(command.Value);
                return;
            }
            
            // Create a Command Context
            var context = new CommandContext(_client, message);
            // Execute the Command, store the result
            var result = await _commands.ExecuteAsync(context, argPos, _map);

            // If the command failed, notify the user
            if (!result.IsSuccess)
            {
                if (!result.ErrorReason.Contains("Unknown command"))
                {
                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
                }
            }
        }
    }
}