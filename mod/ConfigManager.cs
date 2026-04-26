﻿using PluginConfig.API;
using PluginConfig.API.Fields;
using PluginConfig.API.Decorators;
using System.IO;
using UnityEngine;
using TMPro;

namespace RocketRideHUD {
    public class ConfigManager {
        public static PluginConfigurator config = null;

        public static EnumField<WeaponHudAnchor> weaponWallJumpAlignment;
        public static ColorField weaponWallJumpColor;
        public static EnumField<CrosshairAlignment> crosshairWallJumpAlignment;
        public static ColorField crosshairWallJumpColor;

        public static EnumField<WeaponHudAnchor> weaponRocketAlignment;
        public static ColorField weaponRocketColor;
        public static ColorField crosshairRocketColor;
        public static ColorField crosshairRocketUsedColor;
        public static FloatField crosshairRocketUsedOpacity;
        public static BoolField crosshairRocketFuelShow;
        public static ColorField crosshairRocketFuelColor;
        public static FloatField crosshairRocketFuelOffset;
        public static FloatField crosshairRocketFuelWidth;
        public static FloatField crosshairRocketFuelOverstay;
        public static EnumField<RocketAlignment> crosshairRocketAlignment;
        public static FloatField crosshairRocketOffset;
        public static FloatField crosshairRocketThickness;
        public static FloatField crosshairRocketDash;
        public static FloatField crosshairRocketGap;
        public static EnumField<PitchShowCondition> crosshairRocketPitchVisibility;
        public static FloatField crosshairRocketPitchMin;
        public static FloatField crosshairRocketPitchMax;
        public static FloatField crosshairRocketPitchSensitivity;
        public static ColorField crosshairRocketPitchColor;
        public static FloatField crosshairRocketPitchWidth;
        public static FloatField crosshairRocketPitchThickness;

        private static int h1 = 24;
        private static int h2 = 16;
        private static int subtitle = 12;
        private static int h1Gap = 20;
        private static int h2Gap = 12;

        private static ConfigHeader addGap(ConfigPanel panel, int size = 15) {
            return new ConfigHeader(panel, "", size);
        }

        private static void addDisplayOptions() {
            addGap(config.rootPanel, h1Gap); // Hack to avoid overlap with PluginConfigurator profile bar
            new ConfigHeader(config.rootPanel, "-- DISPLAY --", h1);
            new ConfigHeader(config.rootPanel, "Weapon HUD indicators only available on standard style", subtitle);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Rocket rides", h2, TextAlignmentOptions.Left);
            weaponRocketAlignment = new EnumField<WeaponHudAnchor>(config.rootPanel, "Weapon HUD number", "weaponRocketAlignment", WeaponHudAnchor.ShowTopLeft);
            weaponRocketAlignment.postValueChangeEvent += (WeaponHudAnchor newValue) => {
                if (RocketCrosshairController.Instance == null) return;
                RocketRideWeaponController.Instance.UpdateAlignment(newValue);
                WallJumpWeaponController.Instance.UpdateAlignment(weaponWallJumpAlignment.value);
            };
            crosshairRocketAlignment = new EnumField<RocketAlignment>(config.rootPanel, "Crosshair indicator", "crosshairRocketAlignment", RocketAlignment.Bottom);
            crosshairRocketAlignment.postValueChangeEvent += (RocketAlignment e) => {
                if (RocketCrosshairController.Instance == null) return;
                RocketCrosshairController.Instance.UpdateRideIndicators(e);
                RocketCrosshairController.Instance.SetRocketIndicatorsActive(e != RocketAlignment.Hidden);

            };
            crosshairRocketFuelShow = new BoolField(config.rootPanel, "Show fuel bar", "crosshairRocketFuelShow", true);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Freezeframe rocket ride angle hint", h2, TextAlignmentOptions.Left);
            crosshairRocketPitchVisibility = new EnumField<PitchShowCondition>(config.rootPanel, "Show when", "rocketPitchVisibility", PitchShowCondition.HoldingFreezeframe);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Wall jumps", h2, TextAlignmentOptions.Left);
            weaponWallJumpAlignment = new EnumField<WeaponHudAnchor>(config.rootPanel, "Weapon HUD number", "weaponWallJumpAlignment", WeaponHudAnchor.Hidden);
            weaponWallJumpAlignment.postValueChangeEvent += (WeaponHudAnchor newValue) => {
                if (WallJumpWeaponController.Instance == null) return;
                WallJumpWeaponController.Instance.UpdateAlignment(newValue);
            };
            crosshairWallJumpAlignment = new EnumField<CrosshairAlignment>(config.rootPanel, "Crosshair indicator", "crosshairAlignment", CrosshairAlignment.Hidden);
            crosshairWallJumpAlignment.postValueChangeEvent += (CrosshairAlignment e) => {
                if (WallJumpCrosshairController.Instance != null) {
                    WallJumpCrosshairController.Instance.SetIconsRotation(e);
                    if (PowerUpMeter.Instance != null && PowerUpMeter.Instance.latestMaxJuice > 0) WallJumpCrosshairController.Instance.OnPowerUpStarted();
                    WallJumpCrosshairController.Instance.SetIconsActive(e != CrosshairAlignment.Hidden);
                }
            };
        }

