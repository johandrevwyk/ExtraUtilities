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

            string arg = message.GetArg(1);
            if (Configuration!.BannedWords.Any(arg.Contains))
            {
                _ = Discord(player.SteamID.ToString(), player.PlayerName, arg);
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }

        private HookResult OnPlayerChatAll(CCSPlayerController? player, CommandInfo message)
        {
            if (player == null || !player.IsValid || player.IsBot || string.IsNullOrEmpty(message.GetArg(1))) return HookResult.Handled;

            string arg = message.GetArg(1);
            if (Configuration!.BannedWords.Any(arg.Contains))
            {
                _ = Discord(player.SteamID.ToString(), player.PlayerName, arg);
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }
    }
}
