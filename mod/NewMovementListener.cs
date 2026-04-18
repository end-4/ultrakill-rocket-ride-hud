using HarmonyLib;
using System;
using UnityEngine;

namespace RocketRideHUD
{
    public class NewMovementListener : MonoBehaviour
    {
        public static NewMovementListener Instance { get; private set; }
        public const int RocketRidesUntilUseless = 5;

        public static event OnWallJumpsChangedDelegate OnWallJumpsChanged;
        public static event OnRocketRideStartedDelegate OnRocketRideStarted;
        public static event OnRocketRideEndedDelegate OnRocketRideEnded;
        public static event OnRocketRideCountChangedDelegate OnRocketRideCountChanged;

        public delegate void OnWallJumpsChangedDelegate(int number);
        public delegate void OnRocketRideStartedDelegate();
        public delegate void OnRocketRideEndedDelegate();
        public delegate void OnRocketRideCountChangedDelegate(int count);

        private NewMovement nm;
        private Traverse nmT;

        private int previousWallJumps;
        private bool previousRocketRideYes = false;
        private int previousRocketRideCount = RocketRidesUntilUseless;

        private void Awake()
        {
            if (Instance != null && Instance != this) return;
            Instance = this;

            nm = NewMovement.Instance;
            nmT = Traverse.Create(nm);

            previousWallJumps = nm.currentWallJumps;

            // Try to initialize previousRocketRideCount from the game's field if present
            try
            {
                previousRocketRideCount = nmT.Field<int>("rocketRides").Value;
            }
            catch { previousRocketRideCount = RocketRidesUntilUseless; }
            // Try to initialize previousRocketRideYes from common boolean field names
            try { previousRocketRideYes = nmT.Field<bool>("isRidingRocket").Value; } catch { try { previousRocketRideYes = nmT.Field<bool>("isRiding").Value; } catch { previousRocketRideYes = false; } }

        }

        private void Update()
        {
            if (nm == null) return;

            // Wall jump stuff
            if (previousWallJumps != nm.currentWallJumps)
            {
                if (OnWallJumpsChanged != null) OnWallJumpsChanged(3 - nm.currentWallJumps);
                previousWallJumps = nm.currentWallJumps;
                //Core.Logger.LogInfo($"Wall jumps changed. {previous}");
            }

            // Rocket ride stuff
            // Try to detect an integer ride counter on NewMovement (field found via decompiler: "rocketRides")
            int currentCount = -1;
            try
            {
                currentCount = nmT.Field<int>("rocketRides").Value;
            }
            catch { }

            if (currentCount >= 0)
            {
                if (currentCount != previousRocketRideCount)
                {
                    //Core.Logger.LogInfo($"NewMovementListener detected rocketRides change. current={currentCount} previous={previousRocketRideCount}");
                    // expose remaining usable rides as 5 - used, clamped to >= 0
                    int exposed = Mathf.Max(0, RocketRidesUntilUseless - currentCount);
                    //Core.Logger.LogInfo($"NewMovementListener exposing remaining rides: {exposed}");
                    if (OnRocketRideCountChanged != null) OnRocketRideCountChanged(exposed);
                    previousRocketRideCount = currentCount;
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

            if (currentIsRiding != previousRocketRideYes)
            {
                //Core.Logger.LogInfo($"NewMovementListener detected isRiding change. current={currentIsRiding} previous={previousRocketRideYes}");
                if (currentIsRiding && OnRocketRideStarted != null) OnRocketRideStarted();
                if (!currentIsRiding && OnRocketRideEnded != null) OnRocketRideEnded();
                previousRocketRideYes = currentIsRiding;
            }
        }
    }
}
