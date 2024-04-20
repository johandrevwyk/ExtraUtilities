﻿using CounterStrikeSharp.API;
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
                _ = Task.Run(async () => await Discord(player.SteamID.ToString(), player.PlayerName, arg));
                if (Configuration!.BannedWordsSettings.SilencePlayer)
                {
                    // Replace variables in SilenceMessagePlayer
                    string silenceMessagePlayer = Configuration.BannedWordsSettings.SilenceMessagePlayer
                        .Replace("{ChatColors.Red}", $"{ChatColors.Red}") // Keep ChatColors.Red as is
                        .Replace("{ChatColors.Default}", $"{ChatColors.Default}") // Keep ChatColors.Default as is
                        .Replace("{Configuration.BannedWordsSettings.Duration}", Configuration.BannedWordsSettings.Duration.ToString())
                        .Replace("{Configuration.BannedWordsSettings.Reason}", Configuration.BannedWordsSettings.Reason);

                    // Replace variables in SilenceMessageServer
                    string silenceMessageServer = Configuration.BannedWordsSettings.SilenceMessageServer
                        .Replace("{ChatColors.Red}", $"{ChatColors.Red}") // Keep ChatColors.Red as is
                        .Replace("{ChatColors.Default}", $"{ChatColors.Default}") // Keep ChatColors.Default as is
                        .Replace("{player.PlayerName}", player.PlayerName)
                        .Replace("{Configuration.BannedWordsSettings.Reason}", Configuration.BannedWordsSettings.Reason);

                    Server.ExecuteCommand($"css_silence #{player.UserId} {Configuration.BannedWordsSettings.Duration} {Configuration.BannedWordsSettings.Reason}");
                    player.PrintToChat(silenceMessagePlayer);
                    Server.PrintToChatAll(silenceMessageServer);
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
                _ = Task.Run(async () => await Discord(player.SteamID.ToString(), player.PlayerName, arg));
                if (Configuration!.BannedWordsSettings.SilencePlayer)
                {
                    string silenceMessagePlayer = Configuration.BannedWordsSettings.SilenceMessagePlayer
                        .Replace("{ChatColors.Red}", $"{ChatColors.Red}") 
                        .Replace("{ChatColors.Default}", $"{ChatColors.Default}") 
                        .Replace("{Configuration.BannedWordsSettings.Duration}", Configuration.BannedWordsSettings.Duration.ToString())
                        .Replace("{Configuration.BannedWordsSettings.Reason}", Configuration.BannedWordsSettings.Reason);

                    string silenceMessageServer = Configuration.BannedWordsSettings.SilenceMessageServer
                        .Replace("{ChatColors.Red}", $"{ChatColors.Red}") 
                        .Replace("{ChatColors.Default}", $"{ChatColors.Default}") 
                        .Replace("{player.PlayerName}", player.PlayerName)
                        .Replace("{Configuration.BannedWordsSettings.Reason}", Configuration.BannedWordsSettings.Reason);

                    Server.ExecuteCommand($"css_silence #{player.UserId} {Configuration.BannedWordsSettings.Duration} {Configuration.BannedWordsSettings.Reason}");
                    player.PrintToChat(silenceMessagePlayer);
                    Server.PrintToChatAll(silenceMessageServer);
                }
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }
    }
}
