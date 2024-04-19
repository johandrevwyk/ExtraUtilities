using System.Numerics;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Memory;

namespace ExtraUtilities;

public partial class ExtraUtilities : BasePlugin
{
    
    public override string ModuleName => "ExtraUtilities";
    public override string ModuleAuthor => "heartbreakhotel";
    public override string ModuleDescription => "Additional server utilities";
    public override string ModuleVersion => "0.0.3";

    public const string ConfigFileName = "config.json";
    public string GameDir = string.Empty;
    public Config? Configuration;

    private Dictionary<int, CCSPlayerController> connectedPlayers = new Dictionary<int, CCSPlayerController>();

    public override void Load(bool hotReload)
    {
        LoadConfig();

        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterEventHandler<EventRoundPrestart>(OnRoundPrestart);
        RegisterEventHandler<EventWeaponFire>(OnWeaponFire);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnTick>(OnTick);

        AddCommandListener("say", OnPlayerChatAll);
        AddCommandListener("say_team", OnPlayerChatTeam);

        //RAPID FIRE
        // block damage if attacker is in the list
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

    }

}
