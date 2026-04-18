using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RocketRideHUD
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.eternalUnion.pluginConfigurator")]
    public class Core : BaseUnityPlugin
    {
        public const string PluginGUID = "com.github.end-4.rocketRideHud";
        public const string PluginName = "RocketRideHUD";
        public const string PluginVersion = "1.0.2";

        public static string workingPath;
        public static string workingDir;

        public static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("RocketRideHUD");

        public static AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.trpg_walljumphud);

        public static int MaxWalljumps
        {
            get
            {
                return 3;
            }
        }

        public static Color WeaponColor
        {
            get
            {
                return ConfigManager.weaponWallJumpColor.value;
            }
        }

        public static Color CrosshairColor
        {
            get
            {
                return ConfigManager.crosshairWallJumpColor.value;
            }
        }

        private void Awake()
        {
            Harmony Harmony = new Harmony("RocketRideHUD");
            Harmony.PatchAll();

            workingPath = Assembly.GetExecutingAssembly().Location;
            workingDir = Path.GetDirectoryName(workingPath);

            ConfigManager.Init();
        }
    }
}
