
# ExtraUtilities

This plugin provides multiple features such as detecting Spinbot, RapidFire, Bunnyhop and Banned Words

**NOTE: This plugin does not extensively or actively check for a cheater, it rather matches a criteria on player kill with thresholds**

# Requirements

[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)\
[CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin) - if you are banning/silencing people

**This plugin uses the css_ban and css_silence format**

**THIS PLUGIN WILL NOT WORK ON WINDOWS AND WILL CAUSE SERVER CRASHES**

# Installation

Extract the zip in ```addons/counterstrikesharp/plugins```\
Modify the config to your liking - ensuring that discord webhook is set

# Config

```json
{
  "General": {
    "Webhook": "", //your webhook url
    "MessageTemplate": "@everyone Player: [{playername}]({steamProfileUrl}) is in violation of - {type}" //do not change the variables
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
    "BanPlayer": true,
    "BanMessagePlayer": " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord", //do not change the colors or variables, only the text
    "BanMessageServer": " {ChatColors.Red}[Server] - {attackerController.PlayerName} {ChatColors.Default}has automatically been banned due to cheating",
    "BanReason": "Cheating"
  },

  "RapidFire": {
    "Enabled" : true,
    "BanPlayer": true,
    "Threshold": 3,
    "BanMessagePlayer": " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been banned due to cheating, if you think this was a mistake, appeal on the discord", //do not change the colors or variables, only the text
    "BanMessageServer": " {ChatColors.Red}[Server] - {attackerController.PlayerName} {ChatColors.Default}has automatically been banned due to cheating",
    "BanReason": "Cheating"
  },

  "BannedWordsSettings": {
    "Enabled" : true,
    "SilencePlayer": true,
    "Duration": 4080,
    "SilenceMessagePlayer": " {ChatColors.Red}[Server] - {ChatColors.Default}You have automatically been silenced for {Config.BannedWordsSettings.Duration} minutes due to {Config.BannedWordsSettings.Reason}",
    "SilenceMessageServer": " {ChatColors.Red}[Server] - {player.PlayerName} {ChatColors.Default}has automatically been silenced due to {Config.BannedWordsSettings.Reason}",
    "Reason": "Racism"
  },

  "BannedWords": [
    "word1",
    "word2"
  ]

}
```

PS: I'm too lazy to deal with colors, so you will have to stick with red or keep it white

Rapid Fire code borrowed from [RapidFireFix by imi-tat0r](https://github.com/HvH-gg/RapidFireFix/)



