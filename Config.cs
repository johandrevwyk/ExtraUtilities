﻿using Newtonsoft.Json;
using System.Reflection;

namespace ExtraUtilities;

public class Config
{
    public General General { get; set; }
    public SpinDetection SpinDetection { get; set; }
    public Bunnyhop Bunnyhop { get; set; }
    public string[] BannedWords { get; set; }
}

public class General
{
    public string Webhook { get; set; }
}

public class SpinDetection
{
    public int HeadshotPenetratedNoScope { get; set; }
    public int HeadshotPenetrated { get; set; }
    public int HeadshotSmokePenetratedNoScope { get; set; }
    public int HeadshotSmoke { get; set; }
    public int HeadshotSmokePenetrated { get; set; }
    public bool BanPlayer { get; set; }
}

public class Bunnyhop
{
    public float SpeedLimit { get; set; }
    public int Threshold { get; set; }
    public bool DecreasePlayerSpeed { get; set; }  
}