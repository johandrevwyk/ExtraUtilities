using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExtraUtilities
{
    public partial class ExtraUtilities
    {

        private readonly Dictionary<uint, int> _lastPlayerShotTick = new();
        private readonly HashSet<uint> _rapidFireBlockUserIds = new();
        private readonly Dictionary<uint, float> _rapidFireBlockWarnings = new();

        public Dictionary<int, int> RapidFire { get; set; } = new Dictionary<int, int>();

        public HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo @info)
        {
            if (Configuration!.RapidFire.Enabled)
            {
                if (@event.Userid is not { IsValid: true, IsHLTV: false, IsBot: false, UserId: not null, SteamID: > 0 })
                    return HookResult.Continue;

                var nextPrimaryAttackTick = @event.Userid.Pawn.Value?.WeaponServices?.ActiveWeapon.Value?.NextPrimaryAttackTick ?? 0;
                var index = @event.Userid.Pawn.Index;

                if (!_lastPlayerShotTick.TryGetValue(index, out var lastShotTick))
                {
                    _lastPlayerShotTick[index] = Server.TickCount;
                    return HookResult.Continue;
                }

                _lastPlayerShotTick[index] = Server.TickCount;

                // this is ghetto but should work for now
                if (nextPrimaryAttackTick > lastShotTick)
                    return HookResult.Continue;

                // clear list every frame (in case of misses)
                if (_rapidFireBlockUserIds.Count == 0)
                    Server.NextFrame(_rapidFireBlockUserIds.Clear);

                _rapidFireBlockUserIds.Add(index);

                // skip warning if we already warned this player in the last 3 seconds
                if (_rapidFireBlockWarnings.TryGetValue(index, out var lastWarningTime) &&
                    lastWarningTime + 3 > Server.CurrentTime)
                    return HookResult.Continue;

                // warn player

                if (RapidFire.ContainsKey(@event.Userid.Slot)) RapidFire[@event.Userid.Slot]++;

                if (RapidFire[@event.Userid.Slot] == Configuration!.RapidFire.Threshold)
                {
                    string steamid = @event.Userid.SteamID.ToString();
                    string playername = @event.Userid.PlayerName;
                    _ = Task.Run(async () => await Discord(steamid, playername, "RapidFire"));

                    if (Configuration!.RapidFire.BanPlayer)
                    {
                        string banMessagePlayer = Configuration.RapidFire.BanMessagePlayer
                                                    .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                                                    .Replace("{ChatColors.Default}", $"{ChatColors.Default}");

                        string banMessageServer = Configuration.RapidFire.BanMessageServer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{attackerController.PlayerName}", @event.Userid.PlayerName);

                        Server.ExecuteCommand($"css_ban #{@event.Userid.UserId} 0 {Configuration!.RapidFire.BanReason}");

                        @event.Userid.PrintToChat(banMessagePlayer);
                        Server.PrintToChatAll(banMessageServer);
                    }
                }

                _rapidFireBlockWarnings[index] = Server.CurrentTime;
            }
            return HookResult.Continue;
        }
    }
}
