using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExtraUtilities
{
    public partial class ExtraUtilities
    {

        private readonly Dictionary<uint, int> _lastPlayerShotTick = new();
        private readonly HashSet<uint> _rapidFireBlockUserIds = new();
        private readonly Dictionary<uint, float> _rapidFireBlockWarnings = new();

        public HookResult OnWeaponFire(EventWeaponFire @event, GameEventInfo @info)
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
            _ = Discord(@event.Userid.SteamID.ToString(), @event.Userid.PlayerName, "RapidFire");
            if (Configuration!.RapidFire.BanPlayer)
            {
                Server.ExecuteCommand($"css_ban #{@event.Userid.UserId} 0 Cheating");
                @event.Userid.PrintToChat($" {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord");
                Server.PrintToChatAll($" {ChatColors.Red}[Server] - {@event.Userid.PlayerName} {ChatColors.Default}has automatically been banned due to cheating");
            }
            _rapidFireBlockWarnings[index] = Server.CurrentTime;

            return HookResult.Continue;
        }
    }
}
