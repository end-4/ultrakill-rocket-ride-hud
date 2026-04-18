# Rocket ride indicator

on the weapon HUD and crosshair

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/7c126d39-9bba-4b93-8d2b-23d113db30ec" />
<br>

- Displays number of rocket rides left before they'll simply drop. You basically get 5 rides; the 6th can still be mounted but you can't really fly upwards. See full details on [ULTRAKILL Wiki](https://ultrakill.fandom.com/wiki/Freezeframe_Rocket_Launcher#Chain_Surfing)
- Useful for Cyber Grind if you absolutely need to cheese the Mirror Reaper like me
- Also still has wall jump indicator from the [original mod](https://github.com/TRPG0/UK-WallJumpHUD) because... why not

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
- Yes, an LLM was involved. And no, I've never written C# before this
- Why is the sprite for rocket rides in weapon HUD loaded through file not asset bundle? I'm too lazy to install Unity editor for now... I need it to edit this right?
