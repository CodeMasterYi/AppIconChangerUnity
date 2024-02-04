using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AppIconChanger.Editor
{
    internal static class TextureAssetUtils
    {
        public static bool VerifyTexture(Texture2D texture, out List<string> errors)
        {
            return VerifyTexture(texture, 1024, true, out errors);
        }

        public static bool VerifyTexture(Texture2D texture, int minSize, out List<string> errors)
        {
            return VerifyTexture(texture, minSize, true, out errors);
        }

        public static bool VerifyTexture(Texture2D texture, int minSize, bool needsAlpha, out List<string> errors)
        {
            errors = new List<string>();

            if (texture == null)
            {
                errors.Add("Texture is invalid");
                return false;
            }

            var isSquare = texture.width == texture.height;
            var assetPath = AssetDatabase.GetAssetPath(texture);
            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            var hasAlpha = true;
            if (textureImporter == null)
            {
                errors.Add("No TextureImporter found for source texture");
            }
            else
            {
                hasAlpha = textureImporter.DoesSourceTextureHaveAlpha();
            }
            var isReadable = texture.isReadable;

            if (!isReadable)
            {
                errors.Add("Read/Write is not enabled in the texture importer");
            }

            var isLargeEnough = texture.width >= minSize && texture.height >= minSize;
            if (!isLargeEnough)
                errors.Add(string.Format("Texture must be at least {0}x{1} pixels (while it's {2}x{3})",
                    minSize,
                    minSize,
                    texture.width,
                    texture.height
                ));

            if (!isSquare)
                errors.Add(string.Format("Texture must have the same width and height (while it's {0}x{1})",
                    texture.width,
                    texture.height
                ));

            if (!hasAlpha && needsAlpha)
                errors.Add(string.Format("Texture must contain an alpha channel"));

            return isReadable && isSquare && isLargeEnough && (!needsAlpha || hasAlpha);
        }

        public static Texture2D ProcessTexture(Texture2D sourceTexture)
        {
            if (sourceTexture == null) return null;

            var assetPath = AssetDatabase.GetAssetPath(sourceTexture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null || !importer.isReadable) return null;

            Texture2D texture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, true);
            texture.SetPixels(sourceTexture.GetPixels());
            texture.Apply();
            return texture;
        }

        public static Texture2D ScaleTexture(Texture2D sourceTexture, int width, int height)
        {
            if (sourceTexture.width == width && sourceTexture.height == height)
                return sourceTexture;

            var result = new Texture2D(width, height, TextureFormat.ARGB32, false);

            var destPixels = new Color[width * height];
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    destPixels[y * width + x] = sourceTexture.GetPixelBilinear((float)x / (float)width, (float)y / (float)height);
                }
            }
            result.SetPixels(destPixels);
            result.Apply();

            return result;
        }
    }
}
