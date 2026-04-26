using HarmonyLib;
using System.ComponentModel;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RocketRideHUD {
    public class WallJumpWeaponController : MonoBehaviour {
        public static WallJumpWeaponController Instance { get; private set; }

        public GameObject panel;
        private Image bgImage;
        public Image icon;
        public TextMeshProUGUI text;

        private void Start() {
            if (Instance != null && Instance != this) return;
            Instance = this;

            TMP_FontAsset font = transform.parent.parent.parent.GetComponentInChildren<TextMeshProUGUI>().font;

            // Create panel
            panel = new GameObject("WallJumpPanel");
            panel.layer = 5;
            RectTransform rect = panel.AddComponent<RectTransform>();
            panel.transform.SetParent(transform, false);
            UpdateAlignment(ConfigManager.weaponWallJumpAlignment.value);
            rect.sizeDelta = new Vector2(46, 25);
            bgImage = panel.AddComponent<Image>();
            bgImage.enabled = ConfigManager.weaponWallJumpAlignment.value != WeaponHudAnchor.ShowInside;
            var sourceImage = transform.parent.parent.GetComponent<Image>();
            if (sourceImage != null) {
                bgImage.sprite = sourceImage.sprite;
                bgImage.type = sourceImage.type;
                bgImage.pixelsPerUnitMultiplier = sourceImage.pixelsPerUnitMultiplier;
                bgImage.color = sourceImage.color;
                bgImage.material = sourceImage.material;
            }
            HudOpenEffect hudOpenEffect = panel.AddComponent<HudOpenEffect>();

            // Add icon
            GameObject iconO = new GameObject {
                name = "WallJumpsIcon",
                layer = 5
            };
            iconO.transform.SetParent(panel.transform);
            iconO.transform.localPosition = new Vector3(-9, -0.5f, 0);
            iconO.transform.localRotation = new Quaternion();
            iconO.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            icon = iconO.AddComponent<Image>();
            icon.sprite = SpriteLoader.LoadSpriteFromFile(Path.Combine(Core.workingDir, "assets/wall_jump_weapon_hud.png"));
            icon.color = Core.WeaponColor;

            // Add text
            GameObject textO = new GameObject {
                name = "WallJumpsText",
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
            text.text = Core.MaxWalljumps.ToString();
            text.color = Core.WeaponColor;

            SetStuffActive(ConfigManager.weaponWallJumpAlignment.value != WeaponHudAnchor.Hidden);

            // Subscribe to events
            NewMovementListener.OnWallJumpsChanged += SetWallJumps;
            PowerUpMeterListener.OnPowerUpStarted += OnPowerUpChange;
            PowerUpMeterListener.OnPowerUpEnded += OnPowerUpChange;
            SpeedometerListener.OnSpeedometerEnabled += OnSpeedometerEnabled;
            SpeedometerListener.OnSpeedometerDisabled += OnSpeedometerDisabled;
        }

        private void OnDestroy() {
            NewMovementListener.OnWallJumpsChanged -= SetWallJumps;
            PowerUpMeterListener.OnPowerUpStarted -= OnPowerUpChange;
            PowerUpMeterListener.OnPowerUpEnded -= OnPowerUpChange;
            SpeedometerListener.OnSpeedometerEnabled -= OnSpeedometerEnabled;
            SpeedometerListener.OnSpeedometerDisabled -= OnSpeedometerDisabled;
        }

        public void SetWallJumps(int number) {
            if (text == null) return;
            if (number > Core.MaxWalljumps) number = Core.MaxWalljumps;
            text.text = number.ToString();
        }

        public void OnPowerUpChange() {
            SetWallJumps(NewMovement.Instance.gc.onGround ? Core.MaxWalljumps : NewMovement.Instance.currentWallJumps);
        }

        private void OnSpeedometerEnabled() => UpdateSpeedometerAdjustment(true);
        private void OnSpeedometerDisabled() => UpdateSpeedometerAdjustment(false);

        public void UpdateColor() {
            if (icon == null || text == null) return;

            icon.color = Core.WeaponColor;
            text.color = Core.WeaponColor;
        }

        public void SetStuffActive(bool active) {
            if (panel == null) return;

            panel.SetActive(active);
        }

        private bool speedometerShown;
        public void UpdateSpeedometerAdjustment(bool speedometerShown) {
            this.speedometerShown = speedometerShown;
            UpdateAlignment();
        }

        public void UpdateAlignment() {
            UpdateAlignment(ConfigManager.weaponWallJumpAlignment.value);
        }

        public void UpdateAlignment(WeaponHudAnchor newValue) {
            if (newValue == WeaponHudAnchor.Hidden) {
                SetStuffActive(false);
            } else {
                bool rocketRideSameAnchor = newValue == ConfigManager.weaponRocketAlignment.value;
                RectTransform rect = panel.GetComponent<RectTransform>();
                switch (newValue) {
                    case WeaponHudAnchor.ShowTopLeft:
                        rect.anchoredPosition = new Vector2(rocketRideSameAnchor ? -30 : -77, speedometerShown ? 89 : 63);
                        break;
                    case WeaponHudAnchor.ShowTopRight:
                        rect.anchoredPosition = new Vector2(124, rocketRideSameAnchor ? 89 : 63);
                        break;
                    case WeaponHudAnchor.ShowLeft:
                        rect.anchoredPosition = new Vector2(-124, rocketRideSameAnchor ? 11 : 37);
                        break;
                    case WeaponHudAnchor.ShowRight:
                        rect.anchoredPosition = new Vector2(171, rocketRideSameAnchor ? 11 : 38);
                        break;
                    case WeaponHudAnchor.ShowBottom:
                        rect.anchoredPosition = new Vector2(rocketRideSameAnchor ? -30 : -77, -110);
                        break;
                    case WeaponHudAnchor.ShowInside:
                        rect.anchoredPosition = new Vector2(77, -38);
                        break;
                }
                if (bgImage == null) return;
                bgImage.enabled = newValue != WeaponHudAnchor.ShowInside;
                SetStuffActive(true);
            }
        }
    }
}
