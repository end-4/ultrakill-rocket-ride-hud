using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RocketRideHUD {
    public class RocketRideWeaponController : MonoBehaviour {
        public static RocketRideWeaponController Instance { get; private set; }

        public Image icon;
        public TextMeshProUGUI text;

        private int rides;

        private void Start() {
            //Core.Logger.LogInfo("RocketRideWeaponController Start");
            if (Instance != null && Instance != this) return;
            Instance = this;

            TMP_FontAsset font = transform.parent.parent.parent.GetComponentInChildren<TextMeshProUGUI>().font;

            GameObject iconO = new GameObject();
            iconO.name = "RocketRidesIcon";
            iconO.layer = 5;
            iconO.transform.SetParent(transform);
            iconO.transform.localPosition = new Vector3(-85, 35, 0);
            iconO.transform.localRotation = new Quaternion();
            iconO.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            icon = iconO.AddComponent<Image>();
            icon.sprite = SpriteLoader.LoadSpriteFromFile(Path.Combine(Core.workingDir, "assets/rocket_ride_weapon_hud.png"));
            icon.color = ConfigManager.weaponRocketColor.value;

            GameObject textO = new GameObject();
            textO.name = "RocketRidesText";
            textO.layer = 5;
            textO.transform.SetParent(transform);
            textO.transform.localPosition = new Vector3(-65, 36, 0);
            textO.transform.localRotation = new Quaternion();
            textO.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            text = textO.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.alignment = TextAlignmentOptions.Center;
            text.text = "0";
            text.color = ConfigManager.weaponRocketColor.value;

            SetStuffActive(ConfigManager.weaponRocketShow.value);

            // subscribe to listener events
            NewMovementListener.OnRocketRideCountChanged += SetRides;
            //Core.Logger.LogInfo("RocketRideWeaponController subscribed to NewMovementListener events");
        }

        private void OnDestroy() {
            NewMovementListener.OnRocketRideCountChanged -= SetRides;
            //Core.Logger.LogInfo("RocketRideWeaponController OnDestroy");
        }

        public void SetRides(int ridesSoFar) {
            if (text == null) return;
            rides = Mathf.Max(0, Core.MaxRocketRides - ridesSoFar);
            if (rides < 0) rides = 0;
            text.text = rides.ToString();
            //Core.Logger.LogInfo($"RocketRideWeaponController SetRides: {rides}");
        }

        public void Increment() {
            SetRides(rides + 1);
        }

        public void ResetRides() {
            SetRides(0);
        }

        public void UpdateColor() {
            if (icon == null || text == null) return;

            Color c = ConfigManager.weaponRocketColor.value;
            icon.color = c;
            text.color = c;
        }

        public void SetStuffActive(bool active) {
            if (icon == null || text == null) return;

            icon.gameObject.SetActive(active);
            text.gameObject.SetActive(active);
        }
    }
}
