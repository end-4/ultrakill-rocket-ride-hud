# Rocket ride HUD

- Displays number of rocket rides left before they'll simply drop. You get 5 rides, and the 6th rocket can still be mounted but will only fly downwards
- Useful for Cyber Grind if you absolutely need to cheese the Mirror Reaper like me

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
7. In Visual Studio Dev Powershell, run `cd mod` and then `msbuild /p:Configuration=Release`
8. Find the built DLL of the mod in `mod/bin/Release/net472`. Copy it to the mods folder.

# Notes

- Thank you TRPG0 for the original mod [WallJumpHUD](https://github.com/TRPG0/UK-WallJumpHUD)
- Yes, an LLM was involved. And no, I've never written C# before this
- Why is the sprite for rocket rides in weapon HUD loaded through file not asset bundle? I'm too lazy to install Unity editor for now... I need it to edit this right?
