using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExtraUtilities
{
    public partial class ExtraUtilities : BasePlugin, IPluginConfig<UtilitiesConfig>
    {
        public Dictionary<int, int> Speed { get; set; } = new Dictionary<int, int>();

        private void OnTick()
        {
            if (Config.Bunnyhop.Enabled)
            {
                foreach (CCSPlayerController player in connectedPlayers.Values)
                {
                    if (player is { PawnIsAlive: true, IsValid: true })
                    {
                        Vector velocity = player.PlayerPawn!.Value!.AbsVelocity;
                        float velo = velocity.Length2D();
                        var steamID = player.SteamID.ToString();
                        var playerName = player.PlayerName;

                        if (!(player.PlayerPawn!.Value.MoveType == MoveType_t.MOVETYPE_NOCLIP || player.PlayerPawn.Value.ActualMoveType == MoveType_t.MOVETYPE_NOCLIP 
                            || player.PlayerPawn!.Value.MoveType == MoveType_t.MOVETYPE_OBSERVER || player.PlayerPawn!.Value.ActualMoveType == MoveType_t.MOVETYPE_OBSERVER 
                            || player.PlayerPawn!.Value.MoveType == MoveType_t.MOVETYPE_FLY || player.PlayerPawn!.Value.ActualMoveType == MoveType_t.MOVETYPE_FLY) &&
                            velo > Config.Bunnyhop.SpeedLimit)
                        {
                            if (Config.Bunnyhop.DecreasePlayerSpeed)
                            {
                                float mult = Config.Bunnyhop.SpeedLimit / velo;
                                velocity.X *= mult;
                                velocity.Y *= mult;
                                player.PlayerPawn.Value!.AbsVelocity.X = velocity.X;
                                player.PlayerPawn.Value!.AbsVelocity.Y = velocity.Y;
                            }

                            if (Speed.ContainsKey(player.Slot)) Speed[player.Slot]++;
                            if (Speed[player.Slot] == Config.Bunnyhop.Threshold)
                                _ = Task.Run(async () => await Discord(steamID, playerName, "Bunnyhop Speed"));
                        }
                    }
                }
            }
        }
    }
}