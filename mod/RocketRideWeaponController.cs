using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WallJumpHUD
{
    public class RocketRideWeaponController : MonoBehaviour
    {
        public static RocketRideWeaponController Instance { get; private set; }

        public Image icon;
        public TextMeshProUGUI text;

        private int rides;

        private void Start()
        {
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
            // try to load an external icon file next to the plugin (full path: <workingDir>/rocket_icon.png)
            Sprite diskSprite = TryLoadSpriteFromFile(Path.Combine(Core.workingDir, "assets/rocket_ride_weapon_hud.png"));
            if (diskSprite != null) icon.sprite = diskSprite;
            else
            {
                // fallback to bundle asset if disk file not present
                try { icon.sprite = Core.bundle.LoadAsset<Sprite>("assets/icon.png"); } catch { icon.sprite = null; }
            }
            // initial color use rocket-specific config
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
            RocketRideListener.OnRocketRideStarted += Increment;
            RocketRideListener.OnRocketRideEnded += OnRideEnded;
            RocketRideListener.OnRocketRideCountChanged += SetRides;
            //Core.Logger.LogInfo("RocketRideWeaponController subscribed to RocketRideListener events");
        }

        private void OnDestroy()
        {
            RocketRideListener.OnRocketRideStarted -= Increment;
            RocketRideListener.OnRocketRideEnded -= OnRideEnded;
            RocketRideListener.OnRocketRideCountChanged -= SetRides;
            //Core.Logger.LogInfo("RocketRideWeaponController OnDestroy");
        }

        private Sprite TryLoadSpriteFromFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                byte[] data = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!tex.LoadImage(data)) return null;
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            }
            catch
            {
                return null;
            }
        }

        public void SetRides(int number)
        {
            if (text == null) return;
            rides = number;
            if (rides < 0) rides = 0;
            text.text = rides.ToString();
            //Core.Logger.LogInfo($"RocketRideWeaponController SetRides: {rides}");
        }

        public void Increment()
        {
            SetRides(rides + 1);
        }

        public void ResetRides()
        {
            SetRides(0);
        }

        private void OnRideEnded()
        {
            // leave as-is; change to ResetRides() here if you want per-run behavior
            //Core.Logger.LogInfo("RocketRideListener reported ride ended");
        }

        public void UpdateColor()
        {
            if (icon == null || text == null) return;

            Color c = ConfigManager.weaponRocketColor.value;
            icon.color = c;
            text.color = c;
        }

        public void SetStuffActive(bool active)
        {
            if (icon == null || text == null) return;

            icon.gameObject.SetActive(active);
            text.gameObject.SetActive(active);
        }
    }
}
