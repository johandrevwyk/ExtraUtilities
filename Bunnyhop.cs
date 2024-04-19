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
        public Dictionary<int, int> Speed { get; set; } = new Dictionary<int, int>();

        private void OnTick()
        {
            foreach (CCSPlayerController player in connectedPlayers.Values)
            {
                if (player is { PawnIsAlive: true, IsValid: true })
                {
                    Vector velocity = player.PlayerPawn!.Value!.AbsVelocity;
                    float velo = velocity.Length2D();

                    if (!(player.PlayerPawn!.Value.MoveType == MoveType_t.MOVETYPE_NOCLIP ||
                        player.PlayerPawn.Value.ActualMoveType == MoveType_t.MOVETYPE_NOCLIP) &&
                        velo > Configuration!.Bunnyhop.SpeedLimit)
                    {
                        if (Configuration!.Bunnyhop.DecreasePlayerSpeed)
                        {
                            float mult = Configuration!.Bunnyhop.SpeedLimit / velo;
                            velocity.X *= mult;
                            velocity.Y *= mult;
                            player.PlayerPawn.Value!.AbsVelocity.X = velocity.X;
                            player.PlayerPawn.Value!.AbsVelocity.Y = velocity.Y;
                        }

                        if (Speed.ContainsKey(player.Slot)) Speed[player.Slot]++;
                        if (Speed[player.Slot] == Configuration!.Bunnyhop.Threshold)
                            _ = Task.Run(async () => await Discord(player.SteamID.ToString(), player.PlayerName, "Bunnyhop Speed"));
                    }
                }
            }
        }
    }
}
