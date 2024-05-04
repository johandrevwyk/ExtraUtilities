using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace ExtraUtilities;

public partial class ExtraUtilities : BasePlugin, IPluginConfig<UtilitiesConfig>
{
    
    public override string ModuleName => "ExtraUtilities";
    public override string ModuleAuthor => "heartbreakhotel";
    public override string ModuleDescription => "Additional server utilities";
    public override string ModuleVersion => "0.1.3";

    string _hostname = "Not Set";

    private Dictionary<int, CCSPlayerController> connectedPlayers = new Dictionary<int, CCSPlayerController>();

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterEventHandler<EventRoundPrestart>(OnRoundPrestart);        
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnTick>(OnTick);

        AddCommandListener("say", OnPlayerChatAll);
        AddCommandListener("say_team", OnPlayerChatTeam);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook((h) =>
            {
                var damageInfo = h.GetParam<CTakeDamageInfo>(1);

                // attacker is invalid
                if (damageInfo.Attacker.Value == null)
                    return HookResult.Continue;

                // attacker is not in the list
                if (!_rapidFireBlockUserIds.Contains(damageInfo.Attacker.Index))
                    return HookResult.Continue;

                return HookResult.Changed;
            }, HookMode.Pre);
        } else
        {
            Logger.LogInformation("Detected Windows OS, disabling Rapid Fire");
        }
    }

}
