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
            if (Config.RapidFire.Enabled)
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

                if (RapidFire[@event.Userid.Slot] == Config.RapidFire.Threshold)
                {
                    string steamid = @event.Userid.SteamID.ToString();
                    string playername = @event.Userid.PlayerName;
                    _ = Task.Run(async () => await Discord(steamid, playername, "RapidFire"));

                    if (Config.RapidFire.BanPlayer)
                    {

                        string banMessagePlayer = Chat.FormatMessage(Localizer["banmsgplayer_rapid"]);
                        string banMessageServer = Chat.FormatMessage(Localizer["banmsgsvr_rapid", @event.Userid.PlayerName]);

                        Server.ExecuteCommand($"css_ban #{@event.Userid.UserId} 0 {Localizer["banreason_rapid"]}");

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
