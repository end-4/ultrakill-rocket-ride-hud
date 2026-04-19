using UnityEngine;
using UnityEngine.UI;

namespace RocketRideHUD {
    public class RocketCrosshairController : MonoBehaviour {
        public static RocketCrosshairController Instance { get; private set; }

        private Image[] segments;
        private Sprite segmentSprite;
        private int segmentCount = Core.MaxRocketRides;
        private float dashLength;
        private float gapLength;
        private float thickness;

        private int lastRidesSoFar = 0;
        private bool lastRiding = false;
        private float lastFuel = 1;
        private Image upperPitchLine;
        private Image lowerPitchLine;
        private Image fuelBar;
        private Image fuelBarBackground;

        private void Awake() {
            if (Instance != null && Instance != this) return;
            Instance = this;
        }

        private void Start() {
            // create container (use RectTransform for predictable UI positioning)
            GameObject container = new GameObject("RocketSegments");
            container.layer = 5;
            container.transform.SetParent(transform);
            var containerRect = container.AddComponent<RectTransform>();
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchorMin = containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            container.transform.localScale = Vector3.one;

            // read initial configuration so values are applied on startup
            if (ConfigManager.crosshairRocketDash != null) dashLength = ConfigManager.crosshairRocketDash.value;
            if (ConfigManager.crosshairRocketGap != null) gapLength = ConfigManager.crosshairRocketGap.value;
            if (ConfigManager.crosshairRocketThickness != null) thickness = ConfigManager.crosshairRocketThickness.value;
            if (ConfigManager.crosshairRocketOffset != null) offset = ConfigManager.crosshairRocketOffset.value;

            // prepare a simple 1x1 white sprite to draw segments (we draw programmatically)
            Texture2D pixel = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            pixel.filterMode = FilterMode.Point;
            pixel.wrapMode = TextureWrapMode.Clamp;
            pixel.SetPixel(0, 0, Color.white);
            pixel.Apply(false, false);
            segmentSprite = Sprite.Create(pixel, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
            // ensure texture uses point filtering to avoid 1px blending gaps when stretched
            segmentSprite.texture.filterMode = FilterMode.Point;

            segments = new Image[segmentCount];
            for (int i = 0; i < segmentCount; i++) {
                GameObject go = new GameObject($"Seg{i + 1}");
                go.layer = 5;
                go.transform.SetParent(container.transform);
                var img = go.AddComponent<Image>();
                img.sprite = segmentSprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = false;
                img.color = ConfigManager.crosshairRocketColor.value;
                var rect = img.rectTransform;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(dashLength, thickness);
                rect.anchoredPosition = Vector2.zero;
                segments[i] = img;
            }

            // Initialize Pitch Lines
            upperPitchLine = CreatePitchLine(container.transform, "UpperPitch");
            lowerPitchLine = CreatePitchLine(container.transform, "LowerPitch");

            // Initialize Fuel Bar
            fuelBarBackground = CreatePitchLine(container.transform, "FuelBarBackground");
            fuelBar = CreatePitchLine(container.transform, "FuelBar");

            UpdateRideIndicators(ConfigManager.crosshairRocketAlignment.value);
            SetRocketIndicatorsColor();
            SetRocketIndicatorsActive(ConfigManager.crosshairRocketAlignment.value != RocketAlignment.Hidden);

            nm = NewMovement.Instance;
            NewMovementListener.OnRocketRideCountChanged += SetRocketIndicators;
            NewMovementListener.OnRocketFuelChanged += UpdateFuelBar;
            NewMovementListener.OnRidingRocketChanged += UpdateFuelBar;
        }

        private NewMovement nm;

        private Image CreatePitchLine(Transform parent, string name) {
            GameObject go = new GameObject(name);
            go.layer = 5;
            go.transform.SetParent(parent);
            var img = go.AddComponent<Image>();
            img.sprite = segmentSprite;
            var rect = img.rectTransform;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero; // Size is managed in Update()
            return img;
        }

        private void OnDestroy() {
            NewMovementListener.OnRocketRideCountChanged -= SetRocketIndicators;
            NewMovementListener.OnRocketFuelChanged -= UpdateFuelBar;
        }

        private void Update() {
            bool visibilityCheck = false;
            switch (ConfigManager.crosshairRocketPitchVisibility.value) {
                case PitchShowCondition.Never:
                    visibilityCheck = false;
                    break;
                case PitchShowCondition.Always:
                    visibilityCheck = true;
                    break;
                case PitchShowCondition.HoldingRocketLauncher:
                    if (GunControl.Instance != null && GunControl.Instance.currentWeapon != null)
                        visibilityCheck = GunControl.Instance.currentWeapon.GetComponent<RocketLauncher>() != null;
                    break;
                case PitchShowCondition.HoldingFreezeframe:
                    if (GunControl.Instance != null && GunControl.Instance.currentWeapon != null) {
                        RocketLauncher rl = GunControl.Instance.currentWeapon.GetComponent<RocketLauncher>();
                        visibilityCheck = rl != null && rl.variation == 0;
                    }
                    break;
            }

            if (nm == null || upperPitchLine == null || !visibilityCheck) {
                if (upperPitchLine != null) upperPitchLine.gameObject.SetActive(false);
                if (lowerPitchLine != null) lowerPitchLine.gameObject.SetActive(false);
                return;
            }

            float currentPitch = nm.cc.cam.transform.localEulerAngles.x;
            if (currentPitch > 180) currentPitch -= 360; // Normalize to -180 to 180
            // Core.Logger.LogInfo("Current pitch: "+currentPitch);

            float minPitch = ConfigManager.crosshairRocketPitchMin.value;
            float maxPitch = ConfigManager.crosshairRocketPitchMax.value;
            float sens = ConfigManager.crosshairRocketPitchSensitivity.value;

            // Calculate position relative to crosshair using perspective projection (tangent)
            // focalLength is derived so that for small angles, 1 degree = sens pixels.
            float focalLength = sens * (180f / Mathf.PI);
            float diffMin = (currentPitch - minPitch) * Mathf.Deg2Rad;
            float diffMax = (currentPitch - maxPitch) * Mathf.Deg2Rad;

            // Hide lines if they are too far from center (perspective limit)
            upperPitchLine.gameObject.SetActive(Mathf.Abs(diffMin) < 1.5f); // ~86 degrees
            lowerPitchLine.gameObject.SetActive(Mathf.Abs(diffMax) < 1.5f);

            upperPitchLine.rectTransform.anchoredPosition = new Vector2(0, Mathf.Tan(diffMin) * focalLength);
            lowerPitchLine.rectTransform.anchoredPosition = new Vector2(0, Mathf.Tan(diffMax) * focalLength);

            upperPitchLine.color = lowerPitchLine.color = ConfigManager.crosshairRocketPitchColor.value;
            Vector2 size = new Vector2(ConfigManager.crosshairRocketPitchWidth.value, ConfigManager.crosshairRocketPitchThickness.value);
            upperPitchLine.rectTransform.sizeDelta = lowerPitchLine.rectTransform.sizeDelta = size;
        }

        public void UpdateFuelBar(float fuel) {
            lastFuel = fuel;
            UpdateFuelBar(fuel, lastRiding);
        }

        public void UpdateFuelBar(bool riding) {
            lastRiding = riding;
            UpdateFuelBar(lastFuel, riding);
        }

        public void UpdateFuelBar(float fuel, bool riding) {
            if (fuelBar == null) return;

            bool shouldShow = riding && ConfigManager.crosshairRocketFuelShow.value && ConfigManager.crosshairRocketAlignment.value != RocketAlignment.Hidden;
            fuelBar.gameObject.SetActive(shouldShow);
            if (fuelBarBackground != null) fuelBarBackground.gameObject.SetActive(shouldShow);

            if (!shouldShow) return;

            float fuelOffset = ConfigManager.crosshairRocketFuelOffset.value;
            float yOffset = ConfigManager.crosshairRocketAlignment.value == RocketAlignment.Bottom ? -fuelOffset : fuelOffset;
            float h = Mathf.Round(thickness);
            float y = Mathf.Round(yOffset);
            float barWidth = ConfigManager.crosshairRocketFuelWidth.value;

            if (fuelBarBackground != null) {
                Color bgColor = ConfigManager.crosshairRocketUsedColor.value;
                bgColor.a = ConfigManager.crosshairRocketUsedOpacity.value;
                fuelBarBackground.color = bgColor;
                fuelBarBackground.rectTransform.sizeDelta = new Vector2(barWidth, h);
                fuelBarBackground.rectTransform.anchoredPosition = new Vector2(0, y);
            }

            fuelBar.color = ConfigManager.crosshairRocketFuelColor.value;
            fuelBar.rectTransform.sizeDelta = new Vector2(barWidth * fuel, h);
            fuelBar.rectTransform.anchoredPosition = new Vector2(0, y);
        }

        // show/hide indicators by count
        public void SetRocketIndicators(int ridesSoFar) {
            if (segments == null) return;
            lastRidesSoFar = ridesSoFar;

            int availableCount = Mathf.Max(0, Core.MaxRocketRides - ridesSoFar);
            Color activeColor = ConfigManager.crosshairRocketColor.value;
            Color usedColor = ConfigManager.crosshairRocketUsedColor.value;
            usedColor.a = ConfigManager.crosshairRocketUsedOpacity.value;

            for (int i = 0; i < segments.Length; i++) {
                if (segments[i] == null) continue;
                bool alignmentActive = ConfigManager.crosshairRocketAlignment.value != RocketAlignment.Hidden;
                segments[i].gameObject.SetActive(alignmentActive);
                if (alignmentActive) segments[i].color = (i < availableCount) ? activeColor : usedColor;
            }
        }

        public void SetRocketIndicatorsActive(bool active) {
            if (segments == null) return;
            if (!active) {
                for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].gameObject.SetActive(false);
            } else {
                SetRocketIndicators(lastRidesSoFar);
            }
        }

