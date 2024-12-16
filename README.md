# Custom Rundown Progression

After the plugin is built, placed in the plugin folder of BepInEx in R2Modman, run the game to generate the main *RundownProgression.json* file in the "Custom" folder in the datablocks. Upon Launching the game, a folder is created in AppData\Roaming\GTFO-Modding named "MCProgression_Rundowns". This is where the progression is stored for each rundown. Since no unique modifier is present for the names, it can lead to issues if two rundowns share the same name, but why would they? The clears are in pure JSON, and can be easily modified. Now for the rundown progression JSON file.

An full entry in the rundown progression JSON looks like this
```
[
  {
    "RundownName": "INTERNAL_NAME",
    "RundownID": 0,
    "HideTierMarkers": false,

    "TierARequirements": {
      "MainSectors": 0,
      "SecondarySectors": 0,
      "ThirdSectors": 0,
      "AllClearedSectors": 0
    },
    "TierBRequirements": {
      "MainSectors": 0,
      "SecondarySectors": 0,
      "ThirdSectors": 0,
      "AllClearedSectors": 0
    },
    "TierCRequirements": {
      "MainSectors": 0,
      "SecondarySectors": 0,
      "ThirdSectors": 0,
      "AllClearedSectors": 0
    },
    "TierDRequirements": {
      "MainSectors": 0,
      "SecondarySectors": 0,
      "ThirdSectors": 0,
      "AllClearedSectors": 0
    },
    "TierERequirements": {
      "MainSectors": 0,
      "SecondarySectors": 0,
      "ThirdSectors": 0,
      "AllClearedSectors": 0
    },

    "CustomTierRequirements": [
      {
        "Expedition": {
          "ExpeditionIndex": 0,
          "ExpeditionTier": 0
        },
        "ChangePosition": false,
        "NewPosition": "(0 0 0)",
        "LockData": {
          "HideExpedition": false,
          "LockType": 0,
          "Requirements": [
            {
              "NeededExpeditionClears": [
                {
                  "Expedition": {
                    "ExpeditionIndex": 0,
                    "ExpeditionTier": 0
                  },
                  "NeededClears": {
                    "HighClears": 0,
                    "SecondaryClears": 0,
                    "OverloadClears": 0,
                    "PEClears": 0
                  }
                }
              ],
              "MakeExpeditionInvisible": false,
              "DecryptionState": 0,
              "ChangeLockText": false,
              "DecryptedText": "",
              "ForceUnlock": false,
              "Priority": -1
            }
          ]
        },
        "WardenEventsOnLand": [
          {
            "ExpeditionRequirements": [
              {
                "Expedition": {
                  "ExpeditionIndex": 0,
                  "ExpeditionTier": 0
                },
                "NeededClears": {
                  "HighClears": 0,
                  "SecondaryClears": 0,
                  "OverloadClears": 0,
                  "PEClears": 0
                }
              }
            ],
            "WardenEvents": []
          }
        ]
      }
    ]
  }
]
```
NOTABLE VARIABLES:  
`RundownName` - Needs to match the "InternalName" from the RundownDataBlock.  
`RundownID` - PersistentID of the rundown. 
`HideTierMarkers` - Hide the tier markers on the rundown?  
`Tier$Requirements` - Same as the base game, used when comparing expedition data to the tier requirement. 
  
`Expedition`  
&nbsp;&nbsp;&ensp;|  
&nbsp;&nbsp;&ensp;---- `ExpeditionIndex` - An integer value of the expedition index (starts at 0).  
&nbsp;&nbsp;&ensp;|  
&nbsp;&nbsp;&ensp;---- `ExpeditionTier` - Corresponds to the tier of the expedition (see enums section).  
        
`ChangePosition` - Uses the `NewPosition` vector to change the local position of the expedition icon.  
`NewPosition` - As mentioned above, this is the new position of the expedition icon (in local space).  
  
`LockData`  
&nbsp;&nbsp;&ensp;|  
&nbsp;&nbsp;&ensp;---- `LockType` - Determines how the expedition is locked (see enums section)  
&nbsp;&nbsp;&ensp;---- `DecryptionState` - Sets the Encryption/Decrypton state of the expedition icon (see enums section).  
&nbsp;&nbsp;&ensp;---- `Priority` - An integer. If the priority of an event is lower than the highest priority, it will not run. The highest priority starts at -1, checks if the current priority is lower than it, returns if it is true or sets the highestPriority equal to the new priority.  

ENUM VALUES:  
`DecryptionState`  
&nbsp;&nbsp;&ensp;| 0 = Default (NoDecryption)  
&nbsp;&nbsp;&ensp;| 1 = SetDecrypted  
&nbsp;&nbsp;&ensp;| 2 = UnlockDecrypted (used to fix the names or issues with changing the scrambled expeditions)  
`LockType`  
&nbsp;&nbsp;&ensp;| 0 = Default  
&nbsp;&nbsp;&ensp;| 1 = UnlockedByTierClears  
&nbsp;&nbsp;&ensp;| 2 = UnlockedByOtherExpeditions  
`ExpeditionTier`  
&nbsp;&nbsp;&ensp;| 1 = TierA  
&nbsp;&nbsp;&ensp;| 2 = TierB  
&nbsp;&nbsp;&ensp;| 3 = TierC  
&nbsp;&nbsp;&ensp;| 4 = TierD  
&nbsp;&nbsp;&ensp;| 5 = TierE  
