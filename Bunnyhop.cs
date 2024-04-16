using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
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

        public void OnTick()
        {
            var players = Utilities.GetPlayers();

            foreach (var player in players)
            {
                if (player.IsValid && player.PawnIsAlive && !player.IsBot && !player.IsHLTV)
                {
                    Vector velocity = player.PlayerPawn!.Value!.AbsVelocity;
                    float velo = velocity.Length2D();

                    if (Configuration!.Bunnyhop.DecreasePlayerSpeed)
                    {
                        if (velo > Configuration!.Bunnyhop.SpeedLimit)
                        {
                            float mult = Configuration!.Bunnyhop.SpeedLimit / velo;
                            velocity.X *= mult;
                            velocity.Y *= mult;
                            player.PlayerPawn.Value!.AbsVelocity.X = velocity.X;
                            player.PlayerPawn.Value!.AbsVelocity.Y = velocity.Y;
                        }
                    }

                    if (Speed.ContainsKey(player.Slot)) Speed[player.Slot]++;
                    if (Speed[player.Slot] == Configuration!.Bunnyhop.Threshold)
                    {
                        _ = Discord(player.SteamID.ToString(), player.PlayerName, "Speed");
                    }
                }
            }
        }
    }
}
