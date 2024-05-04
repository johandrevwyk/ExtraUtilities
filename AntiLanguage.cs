using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace ExtraUtilities
{
    public partial class ExtraUtilities : BasePlugin, IPluginConfig<UtilitiesConfig>
    {
        private HookResult OnPlayerChatTeam(CCSPlayerController? player, CommandInfo message)
        {
            if (Config.BannedWordsSettings.Enabled)
            {
                if (player == null || !player.IsValid || player.IsBot || string.IsNullOrEmpty(message.GetArg(1))) return HookResult.Handled;

                string arg = message.GetArg(1).ToLower();
                if (Config.BannedWords.Any(word => arg.Contains(word.ToLower())))
                {
                    string steamid = player.SteamID.ToString();
                    string playername = player.PlayerName;
                    _ = Task.Run(async () => await Discord(steamid, playername, arg));
                    if (Config.BannedWordsSettings.SilencePlayer)
                    {
                        string silenceMessagePlayer = Config.BannedWordsSettings.SilenceMessagePlayer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{Config.BannedWordsSettings.Duration}", Config.BannedWordsSettings.Duration.ToString())
                            .Replace("{Config.BannedWordsSettings.Reason}", Config.BannedWordsSettings.Reason);

                        string silenceMessageServer = Config.BannedWordsSettings.SilenceMessageServer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{player.PlayerName}", player.PlayerName)
                            .Replace("{Config.BannedWordsSettings.Reason}", Config.BannedWordsSettings.Reason);

                        Server.ExecuteCommand($"css_silence #{player.UserId} {Config.BannedWordsSettings.Duration} {Config.BannedWordsSettings.Reason}");
                        player.PrintToChat(silenceMessagePlayer);
                        Server.PrintToChatAll(silenceMessageServer);
                    }
                    return HookResult.Handled;
                }
            }
            return HookResult.Continue;
        }

        private HookResult OnPlayerChatAll(CCSPlayerController? player, CommandInfo message)
        {
            if (Config.BannedWordsSettings.Enabled)
            {
                if (player == null || !player.IsValid || player.IsBot || string.IsNullOrEmpty(message.GetArg(1))) return HookResult.Handled;

                string arg = message.GetArg(1).ToLower();
                if (Config.BannedWords.Any(word => arg.Contains(word.ToLower())))
                {
                    string steamid = player.SteamID.ToString();
                    string playername = player.PlayerName;
                    _ = Task.Run(async () => await Discord(steamid, playername, arg));
                    if (Config.BannedWordsSettings.SilencePlayer)
                    {
                        string silenceMessagePlayer = Config.BannedWordsSettings.SilenceMessagePlayer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{Config.BannedWordsSettings.Duration}", Config.BannedWordsSettings.Duration.ToString())
                            .Replace("{Config.BannedWordsSettings.Reason}", Config.BannedWordsSettings.Reason);

                        string silenceMessageServer = Config.BannedWordsSettings.SilenceMessageServer
                            .Replace("{ChatColors.Red}", $"{ChatColors.Red}")
                            .Replace("{ChatColors.Default}", $"{ChatColors.Default}")
                            .Replace("{player.PlayerName}", player.PlayerName)
                            .Replace("{Config.BannedWordsSettings.Reason}", Config.BannedWordsSettings.Reason);

                        Server.ExecuteCommand($"css_silence #{player.UserId} {Config.BannedWordsSettings.Duration} {Config.BannedWordsSettings.Reason}");
                        player.PrintToChat(silenceMessagePlayer);
                        Server.PrintToChatAll(silenceMessageServer);
                    }
                    return HookResult.Handled;
                }
            }
            return HookResult.Continue;
        }
    }
}
