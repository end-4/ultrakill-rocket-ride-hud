﻿using PluginConfig.API;
using PluginConfig.API.Fields;
using PluginConfig.API.Decorators;
using System.IO;
using UnityEngine;

namespace RocketRideHUD {
    public class ConfigManager {
        public static PluginConfigurator config = null;

        public static BoolField weaponWallJumpShow;
        public static ColorField weaponWallJumpColor;
        public static EnumField<CrosshairAlignment> crosshairWallJumpAlignment;
        public static ColorField crosshairWallJumpColor;

        public static BoolField weaponRocketShow;
        public static ColorField weaponRocketColor;
        public static ColorField crosshairRocketColor;
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

        public static void Init() {
            if (config != null) return;
            config = PluginConfigurator.Create("Rocket Ride HUD", Core.PluginGUID);

            string iconPath = Path.Combine(Core.workingDir, "icon.png");
            if (File.Exists(iconPath)) config.SetIconWithURL(iconPath);

            // Display ////////////////////////////////////////////////////////////////////////////////////////
            addGap(config.rootPanel, h1Gap); // Hack to avoid overlap with PluginConfigurator profile bar
            new ConfigHeader(config.rootPanel, "-- DISPLAY --", h1);
            new ConfigHeader(config.rootPanel, "Weapon HUD indicators only available on standard style", subtitle);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "ROCKET RIDES", h2);
            weaponRocketShow = new BoolField(config.rootPanel, "Weapon HUD number", "weaponRocketShow", true);
            weaponRocketShow.postValueChangeEvent += (bool e) => {
                if (RocketRideWeaponController.Instance != null) RocketRideWeaponController.Instance.SetStuffActive(e);
            };
            crosshairRocketAlignment = new EnumField<RocketAlignment>(config.rootPanel, "Crosshair indicator", "crosshairRocketAlignment", RocketAlignment.Bottom);
            crosshairRocketAlignment.postValueChangeEvent += (RocketAlignment e) => {
                if (RocketCrosshairController.Instance != null) {
                    RocketCrosshairController.Instance.SetRocketIndicatorsRotation(e);
                    RocketCrosshairController.Instance.SetRocketIndicatorsActive(e != RocketAlignment.Hidden);
                }
            };

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "FREEZEFRAME ANGLE HINT", h2);
            crosshairRocketPitchVisibility = new EnumField<PitchShowCondition>(config.rootPanel, "Show when", "rocketPitchVisibility", PitchShowCondition.HoldingFreezeframe);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "WALL JUMPS", h2);
            weaponWallJumpShow = new BoolField(config.rootPanel, "Weapon HUD number", "weaponShow", true);
            weaponWallJumpShow.postValueChangeEvent += (bool e) => {
                if (WallJumpWeaponController.Instance != null) WallJumpWeaponController.Instance.SetStuffActive(e);
            };
            crosshairWallJumpAlignment = new EnumField<CrosshairAlignment>(config.rootPanel, "Crosshair indicator", "crosshairAlignment", CrosshairAlignment.Right);
            crosshairWallJumpAlignment.postValueChangeEvent += (CrosshairAlignment e) => {
                if (WallJumpCrosshairController.Instance != null) {
                    WallJumpCrosshairController.Instance.SetIconsRotation(e);
                    if (PowerUpMeter.Instance != null && PowerUpMeter.Instance.latestMaxJuice > 0) WallJumpCrosshairController.Instance.OnPowerUpStarted();
                    WallJumpCrosshairController.Instance.SetIconsActive(e != CrosshairAlignment.Hidden);
                }
            };

            // Customization //////////////////////////////////////////////////////////////////////////////////
            addGap(config.rootPanel, h1Gap);
            new ConfigHeader(config.rootPanel, "-- CUSTOMIZATION --");

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "ROCKET RIDES", h2);
            weaponRocketColor = new ColorField(config.rootPanel, "Weapon HUD indicator", "weaponRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            weaponRocketColor.postValueChangeEvent += (Color e) => {
                if (RocketRideWeaponController.Instance != null) RocketRideWeaponController.Instance.UpdateColor();
            };
            crosshairRocketColor = new ColorField(config.rootPanel, "Crosshair indicator", "crosshairRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            crosshairRocketColor.postValueChangeEvent += (Color e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsColor();
            };
            crosshairRocketOffset = new FloatField(config.rootPanel, "Crosshair: Offset", "crosshairRocketOffset", 50f);
            crosshairRocketOffset.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketOffset(e);
            };
            crosshairRocketThickness = new FloatField(config.rootPanel, "Crosshair: Thickness", "crosshairRocketThickness", 6f);
            crosshairRocketThickness.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsThickness(e);
            };
            crosshairRocketDash = new FloatField(config.rootPanel, "Crosshair: Dash length", "crosshairRocketDash", 20f);
            crosshairRocketDash.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsDash(e);
            };
            crosshairRocketGap = new FloatField(config.rootPanel, "Crosshair: Gap length", "crosshairRocketGap", 1f);
            crosshairRocketGap.postValueChangeEvent += (float e) => {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsGap(e);
            };

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "FREEZEFRAME ANGLE HINT", h2);
            crosshairRocketPitchMin = new FloatField(config.rootPanel, "Minimum pitch (degrees)", "rocketPitchMin", 3.8f);
            crosshairRocketPitchMax = new FloatField(config.rootPanel, "Maximum pitch (degrees)", "rocketPitchMax", 33.8f);
            crosshairRocketPitchSensitivity = new FloatField(config.rootPanel, "Visual sensitivity (px/degree)", "rocketPitchSensitivity", 7f);
            crosshairRocketPitchColor = new ColorField(config.rootPanel, "Color", "rocketPitchColor", new Color(64f / 255f, 232f / 255f, 1f));
            crosshairRocketPitchWidth = new FloatField(config.rootPanel, "Line length", "rocketPitchWidth", 200f);
            crosshairRocketPitchThickness = new FloatField(config.rootPanel, "Line thickness", "rocketPitchThickness", 3f);

            addGap(config.rootPanel, h2Gap);
            new ConfigHeader(config.rootPanel, "WALL JUMPS", h2);
            weaponWallJumpColor = new ColorField(config.rootPanel, "Weapon HUD", "weaponColor", Color.white);
            weaponWallJumpColor.postValueChangeEvent += (Color e) => {
                if (WallJumpWeaponController.Instance != null) WallJumpWeaponController.Instance.UpdateColor();
            };
            crosshairWallJumpColor = new ColorField(config.rootPanel, "Crosshair indicator", "crosshairColor", Color.white);
            crosshairWallJumpColor.postValueChangeEvent += (Color e) => {
                if (WallJumpCrosshairController.Instance != null) WallJumpCrosshairController.Instance.UpdateColor();
            };

        }
    }
}
