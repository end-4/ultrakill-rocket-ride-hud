using UnityEngine;
using UnityEngine.UI;

namespace WallJumpHUD
{
    public class RocketCrosshairController : MonoBehaviour
    {
        public static RocketCrosshairController Instance { get; private set; }

        private Image[] segments;
        private Sprite segmentSprite;
        private int segmentCount = 5;
        private float dashLength = 12f;
        private float gapLength = 2f;
        private float thickness = 4f;

        private void Awake()
        {
            if (Instance != null && Instance != this) return;
            Instance = this;
        }

        private void Start()
        {
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
            for (int i = 0; i < segmentCount; i++)
            {
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

            SetRocketIndicatorsRotation(ConfigManager.crosshairRocketAlignment.value);
            SetRocketIndicatorsColor();
            SetRocketIndicatorsActive(ConfigManager.crosshairRocketShow.value);

            RocketRideListener.OnRocketRideCountChanged += SetRocketIndicators;
        }

        private void OnDestroy()
        {
            RocketRideListener.OnRocketRideCountChanged -= SetRocketIndicators;
        }

        // show/hide indicators by count
        public void SetRocketIndicators(int count)
        {
            if (segments == null) return;
            int capped = Mathf.Clamp(count, 0, segments.Length);
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == null) continue;
                segments[i].gameObject.SetActive(i < capped && ConfigManager.crosshairRocketShow.value && ConfigManager.crosshairWallJumpShow.value);
            }
        }

        public void SetRocketIndicatorsActive(bool active)
        {
            if (segments == null) return;
            if (!active)
            {
                for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].gameObject.SetActive(false);
            }
            else
            {
                SetRocketIndicators(0);
            }
        }

        public void SetRocketIndicatorsColor()
        {
            if (segments == null) return;
            Color c = ConfigManager.crosshairRocketColor.value;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].color = c;
        }

        private float offset = ConfigManager.crosshairRocketOffset.value;

        public void SetRocketOffset(float value)
        {
            offset = value;
            // reapply current rotation/position using stored alignment
            SetRocketIndicatorsRotation(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsThickness(float t)
        {
            thickness = t;
            if (segments == null) return;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].rectTransform.sizeDelta = new Vector2(dashLength, thickness);
            // reposition to account for size change
            SetRocketIndicatorsRotation(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsDash(float d)
        {
            dashLength = d;
            if (segments == null) return;
            for (int i = 0; i < segments.Length; i++) if (segments[i] != null) segments[i].rectTransform.sizeDelta = new Vector2(dashLength, thickness);
            SetRocketIndicatorsRotation(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsGap(float g)
        {
            gapLength = g;
            SetRocketIndicatorsRotation(ConfigManager.crosshairRocketAlignment.value);
        }

        public void SetRocketIndicatorsRotation(RocketAlignment alignment)
        {
            if (segments == null) return;
            // position horizontally centered and offset depending on alignment
            // compensate for parent's lossy scale so positions/sizes behave in pixels
            float invScale = 1f / Mathf.Max(0.0001f, transform.lossyScale.x);

            // Work in local "pixel" units (compensated by invScale) but compute exact edges
            float dashLocal = dashLength * invScale;
            float gapLocal = gapLength * invScale;
            float thicknessLocalF = thickness * invScale;

            // total width of all segments + gaps
            float totalWidth = segments.Length * dashLocal + (segments.Length - 1) * gapLocal;
            float leftStart = -totalWidth / 2f; // left edge of the first segment
            float yOffset = 0f;
            switch (alignment)
            {
                case RocketAlignment.Top: yOffset = offset; break;
                case RocketAlignment.Bottom: yOffset = -offset; break;
                default: yOffset = offset; break;
            }
            for (int i = 0; i < segments.Length; i++)
            {
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

                rect.anchoredPosition = new Vector2(center, Mathf.Round(yOffset * invScale));
                // sizeDelta expects local units; round thickness also
                rect.sizeDelta = new Vector2(width, Mathf.Round(thicknessLocalF));
            }
        }
    }
}
