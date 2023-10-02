using Discord;
using Discord.WebSocket;
using BotFunctionality;


namespace discBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private DiscordEventHandlers _eventHandlers; // Add this field

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 1000,
                GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.Guilds | GatewayIntents.GuildMessageTyping | GatewayIntents.DirectMessages | GatewayIntents.DirectMessageTyping | GatewayIntents.GuildEmojis | GatewayIntents.GuildMembers | GatewayIntents.GuildMessageReactions | GatewayIntents.GuildPresences | GatewayIntents.MessageContent
            });

            _client.Log += Log;

            string token = "MTA0OTY0NjQzNDM1OTcxNzkwOQ.GdWZqp.juXqw4bnrTYrkJYAq063bxIBbRnwEKRsE8KYSo";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _eventHandlers = new DiscordEventHandlers(_client); // Initialize the event handlers

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}













// string token = "MTA0OTY0NjQzNDM1OTcxNzkwOQ.GuVyuX.KAoGb28L6eQLJNwi5wKzis9Pt7vptSuPN39gh4";
