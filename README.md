# Rocket ride indicators

- Remaining rocket rides until they're ineffective (max 5-ish, see [ULTRAKILL Wiki](https://ultrakill.fandom.com/wiki/Freezeframe_Rocket_Launcher#Chain_Surfing))
- Angle hint for Freeze-Dash+shoot-Unfreeze ride method
- Also still has wall jump indicator from the [original mod](https://github.com/TRPG0/UK-WallJumpHUD) because... why not
- The colors can be customized

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/fe7fb4e9-bbef-4c45-982a-3b44d2a3b6cb" />

<br>

# Manual build/installation

0. In the rare case a non-nerd actually compiles from source: by `/` I mean `\` on Windows
1. Create `mod/libs/Managed`
2. Open ULTRAKILL's folder: Steam library -> ULTRAKILL -> click on gear icon, "Manage" > "Browse local files"
3. Find the following DLL files in `ULTRAKILL_Data/Managed` and copy to the folder you created:
    - Assembly-CSharp.dll
    - Unity.TextMeshPro.dll
    - UnityEngine.UI.dll
4. Install the PluginConfigurator mod
5. Open mods folder: Go to r2modman settings > "Browse profile folder", head to `BepInEx/plugins`
6. Copy `EternalsTeam-PluginConfigurator/PluginConfigurator/PluginConfigurator.dll` to the folder you created
7. From the root folder of the repo run `make-package.ps1`. A zip file will be generated. Import it in r2modman settings.

# Notes

- Thank you TRPG0 for the original mod [WallJumpHUD](https://github.com/TRPG0/UK-WallJumpHUD)
- AI use: Does most of the "core" work, went through lenient human review. Human-reorganized and tested.
- Why are sprites loaded from files not asset bundles? I'm too lazy to install Unity editor for now... I need it to edit this right?
