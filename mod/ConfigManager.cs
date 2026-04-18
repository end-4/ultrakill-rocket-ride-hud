using PluginConfig.API;
using PluginConfig.API.Fields;
using PluginConfig.API.Decorators;
using System.IO;
using UnityEngine;

namespace RocketRideHUD
{
    public class ConfigManager
    {
        public static PluginConfigurator config = null;

        public static BoolField weaponWallJumpShow;
        public static ColorField weaponWallJumpColor;
        public static BoolField crosshairWallJumpShow;
        public static EnumField<CrosshairAlignment> crosshairWallJumpAlignment;
        public static ColorField crosshairWallJumpColor;

        public static BoolField weaponRocketShow;
        public static ColorField weaponRocketColor;
        public static BoolField crosshairRocketShow;
        public static ColorField crosshairRocketColor;
        public static EnumField<RocketAlignment> crosshairRocketAlignment;
        public static FloatField crosshairRocketOffset;
        public static FloatField crosshairRocketThickness;
        public static FloatField crosshairRocketDash;
        public static FloatField crosshairRocketGap;

        public static void Init()
        {
            if (config != null) return;
            config = PluginConfigurator.Create("Rocket Ride HUD", Core.PluginGUID);

            string iconPath = Path.Combine(Core.workingDir, "icon.png");
            if (File.Exists(iconPath)) config.SetIconWithURL(iconPath);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            new ConfigHeader(config.rootPanel, "-- DISPLAY --");
            new ConfigHeader(config.rootPanel, "Weapon indicators only visible on Standard HUD", 12);
            weaponWallJumpShow = new BoolField(config.rootPanel, "Wall jumps on Weapon HUD", "weaponShow", true);
            weaponWallJumpShow.postValueChangeEvent += (bool e) =>
            {
                if (WallJumpWeaponController.Instance != null) WallJumpWeaponController.Instance.SetStuffActive(e);
            };

            crosshairWallJumpShow = new BoolField(config.rootPanel, "Wall jumps on Crosshair", "crosshairShow", true);
            crosshairWallJumpShow.postValueChangeEvent += (bool e) =>
            {
                if (WallJumpCrosshairController.Instance != null) WallJumpCrosshairController.Instance.SetIconsActive(e);
            };

            weaponRocketShow = new BoolField(config.rootPanel, "Rocket rides on Weapon HUD", "weaponRocketShow", true);
            weaponRocketShow.postValueChangeEvent += (bool e) =>
            {
                if (RocketRideWeaponController.Instance != null) RocketRideWeaponController.Instance.SetStuffActive(e);
            };

            crosshairRocketShow = new BoolField(config.rootPanel, "Rocket rides on Crosshair", "crosshairRocketShow", true);
            crosshairRocketShow.postValueChangeEvent += (bool e) =>
            {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsActive(e);
            };

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            new ConfigHeader(config.rootPanel, "-- CUSTOMIZATION --");
            weaponWallJumpColor = new ColorField(config.rootPanel, "Wall jump: Weapon HUD", "weaponColor", Color.white);
            weaponWallJumpColor.postValueChangeEvent += (Color e) =>
            {
                if (WallJumpWeaponController.Instance != null) WallJumpWeaponController.Instance.UpdateColor();
            };

            crosshairWallJumpColor = new ColorField(config.rootPanel, "Wall jump: Crosshair", "crosshairColor", Color.white);
            crosshairWallJumpColor.postValueChangeEvent += (Color e) =>
            {
                if (WallJumpCrosshairController.Instance != null) WallJumpCrosshairController.Instance.UpdateColor();
            };

            crosshairWallJumpAlignment = new EnumField<CrosshairAlignment>(config.rootPanel, "Wall jump: Crosshair: Alignment", "crosshairAlignment", CrosshairAlignment.Right);
            crosshairWallJumpAlignment.postValueChangeEvent += (CrosshairAlignment e) =>
            {
                if (WallJumpCrosshairController.Instance != null)
                {
                    WallJumpCrosshairController.Instance.SetIconsRotation(e);
                    if (PowerUpMeter.Instance != null && PowerUpMeter.Instance.latestMaxJuice > 0) WallJumpCrosshairController.Instance.OnPowerUpStarted();
                }
            };

            weaponRocketColor = new ColorField(config.rootPanel, "Rocket rides: Weapon HUD", "weaponRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            weaponRocketColor.postValueChangeEvent += (Color e) =>
            {
                if (RocketRideWeaponController.Instance != null) RocketRideWeaponController.Instance.UpdateColor();
            };

            crosshairRocketColor = new ColorField(config.rootPanel, "Rocket rides: Crosshair", "crosshairRocketColor", new Color(1f, 128f / 255f, 58f / 255f));
            crosshairRocketColor.postValueChangeEvent += (Color e) =>
            {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsColor();
            };

            crosshairRocketAlignment = new EnumField<RocketAlignment>(config.rootPanel, "Rocket rides: Crosshair: Alignment", "crosshairRocketAlignment", RocketAlignment.Bottom);
            crosshairRocketAlignment.postValueChangeEvent += (RocketAlignment e) =>
            {
                if (RocketCrosshairController.Instance != null)
                {
                    RocketCrosshairController.Instance.SetRocketIndicatorsRotation(e);
                    if (PowerUpMeter.Instance != null && PowerUpMeter.Instance.latestMaxJuice > 0) RocketCrosshairController.Instance.SetRocketIndicatorsActive(ConfigManager.crosshairRocketShow.value);
                }
            };
            crosshairRocketOffset = new FloatField(config.rootPanel, "Rocket rides: Crosshair: Offset", "crosshairRocketOffset", 50f);
            crosshairRocketOffset.postValueChangeEvent += (float e) => 
            { 
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketOffset(e); 
            };

            crosshairRocketThickness = new FloatField(config.rootPanel, "Rocket rides: Crosshair: Thickness", "crosshairRocketThickness", 6f);
            crosshairRocketThickness.postValueChangeEvent += (float e) =>
            {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsThickness(e);
            };

            crosshairRocketDash = new FloatField(config.rootPanel, "Rocket rides: Crosshair: Dash length", "crosshairRocketDash", 20f);
            crosshairRocketDash.postValueChangeEvent += (float e) =>
            {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsDash(e);
            };

            crosshairRocketGap = new FloatField(config.rootPanel, "Rocket rides: Crosshair: Gap length", "crosshairRocketGap", 1f);
            crosshairRocketGap.postValueChangeEvent += (float e) =>
            {
                if (RocketCrosshairController.Instance != null) RocketCrosshairController.Instance.SetRocketIndicatorsGap(e);
            };
        }
    }
}