        public void SetRocketIndicatorsColor() {
            if (segments == null) return;
            Color c = ConfigManager.crosshairRocketColor.value;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].color = c;
            SetRocketIndicators(lastRidesSoFar);
        }

        private float offset = ConfigManager.crosshairRocketOffset.value;

        public void SetRocketOffset(float value) {
            offset = value;
            // reapply current rotation/position using stored alignment
            UpdateRideIndicators(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsThickness(float t) {
            thickness = t;
            if (segments == null) return;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].rectTransform.sizeDelta = new Vector2(dashLength, thickness);
            // reposition to account for size change
            UpdateRideIndicators(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsDash(float d) {
            dashLength = d;
            if (segments == null) return;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].rectTransform.sizeDelta = new Vector2(dashLength, thickness);
            UpdateRideIndicators(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsGap(float g) {
            gapLength = g;
            UpdateRideIndicators(ConfigManager.crosshairRocketAlignment.value);
        }

        public void UpdateRideIndicators(RocketAlignment alignment) {
            if (segments == null) return;

            float dashLocal = dashLength;
            float gapLocal = gapLength;
            float thicknessLocalF = thickness;

            // total width of all segments + gaps
            float totalWidth = segments.Length * dashLocal + (segments.Length - 1) * gapLocal;
            float leftStart = -totalWidth / 2f; // left edge of the first segment
            float yOffset = 0f;
            switch (alignment) {
                case RocketAlignment.Top: yOffset = offset; break;
                case RocketAlignment.Bottom: yOffset = -offset; break;
                default: yOffset = offset; break;
            }
            for (int i = 0; i < segments.Length; i++) {
                if (segments[i] == null) continue;
                var rect = segments[i].rectTransform;
                rect.localRotation = Quaternion.identity;
                // compute exact left/right edges for this segment in local units
                float left = leftStart + i * (dashLocal + gapLocal);
                float right = left + dashLocal;

                // round edges to integer pixel boundaries to avoid sampling gaps
                float leftR = Mathf.Round(left);
                float rightR = Mathf.Round(right);

                // ensure at least 1 pixel wide
                float width = Mathf.Max(1f, rightR - leftR);
                float center = (leftR + rightR) / 2f;

                rect.anchoredPosition = new Vector2(center, Mathf.Round(yOffset));
                // sizeDelta expects local units; round thickness also
                rect.sizeDelta = new Vector2(width, Mathf.Round(thicknessLocalF));
            }
        }
    }
}
