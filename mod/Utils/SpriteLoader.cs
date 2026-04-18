using System;
using System.IO;
using UnityEngine;

namespace RocketRideHUD {
    public static class SpriteLoader {
        /// <summary>
        /// Attempts to load an image file from disk and convert it to a Unity Sprite.
        /// Returns null on failure.
        /// </summary>
        /// <param name="path">Full path to the image file.</param>
        /// <param name="pixelsPerUnit">Pixels per unit to use when creating the Sprite. Default 100.</param>
        /// <param name="pivot">Sprite pivot, default center (0.5,0.5).</param>
        /// <param name="textureFormat">Texture format to create. Default ARGB32.</param>
        /// <param name="mipmap">Whether to create the texture with mipmaps. Default false.</param>
        public static Sprite LoadSpriteFromFile(string path, float pixelsPerUnit = 100f, Vector2? pivot = null, TextureFormat textureFormat = TextureFormat.ARGB32, bool mipmap = false) {
            try {
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return null;

                byte[] data = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2, textureFormat, mipmap);
                if (!tex.LoadImage(data)) return null;
                tex.Apply();

                Vector2 pivotValue = pivot ?? new Vector2(0.5f, 0.5f);
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivotValue, pixelsPerUnit);
            } catch (Exception) {
                return null;
            }
        }
    }
}
