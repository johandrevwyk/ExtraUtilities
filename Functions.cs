using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraUtilities
{
    public partial class ExtraUtilities
    {
        private void LoadConfig()
        {
            GameDir = Server.GameDirectory;
            var jsonPath = Path.Join(GameDir + "/csgo/addons/counterstrikesharp/plugins/ExtraUtilities", "config.json");

            Configuration = JsonConvert.DeserializeObject<Config>(File.ReadAllText(jsonPath));

            if (Configuration == null)
                throw new JsonException("Configuration could not be loaded");
        }

        private readonly HttpClient _httpClient = new HttpClient();

        public async Task Discord(string steamid, string playername, string type)
        {
            string messageTemplate = Configuration!.General.MessageTemplate;

            // Construct message with actual values
            string steamProfileUrl = $"https://steamcommunity.com/profiles/{steamid}";
            string message = messageTemplate
                .Replace("{playername}", playername)
                .Replace("{steamProfileUrl}", steamProfileUrl)
                .Replace("{type}", type);

            // Discord webhook URL
            string webhookUrl = Configuration!.General.Webhook;

            // Create JSON payload
            var payload = new
            {
                content = message
            };

            // Serialize payload to JSON
            var jsonPayload = JsonConvert.SerializeObject(payload);

            // Create HTTP content with JSON payload
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send POST request to Discord webhook URL
            var response = await _httpClient.PostAsync(webhookUrl, httpContent);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                Logger.LogInformation("Message sent to Discord webhook successfully.");
            }
            else
            {
                Logger.LogCritical($"Failed to send message to Discord webhook. Status code: {response.StatusCode}");
            }
        }


        private void ResetPlayerStats(int slot)
        {
            HeadshotSmokePenetratedNoScope[slot] = 0;
            HeadshotPenetrated[slot] = 0;
            HeadshotSmoke[slot] = 0;
            HeadshotSmokePenetratedNoScope[slot] = 0;
            HeadshotSmokePenetrated[slot] = 0;
        }

        private void OnMapStart(string mapName)
        {
            foreach (CCSPlayerController player in connectedPlayers.Values)
            {
                ResetPlayerStats(player.Slot);

                _lastPlayerShotTick.Clear();
            }
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
            if (@event.Userid.IsValid)
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

                    var entityFromSlot = Utilities.GetPlayerFromSlot(player.Slot);
                    _lastPlayerShotTick.Remove(entityFromSlot.Pawn.Index);
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
            if (@event.Userid.IsValid)
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

                    _lastPlayerShotTick.Remove(@event.Userid.Pawn.Index);
                    _rapidFireBlockUserIds.Remove(@event.Userid.Pawn.Index);
                    _rapidFireBlockWarnings.Remove(@event.Userid.Pawn.Index);
                    return HookResult.Continue;
                }
            }
            else
            {
                //Console.WriteLine($"[CS2Arena] @evemt.Userid is not valid, proceed to kill yourself");
                return HookResult.Continue;
            }
        }

    }
}