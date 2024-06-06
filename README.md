> [!WARNING]  
> This plugin does not extensively or actively check for a cheater, it rather matches a criteria on player kill with thresholds

# ExtraUtilities

This plugin provides multiple features such as detecting Spinbot, RapidFire, Bunnyhop and Banned Words

# Requirements

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)\
[CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin) - if you are banning/silencing people

**This plugin uses the css_ban and css_silence format**

**Rapid Fire will be disabled on Windows due to Damage Hook causing crashes**

# Installation

Extract the zip in ```addons/counterstrikesharp/plugins```\
After launching the plugin for the first time, you will have to modify your config in ```addons/counterstrikesharp/configs/plugins/ExtraUtilities```

# Config

```json
{
  "General": {
    "Webhook": "" //your webhook url
  },

  "Bunnyhop": {
    "Enabled" : true,
    "SpeedLimit": 320,
    "Threshold": 128,
    "DecreasePlayerSpeed": true
  },

  "SpinDetection": { //thresholds for detection
    "Enabled" : true,
    "HeadshotPenetratedNoScope": 3,
    "HeadshotPenetrated": 10,
    "HeadshotSmokePenetratedNoScope": 3,
    "HeadshotSmoke": 8,
    "HeadshotSmokePenetrated": 5,
    "BanPlayer": true
  },

  "RapidFire": {
    "Enabled" : true,
    "BanPlayer": true,
    "Threshold": 3
  },

  "BannedWordsSettings": {
    "Enabled" : true,
    "SilencePlayer": true,
    "Duration": 4080
  },

  "BannedWords": [
    "word1",
    "word2"
  ]

}
```

Rapid Fire code borrowed from [RapidFireFix by imi-tat0r](https://github.com/HvH-gg/RapidFireFix/)



