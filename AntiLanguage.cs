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
                        string silenceMessagePlayer = Chat.FormatMessage(Localizer["silencemsgplayer", Config.BannedWordsSettings.Duration.ToString(), Localizer["silencereason"]]);
                        string silenceMessageServer = Chat.FormatMessage(Localizer["silencemsgserver", player.PlayerName, Localizer["silencereason"]]);

                        Server.ExecuteCommand($"css_silence #{player.UserId} {Config.BannedWordsSettings.Duration} {Localizer["silencereason"]}");
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
                        string silenceMessagePlayer = Chat.FormatMessage(Localizer["silencemsgplayer", Config.BannedWordsSettings.Duration.ToString(), Localizer["silencereason"]]);
                        string silenceMessageServer = Chat.FormatMessage(Localizer["silencemsgserver", player.PlayerName, Localizer["silencereason"]]);

                        Server.ExecuteCommand($"css_silence #{player.UserId} {Config.BannedWordsSettings.Duration} {Localizer["silencereason"]}");
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
