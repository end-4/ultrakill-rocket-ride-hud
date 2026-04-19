# Rocket ride indicators

- Remaining rocket rides until they're ineffective (max 5-ish, see [ULTRAKILL Wiki](https://ultrakill.fandom.com/wiki/Freezeframe_Rocket_Launcher#Chain_Surfing))
- Remaining rocket fuel until it droops down
- Angle hint for Freeze-Dash+shoot-Unfreeze ride method
- Also still has wall jump indicator from the [original mod](https://github.com/TRPG0/UK-WallJumpHUD) because... why not
- The colors can be customized

<img alt="image" src="https://github.com/user-attachments/assets/6ee07e9b-c1b8-48d5-80c4-b528a13fe619" />

<br>

# Installation

- Normies: Search for this mod in the Online tab of r2modman. [Thunderstore page](https://thunderstore.io/c/ultrakill/p/end_4/RocketRideHUD/)
- Nerds: See below for manual building/installation

# Building

## Auto-ish

1. **Grab dependencies**: Run `setup-libs.ps1`, paying attention to the paths:
    - If your Steam/Ultrakill instance and/or r2modman are installed at a non-default location, specify their paths manually, something like this: `.\setup-libs.ps1 -UltrakillPath "D:\steam gamez\steamapps\common\ULTRAKILL" -R2ModmanProfilePath "E:\someFolder"`
    - To find the path for Ultrakill and R2Modman profile, see the manual build instructions
2. **Build & package**: Run `make-package.ps1`. A zip file will be generated. Import it in r2modman settings.

## Manual

0. Note: by `/` I mean `\`, though they should work the same on Windows 11
1. Create `mod/libs/Managed`
2. Open ULTRAKILL's folder: Steam library -> ULTRAKILL -> click on gear icon, "Manage" > "Browse local files"
3. Find the following DLL files in `ULTRAKILL_Data/Managed` and copy to the folder you created:
    - Assembly-CSharp.dll
    - Unity.TextMeshPro.dll
    - UnityEngine.UI.dll
4. Install the PluginConfigurator mod
5. Open mods folder: Go to r2modman settings > "Browse profile folder", head to `BepInEx/plugins`
6. Copy `EternalsTeam-PluginConfigurator/PluginConfigurator/PluginConfigurator.dll` to the folder you created
7. In the `mods` folder of the repo, run `dotnet build -c Release`
8. Package stuff:
    - Duplicate the `package` folder to into, say, `package_build`
    - Copy the compiled DLL from `mod/bin/Release/net472/RocketRideHUD.dll` to that new folder
    - In `package_build`, create `BepInEx/plugins` then copy the `assets` folder from the repo's root there
    - Zip the CONTENTS of `package_build`. The resulting zip file should not contain a folder of the same name.

## Local install

Open R2Modman, select your game profile, go to Settings > Import local mod, then select the zip file you created

# Notes

- Thank you TRPG0 for the original mod [WallJumpHUD](https://github.com/TRPG0/UK-WallJumpHUD)
- AI use: Does most of the "core" work, went through lenient human review. Human-reorganized and tested.
- Why are sprites loaded from files not asset bundles? I'm too lazy to install Unity editor for now... I need it to edit this right?
