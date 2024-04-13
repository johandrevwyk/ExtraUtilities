using System.Numerics;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text;
using CounterStrikeSharp.API.Modules.Entities.Constants;

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
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);

    }

    public Dictionary<int, int> HeadshotPenetratedNoScope { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotPenetrated { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotSmoke { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotSmokePenetratedNoScope { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> HeadshotSmokePenetrated { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, int> Bullets { get; set; } = new Dictionary<int, int>();

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo @info)
    {
        if (@event.Userid.IsValid)
        {
            if (@event == null) return HookResult.Continue;
            if (@event.Userid == null) return HookResult.Continue;

            var player = @event.Userid;
            bool headshot = @event.Headshot;
            bool noscope = @event.Noscope;
            bool thrusmoke = @event.Thrusmoke;
            int? penetrated = @event.Penetrated;
            CCSPlayerController attackerController = @event.Attacker;

            if (!player.IsBot || player.IsValid || player != null)
            {               
                if (attackerController.IsValid)
                {
                    CheckThreshold(attackerController, "HeadshotPenetratedNoScope", headshot && penetrated > 0 && noscope, 2);
                    CheckThreshold(attackerController, "HeadshotPenetrated", headshot && penetrated > 0, 5); //working
                    CheckThreshold(attackerController, "HeadshotSmokePenetratedNoScope", headshot && thrusmoke && penetrated > 0 && noscope, 2);
                    CheckThreshold(attackerController, "HeadshotSmoke", headshot && thrusmoke, 3); //working
                    CheckThreshold(attackerController, "HeadshotSmokePenetrated", headshot && thrusmoke && penetrated > 0, 3);
                }

            }
        }
        return HookResult.Continue;
    }

    private void CheckThreshold(CCSPlayerController attackerController, string type, bool condition, int threshold)
    {
        if (condition)
        {
            int slot = attackerController.Slot;
            int count = IncreaseCount(type, slot);
            if (count == threshold)
            {
                Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of {type}");
                _ = Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, type);
            }
        }
    }

    private int IncreaseCount(string type, int slot)
    {
        switch (type)
        {
            case "HeadshotPenetratedNoScope":
                if (HeadshotPenetratedNoScope.ContainsKey(slot))
                    return ++HeadshotPenetratedNoScope[slot];
                break;
            case "HeadshotPenetrated":
                if (HeadshotPenetrated.ContainsKey(slot))
                    return ++HeadshotPenetrated[slot];
                break;
            case "HeadshotSmokePenetratedNoScope":
                if (HeadshotSmokePenetratedNoScope.ContainsKey(slot))
                    return ++HeadshotSmokePenetratedNoScope[slot];
                break;
            case "HeadshotSmokePenetrated":
                if (HeadshotSmokePenetrated.ContainsKey(slot))
                    return ++HeadshotSmokePenetrated[slot];
                break;
            case "HeadshotSmoke":
                if (HeadshotSmoke.ContainsKey(slot))
                    return ++HeadshotSmoke[slot];
                break;
        }
        return 0;
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
        // Retrieve the collection of players using Utilities.GetPlayers()
        var players = Utilities.GetPlayers();

        // Reset player stats
        foreach (var player in players)
        {
            ResetPlayerStats(player.Slot);
        }
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
                HeadshotSmokePenetratedNoScope.Remove(player.Slot);
                HeadshotPenetrated.Remove(player.Slot);
                HeadshotSmoke.Remove(player.Slot);
                HeadshotSmokePenetratedNoScope.Remove(player.Slot);
                HeadshotSmokePenetrated.Remove(player.Slot);
                return HookResult.Continue;
            }
        }
        else
        {
            return HookResult.Continue;
        }
    }

    private readonly HttpClient _httpClient = new HttpClient();

    public async Task Discord(string steamid, string playername, string type)
    {
        // Construct your message
        string steamProfileUrl = $"https://steamcommunity.com/profiles/{steamid}";

        string message = $"@everyone Player: [{playername}]({steamProfileUrl}) has reached the limit of - {type} (most likely a spinbotter/cheater)";

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
            HeadshotPenetratedNoScope[player.Slot] = 0;
            HeadshotPenetrated[player.Slot] = 0;
            HeadshotSmoke[player.Slot] = 0;
            HeadshotSmokePenetratedNoScope[player.Slot] = 0;
            HeadshotSmokePenetrated[player.Slot] = 0;
        }

        return HookResult.Continue;
    }
}
