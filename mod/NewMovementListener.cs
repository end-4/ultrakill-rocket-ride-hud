﻿using HarmonyLib;
using System;
using UnityEngine;

namespace RocketRideHUD {
    public class NewMovementListener : MonoBehaviour {
        public static NewMovementListener Instance { get; private set; }

        public static event OnWallJumpsChangedDelegate OnWallJumpsChanged;
        public static event OnRocketRideCountChangedDelegate OnRocketRideCountChanged;
        public static event OnRocketFuelChangedDelegate OnRocketFuelChanged;

        public delegate void OnWallJumpsChangedDelegate(int number);
        public delegate void OnRocketRideCountChangedDelegate(int count);
        public delegate void OnRocketFuelChangedDelegate(float fuelAmount);

        private NewMovement nm;

        private int previousWallJumps;
        private int previousRocketRideCount = Core.MaxRocketRides;

        private void Awake() {
            if (Instance != null && Instance != this) return;
            Instance = this;

            nm = NewMovement.Instance;
            previousWallJumps = nm.currentWallJumps;
            previousRocketRideCount = nm.rocketRides;
        }

        private void Update() {
            if (nm == null) return;

            // Wall jump count
            if (previousWallJumps != nm.currentWallJumps) {
                if (OnWallJumpsChanged != null) OnWallJumpsChanged(Core.MaxWalljumps - nm.currentWallJumps);
                previousWallJumps = nm.currentWallJumps;
            }

            // Rocket ride count
            int currentCount = -1;
            currentCount = nm.rocketRides;

            if (currentCount >= 0) {
                if (currentCount != previousRocketRideCount) {
                    if (OnRocketRideCountChanged != null) OnRocketRideCountChanged(currentCount);
                    previousRocketRideCount = currentCount;
                }
            }

            // Rocket fuel tracking
            if (nm.ridingRocket != null) {
                // In ULTRAKILL, rocket flight stability is managed by the private 'downpull' field.
                // It starts at -0.5 and the rocket begins drooping once it exceeds 0.
                // We map the range [-0.5, overstay] to [1, 0] so the bar can last slightly into the droop phase.
                float dp = Traverse.Create(nm.ridingRocket).Field<float>("downpull").Value;
                float overstay = ConfigManager.crosshairRocketFuelOverstay.value;
                float fuelAmount = Mathf.InverseLerp(overstay, -0.5f, dp); 
                if (OnRocketFuelChanged != null) OnRocketFuelChanged(fuelAmount);
            } else {
                if (OnRocketFuelChanged != null) OnRocketFuelChanged(0f);
            }
        }
    }
}
