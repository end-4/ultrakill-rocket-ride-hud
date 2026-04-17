using HarmonyLib;

namespace WallJumpHUD.Patches
{
    [HarmonyPatch(typeof(NewMovement), "Start")]
    public class NewMovement_Start_RocketRide_Patch
    {
        public static void Postfix(NewMovement __instance)
        {
            __instance.gameObject.AddComponent<WallJumpHUD.RocketRideListener>();
        }
    }
}
