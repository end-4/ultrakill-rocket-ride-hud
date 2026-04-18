using HarmonyLib;
using System;
using UnityEngine;

namespace RocketRideHUD {
    public class NewMovementListener : MonoBehaviour {
        public static NewMovementListener Instance { get; private set; }

        public static event OnWallJumpsChangedDelegate OnWallJumpsChanged;
        public static event OnRocketRideCountChangedDelegate OnRocketRideCountChanged;

        public delegate void OnWallJumpsChangedDelegate(int number);
        public delegate void OnRocketRideCountChangedDelegate(int count);

        private NewMovement nm;
        private Traverse nmT;

        private int previousWallJumps;
        private int previousRocketRideCount = Core.MaxRocketRides;

        private void Awake() {
            if (Instance != null && Instance != this) return;
            Instance = this;

            nm = NewMovement.Instance;
            nmT = Traverse.Create(nm);

            previousWallJumps = nm.currentWallJumps;

            // Try to initialize previousRocketRideCount from the game's field if present
            try {
                previousRocketRideCount = nmT.Field<int>("rocketRides").Value;
            } catch { previousRocketRideCount = Core.MaxRocketRides; }

        }

        private void Update() {
            if (nm == null) return;

            // Wall jump stuff
            if (previousWallJumps != nm.currentWallJumps) {
                if (OnWallJumpsChanged != null) OnWallJumpsChanged(3 - nm.currentWallJumps);
                previousWallJumps = nm.currentWallJumps;
                //Core.Logger.LogInfo($"Wall jumps changed. {previous}");
            }

            // Rocket ride stuff
            // Try to detect an integer ride counter on NewMovement (field found via decompiler: "rocketRides")
            int currentCount = -1;
            try {
                currentCount = nmT.Field<int>("rocketRides").Value;
            } catch { }

            if (currentCount >= 0) {
                if (currentCount != previousRocketRideCount) {
                    if (OnRocketRideCountChanged != null) OnRocketRideCountChanged(currentCount);
                    previousRocketRideCount = currentCount;
                }

                return;
            }
        }
    }
}