        private static void addCustomizationOptions() {
            addGap(config.rootPanel, h1Gap);
            new ConfigHeader(config.rootPanel, "-- CUSTOMIZATION --");

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Rocket rides", h2, TextAlignmentOptions.Left);
            weaponRocketColor = new ColorField(config.rootPanel, "Weapon HUD indicator", "weaponRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            weaponRocketColor.postValueChangeEvent += (Color e) => {
                if (RocketRideWeaponController.Instance != null) RocketRideWeaponController.Instance.UpdateColor();
            };
            crosshairRocketColor = new ColorField(config.rootPanel, "Crosshair: rides", "crosshairRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            crosshairRocketColor.postValueChangeEvent += (Color e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsColor();
            };
            crosshairRocketUsedColor = new ColorField(config.rootPanel, "Used rides", "crosshairRocketUsedColor", new Color(1f, 1f, 1f));
            crosshairRocketUsedColor.postValueChangeEvent += (Color e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsColor();
            };
            crosshairRocketUsedOpacity = new FloatField(config.rootPanel, "Used rides opacity", "crosshairRocketUsedOpacity", 0.4f);
            crosshairRocketUsedOpacity.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsColor();
            };
            crosshairRocketThickness = new FloatField(config.rootPanel, "Line thickness", "crosshairRocketThickness", 4f);
            crosshairRocketThickness.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsThickness(e);
            };
            crosshairRocketOffset = new FloatField(config.rootPanel, "Crosshair: Rides offset", "crosshairRocketOffset", 40f);
            crosshairRocketOffset.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketOffset(e);
            };
            crosshairRocketDash = new FloatField(config.rootPanel, "Crosshair: Dash length", "crosshairRocketDash", 18f);
            crosshairRocketDash.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsDash(e);
            };
            crosshairRocketGap = new FloatField(config.rootPanel, "Crosshair: Gap length", "crosshairRocketGap", -3f);
            crosshairRocketGap.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsGap(e);
            };
            crosshairRocketFuelColor = new ColorField(config.rootPanel, "Fuel bar color", "crosshairRocketFuelColor", new Color(1f, 128f / 255f, 58f / 255f));
            crosshairRocketFuelOffset = new FloatField(config.rootPanel, "Fuel bar offset", "crosshairRocketFuelOffset", 48f);
            crosshairRocketFuelWidth = new FloatField(config.rootPanel, "Fuel bar width", "crosshairRocketFuelWidth", 108f);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Freezeframe rocket ride angle hint", h2, TextAlignmentOptions.Left);
            crosshairRocketPitchColor = new ColorField(config.rootPanel, "Color", "rocketPitchColor", new Color(64f / 255f, 232f / 255f, 1f));
            crosshairRocketPitchWidth = new FloatField(config.rootPanel, "Line length", "rocketPitchWidth", 150f);
            crosshairRocketPitchThickness = new FloatField(config.rootPanel, "Line thickness", "rocketPitchThickness", 2f);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Wall jumps", h2, TextAlignmentOptions.Left);
            weaponWallJumpColor = new ColorField(config.rootPanel, "Weapon HUD", "weaponColor", Color.white);
            weaponWallJumpColor.postValueChangeEvent += (Color e) => {
                if (WallJumpWeaponController.Instance != null) WallJumpWeaponController.Instance.UpdateColor();
            };
            crosshairWallJumpColor = new ColorField(config.rootPanel, "Crosshair indicator", "crosshairColor", Color.white);
            crosshairWallJumpColor.postValueChangeEvent += (Color e) => {
                if (WallJumpCrosshairController.Instance != null) WallJumpCrosshairController.Instance.UpdateColor();
            };
        }

        private static void addAdvancedOptions() {
            addGap(config.rootPanel, h1Gap);
            new ConfigHeader(config.rootPanel, "-- ADVANCED --");
            new ConfigHeader(config.rootPanel, "Values are approximations based on the mod author's personal testing. Real-world effectiveness can vary depending on timing/reaction. For example, to do a Freezeframe ride, you might need to unfreeze later if you aim lower", subtitle, TextAlignmentOptions.Left);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Rocket rides", h2, TextAlignmentOptions.Left);
            crosshairRocketFuelOverstay = new FloatField(config.rootPanel, "Empty fuel downpull threshold", "crosshairRocketFuelOverstay", 0.1f);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "// Freezeframe rocket ride angle hint", h2, TextAlignmentOptions.Left);
            crosshairRocketPitchMin = new FloatField(config.rootPanel, "Minimum pitch (degrees)", "rocketPitchMin", 7f);
            crosshairRocketPitchMax = new FloatField(config.rootPanel, "Maximum pitch (degrees)", "rocketPitchMax", 35f);
            crosshairRocketPitchSensitivity = new FloatField(config.rootPanel, "Visual sensitivity (px/degree)", "rocketPitchSensitivity", 6f);
        }

        public static void Init() {
            if (config != null) return;
            config = PluginConfigurator.Create("Rocket Ride HUD", Core.PluginGUID);

            string iconPath = Path.Combine(Core.workingDir, "icon.png");
            if (File.Exists(iconPath)) config.SetIconWithURL(iconPath);

            addDisplayOptions();
            addCustomizationOptions();
            addAdvancedOptions();
        }
    }
}
