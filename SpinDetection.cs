using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraUtilities
{
    public partial class ExtraUtilities
    {
        private HashSet<string> alreadyBannedPlayers = new HashSet<string>();
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
                        CheckThreshold(attackerController, "HeadshotPenetratedNoScope", headshot && penetrated > 0 && noscope, Configuration!.SpinDetection.HeadshotPenetratedNoScope);
                        CheckThreshold(attackerController, "HeadshotPenetrated", headshot && penetrated > 0, Configuration!.SpinDetection.HeadshotPenetrated); //working
                        CheckThreshold(attackerController, "HeadshotSmokePenetratedNoScope", headshot && thrusmoke && penetrated > 0 && noscope, Configuration!.SpinDetection.HeadshotSmokePenetratedNoScope);
                        CheckThreshold(attackerController, "HeadshotSmoke", headshot && thrusmoke, Configuration!.SpinDetection.HeadshotSmoke); //working
                        CheckThreshold(attackerController, "HeadshotSmokePenetrated", headshot && thrusmoke && penetrated > 0, Configuration!.SpinDetection.HeadshotSmokePenetrated);
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
                if (count == threshold && !alreadyBannedPlayers.Contains(attackerController.SteamID.ToString()))
                {
                    Logger.LogInformation($"Player with steamid {attackerController.SteamID.ToString()} has reached the threshold of {type}");
                    _ = Task.Run(async () => await Discord(attackerController.SteamID.ToString(), attackerController.PlayerName, type);

                    if (Configuration!.SpinDetection.BanPlayer)
                    {
                        Server.ExecuteCommand($"css_ban #{attackerController.UserId} 0 Cheating");
                        attackerController.PrintToChat($" {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord");
                        Server.PrintToChatAll($" {ChatColors.Red}[Server] - {attackerController.PlayerName} {ChatColors.Default}has automatically been banned due to cheating");
                        alreadyBannedPlayers.Add(attackerController.SteamID.ToString());
                    }

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
    }
}
