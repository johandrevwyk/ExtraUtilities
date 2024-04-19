using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
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
        private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo message)
        {
            if (player == null || !player.IsValid || player.IsBot || string.IsNullOrEmpty(message.GetArg(1))) return HookResult.Handled;

            string arg = message.GetArg(1).ToLower();
            if (Configuration!.BannedWords.Any(word => arg.Contains(word.ToLower())))
            {
                _ = Task.Run(async () => await Discord(player.SteamID.ToString(), player.PlayerName, arg);
                if (Configuration!.BannedWordsSettings.SilencePlayer)
                {
                    Server.ExecuteCommand($"css_silence #{player.UserId} {Configuration.BannedWordsSettings.Duration} {Configuration.BannedWordsSettings.Reason}");
                    player.PrintToChat($" {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been silenced for {Configuration.BannedWordsSettings.Duration} minutes due to {Configuration.BannedWordsSettings.Reason}");
                    Server.PrintToChatAll($" {ChatColors.Red}[Server] - {player.PlayerName} {ChatColors.Default}has automatically been silenced due to {Configuration.BannedWordsSettings.Reason}");
                }
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }

        private HookResult OnPlayerChatAll(CCSPlayerController? player, CommandInfo message)
        {
            if (player == null || !player.IsValid || player.IsBot || string.IsNullOrEmpty(message.GetArg(1))) return HookResult.Handled;

            string arg = message.GetArg(1).ToLower();
            if (Configuration!.BannedWords.Any(word => arg.Contains(word.ToLower())))
            {
                _ = Task.Run(async () => await Discord(player.SteamID.ToString(), player.PlayerName, arg);

                if (Configuration!.BannedWordsSettings.SilencePlayer)
                {
                    Server.ExecuteCommand($"css_silence #{player.UserId} {Configuration.BannedWordsSettings.Duration} {Configuration.BannedWordsSettings.Reason}");
                    player.PrintToChat($" {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been silenced for {Configuration.BannedWordsSettings.Duration} minutes due to {Configuration.BannedWordsSettings.Reason}");
                    Server.PrintToChatAll($" {ChatColors.Red}[Server] - {player.PlayerName} {ChatColors.Default}has automatically been silenced due to {Configuration.BannedWordsSettings.Reason}");
                }
             
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }
    }
}
