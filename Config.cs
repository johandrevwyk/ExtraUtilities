namespace ExtraUtilities;

public class Config
{
    public General General { get; set; }
    public SpinDetection SpinDetection { get; set; }
    public Bunnyhop Bunnyhop { get; set; }
    public RapidFire RapidFire { get; set; }
    public BannedWordsSettings BannedWordsSettings { get; set; }
    public string[] BannedWords { get; set; }
}

public class General
{
    public string Webhook { get; set; }
    public string MessageTemplate { get; set; }
}

public class SpinDetection
{
    public bool Enabled { get; set; }
    public int HeadshotPenetratedNoScope { get; set; }
    public int HeadshotPenetrated { get; set; }
    public int HeadshotSmokePenetratedNoScope { get; set; }
    public int HeadshotSmoke { get; set; }
    public int HeadshotSmokePenetrated { get; set; }
    public bool BanPlayer { get; set; }
    public string BanMessagePlayer { get; set; }
    public string BanMessageServer { get; set; }
    public string BanReason { get; set; }
}

public class Bunnyhop
{
    public bool Enabled { get; set; }
    public float SpeedLimit { get; set; }
    public int Threshold { get; set; }
    public bool DecreasePlayerSpeed { get; set; }  
}

public class RapidFire
{
    public bool Enabled { get; set; }
    public bool BanPlayer { get; set; }
    public string BanMessagePlayer { get; set; }
    public string BanMessageServer { get; set; }
    public string BanReason { get; set; }
    public int Threshold { get; set; }
}

public class BannedWordsSettings
{
    public bool Enabled { get; set; }
    public bool SilencePlayer { get; set; }
    public int Duration { get; set; }
    public string SilenceMessagePlayer { get; set; }
    public string SilenceMessageServer { get; set; }
    public string Reason { get; set; }
}