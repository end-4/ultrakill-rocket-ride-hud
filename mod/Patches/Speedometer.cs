using System.Drawing.Printing;
using HarmonyLib;

namespace RocketRideHUD.Patches {
    [HarmonyPatch(typeof(Speedometer), "OnEnable")]
    public class Speedometer_Awake_Patch {
        public static void Postfix(Speedometer __instance) {
            if (SpeedometerListener.Instance == null) return;
            SpeedometerListener.Instance.UpdateState(__instance.gameObject.activeSelf);
        }
    }

    [HarmonyPatch(typeof(Speedometer), "OnPrefChanged")]
    public class Speedometer_OnPrefChanged_Patch {
        public static void Postfix(Speedometer __instance, string id) {
            if (SpeedometerListener.Instance == null) return;
            if (id == "speedometer") SpeedometerListener.Instance.UpdateState(__instance.gameObject.activeSelf);
        }
    }
}
