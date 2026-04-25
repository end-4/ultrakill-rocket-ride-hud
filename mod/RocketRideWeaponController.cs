using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel.Design.Serialization;

namespace RocketRideHUD {
    public class RocketRideWeaponController : MonoBehaviour {
        public static RocketRideWeaponController Instance { get; private set; }

        public GameObject panel;
        private Image bgImage;
        public Image icon;
        public TextMeshProUGUI text;

        private int rides = Core.MaxRocketRides;

        private void Start() {
            //Core.Logger.LogInfo("RocketRideWeaponController Start");
            if (Instance != null && Instance != this) return;
            Instance = this;

            // Core.Logger.LogInfo("OBJ TRACE " + name + ", " + transform.parent.name + ", " + transform.parent.parent.name);
            // ^ Expected result: Image, Filler, GunPanel

            TMP_FontAsset font = transform.parent.parent.parent.GetComponentInChildren<TextMeshProUGUI>().font;

            // Create panel
            panel = new GameObject("RocketRidePanel");
            panel.layer = 5;
            RectTransform rect = panel.AddComponent<RectTransform>();
            panel.transform.SetParent(transform, false);
            UpdateAlignment(ConfigManager.weaponRocketAlignment.value);
            rect.sizeDelta = new Vector2(46, 25);
            bgImage = panel.AddComponent<Image>();
            bgImage.enabled = ConfigManager.weaponRocketAlignment.value != RocketWeaponHudShow.ShowInside;
            var sourceImage = transform.parent.parent.GetComponent<Image>();
            if (sourceImage != null) {
                bgImage.sprite = sourceImage.sprite;
                bgImage.type = sourceImage.type;
                bgImage.pixelsPerUnitMultiplier = sourceImage.pixelsPerUnitMultiplier;
                bgImage.color = sourceImage.color;
                bgImage.material = sourceImage.material;
            }

            // Add icon
            GameObject iconO = new GameObject {
                name = "RocketRidesIcon",
                layer = 5
            };
            iconO.transform.SetParent(panel.transform);
            iconO.transform.localPosition = new Vector3(-10, 0, 0);
            iconO.transform.localRotation = new Quaternion();
            iconO.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            icon = iconO.AddComponent<Image>();
            icon.sprite = SpriteLoader.LoadSpriteFromFile(Path.Combine(Core.workingDir, "assets/rocket_ride_weapon_hud.png"));
            icon.color = ConfigManager.weaponRocketColor.value;

            // Add text
            GameObject textO = new GameObject {
                name = "RocketRidesText",
                layer = 5
            };
            textO.transform.SetParent(panel.transform);
            textO.transform.localPosition = new Vector3(8, 0, 0);
            textO.transform.localRotation = new Quaternion();
            textO.transform.localScale = new Vector3(1f, 1f, 1f);
            text = textO.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = 18;
            text.alignment = TextAlignmentOptions.Center;
            text.text = rides.ToString();
            text.color = ConfigManager.weaponRocketColor.value;

            SetStuffActive(ConfigManager.weaponRocketAlignment.value != RocketWeaponHudShow.Hidden);

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

        public void UpdateColor() {
            if (icon == null || text == null) return;

            Color c = ConfigManager.weaponRocketColor.value;
            icon.color = c;
            text.color = c;
        }

        public void SetStuffActive(bool active) {
            if (panel == null) return;

            panel.SetActive(active);
        }

        public void UpdateAlignment(RocketWeaponHudShow newValue) {
            if (newValue == RocketWeaponHudShow.Hidden) {
                SetStuffActive(false);
            } else {
                // top 124, 64  left -124 37 right 171 37 bottom -77 -110
                // rect.anchoredPosition = new Vector2(124, 64);
                RectTransform rect = panel.GetComponent<RectTransform>();
                switch (newValue) {
                    case RocketWeaponHudShow.ShowTop:
                        rect.anchoredPosition = new Vector2(124, 64);
                        break;
                    case RocketWeaponHudShow.ShowLeft:
                        rect.anchoredPosition = new Vector2(-124, 37);
                        break;
                    case RocketWeaponHudShow.ShowRight:
                        rect.anchoredPosition = new Vector2(171, 37);
                        break;
                    case RocketWeaponHudShow.ShowBottom:
                        rect.anchoredPosition = new Vector2(-77, -110);
                        break;
                    case RocketWeaponHudShow.ShowInside:
                        rect.anchoredPosition = new Vector2(-77, 37);
                        break;
                }
                if (bgImage == null) return;
                bgImage.enabled = newValue != RocketWeaponHudShow.ShowInside;
                SetStuffActive(true);
            }
        }
    }
}
