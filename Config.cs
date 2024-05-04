using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace ExtraUtilities
{
    public class General
    {
        [JsonPropertyName("Webhook")]
        public string Webhook { get; set; } = "";

        [JsonPropertyName("MessageTemplate")]
        public string MessageTemplate { get; set; } = "@everyone Player: [{playername}]({steamProfileUrl}) is in violation of - {type}";
    }

    public class Bunnyhop
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("SpeedLimit")]
        public float SpeedLimit { get; set; } = 320;

        [JsonPropertyName("Threshold")]
        public int Threshold { get; set; } = 128;

        [JsonPropertyName("DecreasePlayerSpeed")]
        public bool DecreasePlayerSpeed { get; set; } = true;
    }

    public class SpinDetection
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("HeadshotPenetratedNoScope")]
        public int HeadshotPenetratedNoScope { get; set; } = 3;

        [JsonPropertyName("HeadshotPenetrated")]
        public int HeadshotPenetrated { get; set; } = 10;

        [JsonPropertyName("HeadshotSmokePenetratedNoScope")]
        public int HeadshotSmokePenetratedNoScope { get; set; } = 3;

        [JsonPropertyName("HeadshotSmoke")]
        public int HeadshotSmoke { get; set; } = 8;

        [JsonPropertyName("HeadshotSmokePenetrated")]
        public int HeadshotSmokePenetrated { get; set; } = 5;

        [JsonPropertyName("BanPlayer")]
        public bool BanPlayer { get; set; } = true;

        [JsonPropertyName("BanMessagePlayer")]
        public string BanMessagePlayer { get; set; } = " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord";

        [JsonPropertyName("BanMessageServer")]
        public string BanMessageServer { get; set; } = " {ChatColors.Red}[Server] - {attackerController.PlayerName} {ChatColors.Default}has automatically been banned due to cheating";

        [JsonPropertyName("BanReason")]
        public string BanReason { get; set; } = "Cheating";
    }

    public class RapidFire
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("BanPlayer")]
        public bool BanPlayer { get; set; } = true;

        [JsonPropertyName("Threshold")]
        public int Threshold { get; set; } = 3;

        [JsonPropertyName("BanMessagePlayer")]
        public string BanMessagePlayer { get; set; } = " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord";

        [JsonPropertyName("BanMessageServer")]
        public string BanMessageServer { get; set; } = " {ChatColors.Red}[Server] - {attackerController.PlayerName} {ChatColors.Default}has automatically been banned due to cheating";

        [JsonPropertyName("BanReason")]
        public string BanReason { get; set; } = "Cheating";
    }

    public class BannedWordsSettings
    {
        [JsonPropertyName("Enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("SilencePlayer")]
        public bool SilencePlayer { get; set; } = true;

        [JsonPropertyName("Duration")]
        public int Duration { get; set; } = 4080;

        [JsonPropertyName("SilenceMessagePlayer")]
        public string SilenceMessagePlayer { get; set; } = " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been silenced for {Config.BannedWordsSettings.Duration} minutes due to {Config.BannedWordsSettings.Reason}";

        [JsonPropertyName("SilenceMessageServer")]
        public string SilenceMessageServer { get; set; } = " {ChatColors.Red}[Server] - {player.PlayerName} {ChatColors.Default}has automatically been silenced due to {Config.BannedWordsSettings.Reason}";

        [JsonPropertyName("Reason")]
        public string Reason { get; set; } = "Racism";
    }

    public class UtilitiesConfig : BasePluginConfig
    {
        [JsonPropertyName("General")]
        public General General { get; set; } = new General();

        [JsonPropertyName("Bunnyhop")]
        public Bunnyhop Bunnyhop { get; set; } = new Bunnyhop();

        [JsonPropertyName("SpinDetection")]
        public SpinDetection SpinDetection { get; set; } = new SpinDetection();

        [JsonPropertyName("RapidFire")]
        public RapidFire RapidFire { get; set; } = new RapidFire();

        [JsonPropertyName("BannedWordsSettings")]
        public BannedWordsSettings BannedWordsSettings { get; set; } = new BannedWordsSettings();

        [JsonPropertyName("BannedWords")]
        public string[] BannedWords { get; set; } = ["word1", "word2"];

        [JsonPropertyName("ConfigVersion")] public override int Version { get; set; } = 1;
    }

    public partial class ExtraUtilities : BasePlugin, IPluginConfig<UtilitiesConfig>
    {
        public required UtilitiesConfig Config { get; set; }

        public void OnConfigParsed(UtilitiesConfig config)
        {
            Console.WriteLine("Config Loaded Succesfully");
            Config = config;

            if (Config.Version != 1) throw new Exception("Config version mismatch");

            if (string.IsNullOrEmpty(Config.General.Webhook))
            {
                throw new Exception("Webhook needs to be set");
            }

        }
    }

}

