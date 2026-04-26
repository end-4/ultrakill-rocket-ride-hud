﻿namespace RocketRideHUD {
    public enum Direction {
        Normal,
        Reverse
    }

    public enum CrosshairAlignment {
        Hidden,
        Top,
        Left,
        Right,
        Bottom
    }

    public enum RocketAlignment {
        Hidden,
        Top,
        Bottom
    }

    public enum PitchShowCondition {
        Never,
        Always,
        HoldingRocketLauncher,
        HoldingFreezeframe
    }

    public enum WeaponHudAnchor {
        Hidden,
        ShowInside,
        ShowTopLeft, ShowTopRight, ShowBottom, ShowLeft, ShowRight
    }
}
