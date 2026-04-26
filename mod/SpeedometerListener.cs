using UnityEngine;

namespace RocketRideHUD {
    public class SpeedometerListener : MonoBehaviour {
        public static SpeedometerListener Instance { get; private set; }

        public static event OnSpeedometerEnabledDelegate OnSpeedometerEnabled;
        public static event OnSpeedometerDisabledDelegate OnSpeedometerDisabled;

        public delegate void OnSpeedometerEnabledDelegate();
        public delegate void OnSpeedometerDisabledDelegate();

        private bool? previous = null;

        private void Awake() {
            if (Instance != null && Instance != this) return;
            Instance = this;
        }

        public void UpdateState(bool show) {
            if (previous != show) {
                previous = show;
                if (show) OnSpeedometerEnabled();
                else OnSpeedometerDisabled();
            }
        }

    }
}
