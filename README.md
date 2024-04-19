
# ExtraUtilities

This plugin provides multiple features such as detecting Spinbot, RapidFire, Bunnyhop and Banned Words

**NOTE: This plugin does not extensively or actively check for a cheater, it rather matches a criteria on player kill with thresholds**

# Requirements

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)\
[CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin) - if you are banning/silencing people

**This plugin uses the css_ban and css_silence format**

# Installation

Extract the zip in ```addons/counterstrikesharp/plugins```\
Modify the config to your liking - ensuring that discord webhook is set

# Config

```json
{
  "General": {
    "Webhook": "", //your discord webhook url
    "MessageTemplate": "@everyone Player: [{playername}]({steamProfileUrl}) is in violation of - {type}" //do not change the variables
  },

  "Bunnyhop": {
    "SpeedLimit": 320, //this is the default walking speed
    "Threshold": 128, //this is pertick
    "DecreasePlayerSpeed": true //this will decrease the playerspeed if the speedlimit is met
  },

  "SpinDetection": { //thresholds before sending to discord and/or banning
    "HeadshotPenetratedNoScope": 3,
    "HeadshotPenetrated": 10,
    "HeadshotSmokePenetratedNoScope": 3,
    "HeadshotSmoke": 8,
    "HeadshotSmokePenetrated": 5,
    "BanPlayer": true
  },

  "RapidFire": {
    "BanPlayer": true
  },

  "BannedWordsSettings": {
    "SilencePlayer": true,
    "Duration": 4080,
    "Reason": "Racism"
  },

  "BannedWords": [
    "word1",
    "word2"
  ]

}
```

Rapid Fire code borrowed from [RapidFireFix by imi-tat0r](https://github.com/HvH-gg/RapidFireFix/)



