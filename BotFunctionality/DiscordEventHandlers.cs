using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using ReplayUpload;

namespace BotFunctionality
{
    public class DiscordEventHandlers
    {
        private DiscordSocketClient _client;
        private SocketGuild guild;
        private Uploader upload;
        private List<ulong> sentMessages;
        private List<Stream> StreamsInUsage;
        private List<EmbedBuilder> EmbedsToModify;

        public DiscordEventHandlers(DiscordSocketClient client)
        {
            upload = new Uploader();
            StreamsInUsage = new List<Stream>();
            sentMessages = new List<ulong>();
            EmbedsToModify = new List<EmbedBuilder>();  

            _client = client;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += HandleSlashCommand;
            _client.MessageReceived += HandleMessageReceived;
            _client.ReactionAdded += HandleReactionAdded;
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "addnick":
                    await HandleAddNick(command);
                    break;
                case "viewnicks":
                    await HandleViewNicks(command);
                    break;
            }
        }

        private async Task HandleMessageReceived(SocketMessage message)
        {

            if (message.Attachments.Count > 0)
            {
                List<Stream> FilesCollection = new List<Stream>();
                List<string> FileNames = new List<string>();

                foreach (var attachment in message.Attachments)
                {
                    var fileType = Path.GetExtension(attachment.Filename);
                    if (fileType == ".BfME2Replay")
                    {
                        using (var client = new HttpClient())
                        {
                            client.Timeout = TimeSpan.FromHours(50);
                            var fileContents = await client.GetAsync(attachment.Url);
                            var replayFileStream = await fileContents.Content.ReadAsStreamAsync();
                            FilesCollection.Add(replayFileStream);
                            FileNames.Add(attachment.Filename);
                        }
                    }
                }

                StreamsInUsage.AddRange(FilesCollection);
                upload.AddMoreData(FilesCollection, FileNames);
                List<EmbedBuilder> Embeds = upload.GetEmbeds();

                for (int i = 0; i < Embeds.Count; i++)
                {
                    var embed = Embeds[i];
                    bool invalid = false;

                    if (CheckEmbedFieldForError(embed, 0) || CheckEmbedFieldForError(embed, 2) || (char.ToLower(FileNames[i][0]) != 'w' && char.ToLower(FileNames[i][0]) != 'l'))
                    { invalid = true; }

                    if (invalid)
                    {
                        embed.WithFooter(footer => footer.Text = "⛔ Replay not valid. Please check for errors ⛔");
                    }
                    else
                    {
                        embed.WithFooter(footer => footer.Text = "Waiting for league manager ✅");
                    }
                    EmbedsToModify.Add(embed);

                    var sentMessage = await message.Channel.SendMessageAsync(embed: embed.Build());
                    sentMessages.Add(sentMessage.Id);

                        if (invalid) 
                        {
                            _ = Task.Run(async () =>
                            {
                                await sentMessage.AddReactionAsync(new Emoji("❎"));
                            });
                        }
                        else
                        {
                            _ = Task.Run(async () =>
                            {
                                await sentMessage.AddReactionAsync(new Emoji("✅"));
                                await sentMessage.AddReactionAsync(new Emoji("❎"));
                            });  
                        } 
                }
            }
        }

        private async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        {

            var message = await cachedMessage.GetOrDownloadAsync();

            // Check if the reaction is on the correct message (embed message with ✅❎ reactions)
            if (sentMessages.Contains(message.Id) && reaction.UserId != 1049646434359717909)  //1049646434359717909
            {
                int index = sentMessages.IndexOf(message.Id);
                var modifiedEmbed = EmbedsToModify[index];

                if (reaction.Emote.Name == "✅" && reaction.UserId == 521806629679792130)
                {
                    modifiedEmbed.WithFooter(footer => footer.Text = "Replay added ✅");
                    var newEmbed = new Optional<Embed>(modifiedEmbed.Build());
                    var viewOnWebsiteButton = new ButtonBuilder
                    {
                        Style = ButtonStyle.Link,
                        Label = "View on the website",
                        Url = "https://events.laterredumilieu.fr/en/league/matchs" 
                    };

                    var componentBuilder = new ComponentBuilder();
                    componentBuilder.WithButton(viewOnWebsiteButton);

                    await message.ModifyAsync(msg =>
                    {
                        msg.Embed = newEmbed;
                        msg.Components = componentBuilder.Build();
                    });

                    await upload.GameApproved(sentMessages.IndexOf(message.Id));
                    sentMessages [sentMessages.IndexOf(message.Id)] = 0;
                }
                else if (reaction.Emote.Name == "❎")
                {
                    modifiedEmbed.WithFooter(footer => footer.Text = "Replay canceled ❎");
                    var newEmbed = new Optional<Embed>(modifiedEmbed.Build());
                    await message.ModifyAsync(msg => msg.Embed = newEmbed);
                    sentMessages[sentMessages.IndexOf(message.Id)] = 0;
                }

                if (sentMessages.All(value => value == 0))
                {
                    sentMessages.Clear();
                    StreamsInUsage.ForEach(stream => stream.Close());
                    StreamsInUsage.Clear();
                    EmbedsToModify.Clear();
                    upload = new Uploader();
                }
            }
        }

        private async Task HandleAddNick(SocketSlashCommand command)
        {
            string nickname = (string)command.Data.Options.First().Value;
            string userName = command.User.Username;
            SocketGuildUser Artanis = guild.GetUser(521806629679792130);
            var dmChannel = await Artanis.CreateDMChannelAsync();
            await dmChannel.SendMessageAsync($"Person with discort nick {userName} and id userId, wants a league nick {nickname}");
            await command.RespondAsync(text: $"Your new nickname {nickname} will soon be added", ephemeral: true);
        }
        private async Task HandleViewNicks(SocketSlashCommand command)
        {
            var matchingKeys = upload.FetchNicknames(command.User.Username);

            if (matchingKeys.Count > 0)
            {
                await command.RespondAsync(text: $"The keys associated with the value '{command.User.Username}' are: {string.Join(", ", matchingKeys)}.", ephemeral: true);
            }
            else
            {
                await command.RespondAsync(text: $"No keys found for the value '{command.User.Username}'.", ephemeral: true);
            }
        }

        static bool CheckEmbedFieldForError(EmbedBuilder embed, int fieldIndex)
        {
            if (embed.Fields.Count > fieldIndex)
            {
                var field = embed.Fields[fieldIndex];
                if (field.Name != null && field.Name.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (field.Value != null && field.Value.ToString().Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task Client_Ready()
        {
            guild = _client.GetGuild(735052555633164340);  // paste rotwk server id here
            var guildCommand = new SlashCommandBuilder()
                .WithName("addnick")
                .WithDescription("Set an additional nickname for league")
                .AddOption("nickname", ApplicationCommandOptionType.String, "Enter additional nickname that you would like to use in game", isRequired: true);

            var guildCommand2 = new SlashCommandBuilder()
                .WithName("viewnicks")
                .WithDescription("view players nicks");

            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
                await guild.CreateApplicationCommandAsync(guildCommand2.Build());
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        // Add other event handler methods here 
    }
}
