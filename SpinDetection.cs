using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

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

        public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo @info)
        {
            if (@event == null) return HookResult.Continue;
            if (@event.Userid == null) return HookResult.Continue;

            CCSPlayerController victim = @event.Userid;
            CCSPlayerController attackerController = @event.Attacker!;

            if (Config.SpinDetection.Enabled)
            {
                if (victim.IsValid && attackerController.IsValid)
                {
                    bool headshot = @event.Headshot;
                    bool noscope = @event.Noscope;
                    bool thrusmoke = @event.Thrusmoke;
                    int? penetrated = @event.Penetrated;

                    CheckThreshold(attackerController, "HeadshotPenetratedNoScope", headshot && penetrated > 0 && noscope, Config.SpinDetection.HeadshotPenetratedNoScope);
                    CheckThreshold(attackerController, "HeadshotPenetrated", headshot && penetrated > 0, Config.SpinDetection.HeadshotPenetrated);
                    CheckThreshold(attackerController, "HeadshotSmokePenetratedNoScope", headshot && thrusmoke && penetrated > 0 && noscope, Config.SpinDetection.HeadshotSmokePenetratedNoScope);
                    CheckThreshold(attackerController, "HeadshotSmoke", headshot && thrusmoke, Config.SpinDetection.HeadshotSmoke);
                    CheckThreshold(attackerController, "HeadshotSmokePenetrated", headshot && thrusmoke && penetrated > 0, Config.SpinDetection.HeadshotSmokePenetrated);
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
                    string steamid = attackerController.SteamID.ToString();
                    string playername = attackerController.PlayerName;
                    _ = Task.Run(async () => await Discord(steamid, playername, type));

                    if (Config.SpinDetection.BanPlayer)
                    {
                        string banMessagePlayer = Config.SpinDetection.BanMessagePlayer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}");

                        string banMessageServer = Config.SpinDetection.BanMessageServer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{attackerController.PlayerName}", attackerController.PlayerName);

                        Server.ExecuteCommand($"css_ban #{attackerController.UserId} 0 {Config.SpinDetection.BanReason}");

                        attackerController.PrintToChat(banMessagePlayer);
                        Server.PrintToChatAll(banMessageServer);

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
