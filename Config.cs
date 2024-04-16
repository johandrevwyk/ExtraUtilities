using Newtonsoft.Json;
using System.Reflection;

namespace ExtraUtilities;

public class Config
{
    public General General { get; set; }
    public SpinDetection SpinDetection { get; set; }
    public string[] WelcomeMessages { get; set; }
}

public class General
{
    public string Webhook { get; set; }
}
public class SpinDetection
{
    public General General { get; set; }
}