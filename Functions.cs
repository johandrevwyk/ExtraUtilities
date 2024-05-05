using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace ExtraUtilities
{
    public static class Chat
    {
        private static readonly Dictionary<string, char> PredefinedColors = typeof(ChatColors)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .ToDictionary(field => $"{{{field.Name}}}", field => (char)(field.GetValue(null) ?? '\x01'));

        public static string FormatMessage(string message) =>
            PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, $"{color.Value}"));

        public static string CleanMessage(string message) =>
            PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, "").Replace(color.Value.ToString(), ""));
    }

    public partial class ExtraUtilities : BasePlugin, IPluginConfig<UtilitiesConfig>
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task Discord(string steamid, string playername, string type)
        {
            string header = _hostname;

            string steamProfileUrl = $"https://steamcommunity.com/profiles/{steamid}";


            string data = Localizer["message_template", playername, steamProfileUrl, type];

            string message = $"**{header}**\n{data}";

            Console.WriteLine(message); //fallback incase discord webhook doesnt work

            string webhookUrl = Config.General.Webhook;

            var payload = new
            {
                content = message
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(webhookUrl, httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Message sent to Discord webhook successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to send message to Discord webhook. Status code: {response.StatusCode}");
            }
        }



        private void ResetPlayerStats(int slot)
        {
            HeadshotSmokePenetratedNoScope[slot] = 0;
            HeadshotPenetrated[slot] = 0;
            HeadshotSmoke[slot] = 0;
            HeadshotSmokePenetratedNoScope[slot] = 0;
            HeadshotSmokePenetrated[slot] = 0;
            RapidFire[slot] = 0;
        }

        private void OnMapStart(string mapName)
        {
            foreach (CCSPlayerController player in connectedPlayers.Values)
            {
                ResetPlayerStats(player.Slot);

                _lastPlayerShotTick.Clear();
            }

            _hostname = ConVar.Find("hostname")!.StringValue;
        }
        public HookResult OnRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
        {
            foreach (CCSPlayerController player in connectedPlayers.Values)
            {
                Speed[player.Slot] = 0;
            }

            return HookResult.Continue;
        }

        public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            if (@event.Userid!.IsValid)
            {
                var player = @event.Userid;

                if (!player.IsValid || player.IsBot || player.IsHLTV)
                {
                    return HookResult.Continue;
                }
                else
                {
                    Speed.Remove(player.Slot);
                    HeadshotSmokePenetratedNoScope.Remove(player.Slot);
                    HeadshotPenetrated.Remove(player.Slot);
                    HeadshotSmoke.Remove(player.Slot);
                    HeadshotSmokePenetratedNoScope.Remove(player.Slot);
                    HeadshotSmokePenetrated.Remove(player.Slot);
                    RapidFire.Remove(player.Slot);

                    var entityFromSlot = Utilities.GetPlayerFromSlot(player.Slot);
                    _lastPlayerShotTick.Remove(entityFromSlot!.Pawn.Index);
                    _rapidFireBlockUserIds.Remove(entityFromSlot.Pawn.Index);
                    _rapidFireBlockWarnings.Remove(entityFromSlot.Pawn.Index);

                    connectedPlayers.Remove(player.Slot);
                    return HookResult.Continue;
                }
            }
            else
            {
                return HookResult.Continue;
            }
        }

        public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (@event.Userid!.IsValid)
            {
                var player = @event.Userid;

                if (!player.IsValid || player.IsBot || player.IsHLTV)
                {
                    return HookResult.Continue;
                }
                else
                {
                    connectedPlayers[player.Slot] = new CCSPlayerController(player.Handle);
                    Speed[player.Slot] = 0;
                    HeadshotPenetratedNoScope[player.Slot] = 0;
                    HeadshotPenetrated[player.Slot] = 0;
                    HeadshotSmoke[player.Slot] = 0;
                    HeadshotSmokePenetratedNoScope[player.Slot] = 0;
                    HeadshotSmokePenetrated[player.Slot] = 0;
                    RapidFire[player.Slot] = 0;

                    _lastPlayerShotTick.Remove(@event.Userid.Pawn.Index);
                    _rapidFireBlockUserIds.Remove(@event.Userid.Pawn.Index);
                    _rapidFireBlockWarnings.Remove(@event.Userid.Pawn.Index);
                    return HookResult.Continue;
                }
            }
            else
            {
                return HookResult.Continue;
            }
        }

    }
}