using HarmonyLib;
using System;
using UnityEngine;

namespace WallJumpHUD
{
    public class RocketRideListener : MonoBehaviour
    {
        public static RocketRideListener Instance { get; private set; }

        public static event OnRocketRideStartedDelegate OnRocketRideStarted;
        public static event OnRocketRideEndedDelegate OnRocketRideEnded;
        public static event OnRocketRideCountChangedDelegate OnRocketRideCountChanged;

        public delegate void OnRocketRideStartedDelegate();
        public delegate void OnRocketRideEndedDelegate();
        public delegate void OnRocketRideCountChangedDelegate(int count);

        private NewMovement nm;
        private Traverse nmT;

        private bool previousIsRiding = false;
        private int previousCount = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) return;
            Instance = this;

            nm = NewMovement.Instance;
            nmT = Traverse.Create(nm);

            // Try to initialize previousCount from the game's field if present
            try
            {
                previousCount = nmT.Field<int>("rocketRides").Value;
            }
            catch { previousCount = 0; }

            // Try to initialize previousIsRiding from common boolean field names
            try { previousIsRiding = nmT.Field<bool>("isRidingRocket").Value; } catch { try { previousIsRiding = nmT.Field<bool>("isRiding").Value; } catch { previousIsRiding = false; } }

            //Core.Logger.LogInfo($"RocketRideListener Awake. previousCount={previousCount} previousIsRiding={previousIsRiding}");
        }

        private void Update()
        {
            if (nm == null) return;

            // Try to detect an integer ride counter on NewMovement (field found via decompiler: "rocketRides")
            int currentCount = -1;
            try
            {
                currentCount = nmT.Field<int>("rocketRides").Value;
            }
            catch { }

            if (currentCount >= 0)
            {
                if (currentCount != previousCount)
                {
                    //Core.Logger.LogInfo($"RocketRideListener detected rocketRides change. current={currentCount} previous={previousCount}");
                    // expose remaining usable rides as 5 - used, clamped to >= 0
                    int exposed = Mathf.Max(0, 5 - currentCount);
                    //Core.Logger.LogInfo($"RocketRideListener exposing remaining rides: {exposed}");
                    if (OnRocketRideCountChanged != null) OnRocketRideCountChanged(exposed);
                    previousCount = currentCount;
                }

                return;
            }

            // If no integer counter found, try to detect a boolean "is riding" field
            bool currentIsRiding = false;
            try
            {
                // try common candidate names; replace/add names as you find them
                currentIsRiding = nmT.Field<bool>("isRidingRocket").Value;
            }
            catch
            {
                try { currentIsRiding = nmT.Field<bool>("isRiding").Value; } catch { }
                try { if (!currentIsRiding) currentIsRiding = nmT.Field<bool>("ridingRocket").Value; } catch { }
            }

            if (currentIsRiding != previousIsRiding)
            {
                //Core.Logger.LogInfo($"RocketRideListener detected isRiding change. current={currentIsRiding} previous={previousIsRiding}");
                if (currentIsRiding && OnRocketRideStarted != null) OnRocketRideStarted();
                if (!currentIsRiding && OnRocketRideEnded != null) OnRocketRideEnded();
                previousIsRiding = currentIsRiding;
            }
        }
    }
}
