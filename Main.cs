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

namespace ExtraUtilities;

public partial class ExtraUtilities : BasePlugin
{
    
    public override string ModuleName => "ExtraUtilities";
    public override string ModuleAuthor => "heartbreakhotel";
    public override string ModuleDescription => "Additional server utilities";
    public override string ModuleVersion => "0.0.2";

    public const string ConfigFileName = "config.json";
    public string GameDir = string.Empty;
    public Config? Configuration;

    public override void Load(bool hotReload)
    {
       // Logger.LogInformation("Spin detection started");

        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
        RegisterListener<Listeners.OnMapStart>(OnMapStart);
        RegisterListener<Listeners.OnTick>(OnTick);

        AddCommandListener("say", OnPlayerChatAll);
        AddCommandListener("say_team", OnPlayerChatTeam);

    }

}
