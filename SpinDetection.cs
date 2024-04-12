using System.Numerics;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text;

namespace SpinDetection;

public partial class SpinDetection : BasePlugin
{
    public override string ModuleName => "SpinDetection";
    public override string ModuleAuthor => "heartbreakhotel";
    public override string ModuleDescription => "Basic spinbot detection";
    public override string ModuleVersion => "0.0.1";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Spin detection started");

        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);

    }

    public Dictionary<int, int> HeadshotPenetratedNoScope { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotPenetrated { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotSmoke { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotSmokePenetratedNoScope { get; set; } = new Dictionary<int, int>();

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo @info)
    {

        if (@event.Userid.IsValid && !@event.Userid.IsHLTV)
        {
            int? attacker = @event.Attacker.UserId;

            bool headshot = @event.Headshot;
            bool noscope = @event.Noscope;
            int penetrated = @event.Penetrated;
            bool thrusmoke = @event.Thrusmoke;

            int attackerId = attacker!.Value;
            CCSPlayerController attackerController = Utilities.GetPlayerFromUserid(attackerId);

            if (attacker.HasValue)
            {
                if (headshot && (penetrated > 0) && noscope)
                {
                    if (attacker.HasValue)
                    {
                        HeadshotPenetratedNoScope[attackerController.Slot]++;

                        if (HeadshotPenetratedNoScope[attackerController.Slot] == 2)
                        {
                            Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of HeadshotPenetratedNoScope");
                            _ = Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, "HeadshotPenetratedNoScope");
                        }
                    }

                }

                if (headshot && (penetrated > 0))
                {
                    if (attacker.HasValue)
                    {
                        HeadshotPenetrated[attackerController.Slot]++;

                        if (HeadshotPenetrated[attackerController.Slot] == 5)
                        {
                            Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of HeadshotPenetrated");
                            _ = Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, "HeadshotPenetrated");
                        }
                    }
                }

                if (headshot && thrusmoke && (penetrated > 0) && noscope)
                {
                    if (attacker.HasValue)
                    {
                        HeadshotSmokePenetratedNoScope[attackerController.Slot]++;

                        if (HeadshotSmokePenetratedNoScope[attackerController.Slot] == 2)
                        {
                            Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of HeadshotSmokePenetratedNoScope");
                            _ = Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, "HeadshotSmokePenetratedNoScope");
                        }
                    }
                }

                if (headshot && thrusmoke)
                {
                    if (attacker.HasValue)
                    {
                        HeadshotSmoke[attackerController.Slot]++;

                        if (HeadshotSmoke[attackerController.Slot] == 3)
                        {
                            Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of HeadshotSmoke");
                            _ = Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, "HeadshotSmoke");
                        }
                    }
                }
            }
            
        }
        return HookResult.Continue;
    }

    private readonly HttpClient _httpClient = new HttpClient();

    public async Task Discord(string steamid, string playername, string type)
    {
        // Construct your message
        string message = $"Steam ID: {steamid}, Player: {playername} has reached the limit of - {type}";

        // Discord webhook URL
        string webhookUrl = "https://discord.com/api/webhooks/1228471920136294461/JA0BoM2EmOIJj4JJkvE45s-ZM9RzPKww8vda-PtTmv0jwCd6BX63KW5aJ79iRm4U_LBi";

        // Create JSON payload
        var payload = new
        {
            content = message
        };

        // Serialize payload to JSON
        var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

        // Create HTTP content with JSON payload
        var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Send POST request to Discord webhook URL
        var response = await _httpClient.PostAsync(webhookUrl, httpContent);

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Message sent to Discord webhook successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to send message to Discord webhook. Status code: {response.StatusCode}");
        }
    }

    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo @info)
    {
        CCSPlayerController player = @event.Userid;

        if (player.IsValid && !player.IsHLTV)
        {
            HeadshotSmokePenetratedNoScope[player.Slot] = 0;
            HeadshotPenetrated[player.Slot] = 0;
            HeadshotSmoke[player.Slot] = 0;
            HeadshotSmokePenetratedNoScope[player.Slot] = 0;
        }

        return HookResult.Continue;
    }
}
