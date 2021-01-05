using Discord;
using Discord.WebSocket;
using DXStats.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DxStats.Services
{
    public class DiscordStartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IOptions<DiscordCredentials> _discordCredentials;

        public DiscordStartupService(DiscordSocketClient discord, IOptions<DiscordCredentials> discordCredentials)
        {
            _discord = discord;
            _discordCredentials = discordCredentials;
        }

        public async Task StartAsync()
        {
            string discordToken = _discordCredentials.Value.Token;
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();
        }
    }
}
