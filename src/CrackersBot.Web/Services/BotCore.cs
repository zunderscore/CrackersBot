using CrackersBot.Core;
using CrackersBot.Core.Actions;
using CrackersBot.Core.Variables;
using CrackersBot.Web.Services.Automation.Actions;
using CrackersBot.Web.Services.Automation.Variables;
using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CrackersBot.Web.Services
{
    public class BotCore : IBotCore
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _discordSocketClient;

        public BotCore(IConfiguration config)
        {
            _config = config;
            _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50,
            });
        }

        public IDiscordClient DiscordClient => _discordSocketClient;

        public ConcurrentDictionary<string, IAction> RegisteredActions { get; } = new ConcurrentDictionary<string, IAction>();

        public ConcurrentDictionary<string, IVariable> RegisteredVariables { get; } = new ConcurrentDictionary<string, IVariable>();

        internal async Task StartBotCoreAsync()
        {
            RegisterCoreActions();
            RegisterCoreVariables();

            _discordSocketClient.Ready += ClientReady;
            _discordSocketClient.MessageReceived += OnMessageReceived;
            _discordSocketClient.MessageDeleted += OnMessageDeleted;

            await _discordSocketClient.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
            await _discordSocketClient.StartAsync();

            await _discordSocketClient.SetActivityAsync(new Game("Beep boop", ActivityType.CustomStatus));
            
            await OnBotStarted();
        }

        internal async Task StopBotCoreAsync()
        {
            await _discordSocketClient.StopAsync();
            await _discordSocketClient.DisposeAsync();
        }

        // Actions

        public static string GetActionId(Type actionType)
        {
            var attr = actionType.GetCustomAttributes(false)
                .FirstOrDefault(a => a is ActionIdAttribute);

            if (attr is null) throw new ArgumentException($"{actionType.Name} is not a valid Action");

            return ((ActionIdAttribute)attr).Id;
        }

        public bool IsActionRegistered(string id)
        {
            return RegisteredActions.Keys.Any(k => k.ToLowerInvariant() == id.ToLowerInvariant());
        }

        public void RegisterAction(IAction action)
        {
            var actionId = GetActionId(action.GetType());
            if (IsActionRegistered(actionId))
            {
                Debug.WriteLine($"Unable to register Action {actionId} as it has already been registered");
                return;
            }

            if (RegisteredActions.TryAdd(actionId, action))
            {
                Debug.WriteLine($"Registered Action {actionId}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Action {actionId} (registration failed)");
            }
        }

        public void UnregisterAction(string id)
        {
            if (!IsActionRegistered(id))
            {
                Debug.WriteLine($"Unable to unregister Action {id} since it is not currently registered");
                return;
            }
            
            if (RegisteredActions.TryRemove(id, out _))
            {
                Debug.WriteLine($"Unregistered Action {id}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Action {id} (removal failed)");
            }
        }

        public IAction GetRegisteredAction(string actionId)
        {
            if (!IsActionRegistered(actionId))
            {
                throw new ArgumentException($"Action {actionId} is not registered");
            }

            return RegisteredActions[actionId];
        }

        // Variables

        public bool IsVariableRegistered(string token)
        {
            return RegisteredVariables.Keys.Any(k => k.ToLowerInvariant() == token.ToLowerInvariant());
        }

        public void RegisterVariable(IVariable variable)
        {
            if (IsVariableRegistered(variable.Token))
            {
                Debug.WriteLine($"Unable to register Variable {variable.Token} as it has already been registered");
                return;
            }

            if (RegisteredVariables.TryAdd(variable.Token, variable))
            {
                Debug.WriteLine($"Registered Variable {variable.Token}");
            }
            else
            {
                Debug.WriteLine($"Unable to register Variable {variable.Token} (registration failed)");
            }
        }

        public void UnregisterVariable(string token)
        {
            if (!IsVariableRegistered(token))
            {
                Debug.WriteLine($"Unable to unregister Variable {token} since it is not currently registered");
                return;
            }
            
            if (RegisteredVariables.TryRemove(token, out _))
            {
                Debug.WriteLine($"Unregistered Variable {token}");
            }
            else
            {
                Debug.WriteLine($"Unable to unregister Variable {token} (removal failed)");
            }
        }

        private List<string> GetVariableTokens(string value)
        {
            var tokenRegex = new Regex(@"\$(\w+)");
            var tokens = tokenRegex.Matches(value).Cast<Match>()
                .SelectMany(m => m.Groups.Cast<Group>()
                    .Skip(1) // Ignore the complete group with the $ prefix
                    .SelectMany(g => g.Captures.Cast<Capture>()
                        .Select(c => c.Value)));

            return tokens.ToList();
        }

        public string ProcessVariables(string value, Dictionary<string, object> context)
        {
            foreach (var token in GetVariableTokens(value))
            {
                Debug.WriteLine($"Token: {token}");

                if (RegisteredVariables.ContainsKey(token))
                {
                    value = value.Replace($"${token}", RegisteredVariables[token].GetValue(this, context));
                }
            }

            return value;
        }

        // Register core items

        private void RegisterCoreActions()
        {
            RegisterAction(new SendDiscordChannelMessageAction());
            RegisterAction(new SendDiscordDirectMessageAction());
            RegisterAction(new ReactToDiscordMessageAction());
            RegisterAction(new ClearDiscordChannelAction());
        }

        private void RegisterCoreVariables()
        {
            RegisterVariable(new DiscordAuthorIdVariable());
            RegisterVariable(new DiscordChannelIdVariable());
            RegisterVariable(new DiscordMessageIdVariable());
            RegisterVariable(new MessageTextVariable());
            RegisterVariable(new RegisteredActionCountVariable());
            RegisterVariable(new RegisteredVeriableCountVariable());
        }

        // Bot events

        private async Task ClientReady()
        {
            await Task.Run(() => Debug.WriteLine("Client ready!"));
            return;
        }

        // Default event handlers

        private async Task OnBotStarted()
        {
            foreach (var handler in Zunderdome.BotStartedEventHandlers)
            {
                await handler.Handle(this);
            }
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Channel is SocketTextChannel textChannel &&
                textChannel.Guild.Id == Zunderdome.ZUNDERDOME_DISCORD_GUILD_ID)
            {
                foreach (var handler in Zunderdome.MessageReceivedEventHandlers)
                {
                    await handler.Handle(this, message);
                }
            }
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            if (message.HasValue)
            {
                var originalMessage = String.IsNullOrWhiteSpace(message.Value.ToString()) ? "[empty message]" : message.Value.ToString();

                var embed = new EmbedBuilder()
                    .WithTitle("Message Deleted")
                    .AddField("Channel", $"<#{channel.Id}>")
                    .AddField("Original Author", $"<@{message.Value.Author.Id}>")
                    .AddField("Original Message", originalMessage, inline: true)
                    .Build();

                if (message.Value.Channel is ITextChannel textChannel &&
                    textChannel.Guild.Id == Zunderdome.ZUNDERDOME_DISCORD_GUILD_ID)
                {
                    foreach (var handler in Zunderdome.MessageDeletedEventHandlers)
                    {
                        await handler.Handle(this, message.Id, channel.Id, message.Value);
                    }
                }
            }
            else
            {
                Debug.WriteLine("Unable to retrieve deleted message from cache; skipping events");
            }
        }
    }
}