using System;
using UnityEngine;

namespace WallJumpHUD
{
    public class NewMovementListener : MonoBehaviour
    {
        public static NewMovementListener Instance { get; private set; }

        public static event OnWallJumpsChangedDelegate OnWallJumpsChanged;

        public delegate void OnWallJumpsChangedDelegate(int number);

        private int previousWallJumps;

        private void Awake()
        {
            if (Instance != null && Instance != this) return;
            Instance = this;
            previousWallJumps = NewMovement.Instance.currentWallJumps;
        }

        private void Update()
        {
            if (previousWallJumps != NewMovement.Instance.currentWallJumps)
            {
                if (OnWallJumpsChanged != null) OnWallJumpsChanged(3 - NewMovement.Instance.currentWallJumps);
                previousWallJumps = NewMovement.Instance.currentWallJumps;
                //Core.Logger.LogInfo($"Wall jumps changed. {previous}");
            }
        }
    }
}
