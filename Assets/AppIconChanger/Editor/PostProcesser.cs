﻿#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AppIconChanger.Editor
{
    public static class PostProcessor
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS) return;

            var alternateIcons = AssetDatabase.FindAssets($"t:{nameof(AlternateIcon)}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<AlternateIcon>(x))
                .ToArray();
            if (alternateIcons.Length == 0) return;

            var imagesXcassetsDirectoryPath = Path.Combine(pathToBuiltProject, "Unity-iPhone", "Images.xcassets");

            var iconNames = new List<string>();
            foreach (var alternateIcon in alternateIcons)
            {
                iconNames.Add(alternateIcon.iconName);
                var iconDirectoryPath = Path.Combine(imagesXcassetsDirectoryPath, $"{alternateIcon.iconName}.appiconset");
                Directory.CreateDirectory(iconDirectoryPath);

                var contentsJsonPath = Path.Combine(iconDirectoryPath, "Contents.json");
                var contentsJson = ContentsJsonText;
                contentsJson = contentsJson.Replace("iPhoneContents", PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad ? ContentsiPhoneJsonText : string.Empty);
                contentsJson = contentsJson.Replace("iPadContents", PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPadOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad ? ContentsiPadJsonText : string.Empty);
                File.WriteAllText(contentsJsonPath, contentsJson, Encoding.UTF8);

                var sourceTexture = TextureAssetUtils.ProcessTexture(alternateIcon.source);
                if (PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad)
                {
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneNotification40px, 40, Path.Combine(iconDirectoryPath, "iPhoneNotification40px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneNotification60px, 60, Path.Combine(iconDirectoryPath, "iPhoneNotification60px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneSettings58px, 58, Path.Combine(iconDirectoryPath, "iPhoneSettings58px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneSettings87px, 87, Path.Combine(iconDirectoryPath, "iPhoneSettings87px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneSpotlight80px, 80, Path.Combine(iconDirectoryPath, "iPhoneSpotlight80px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneSpotlight120px, 120, Path.Combine(iconDirectoryPath, "iPhoneSpotlight120px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneApp120px, 120, Path.Combine(iconDirectoryPath, "iPhoneApp120px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPhoneApp180px, 180, Path.Combine(iconDirectoryPath, "iPhoneApp180px.png"));
                }

                if (PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPadOnly || PlayerSettings.iOS.targetDevice == iOSTargetDevice.iPhoneAndiPad)
                {
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadNotifications20px, 20, Path.Combine(iconDirectoryPath, "iPadNotifications20px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadNotifications40px, 40, Path.Combine(iconDirectoryPath, "iPadNotifications40px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadSettings29px, 29, Path.Combine(iconDirectoryPath, "iPadSettings29px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadSettings58px, 58, Path.Combine(iconDirectoryPath, "iPadSettings58px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadSpotlight40px, 40, Path.Combine(iconDirectoryPath, "iPadSpotlight40px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadSpotlight80px, 80, Path.Combine(iconDirectoryPath, "iPadSpotlight80px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadApp76px, 76, Path.Combine(iconDirectoryPath, "iPadApp76px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadApp152px, 152, Path.Combine(iconDirectoryPath, "iPadApp152px.png"));
                    SaveIcon(alternateIcon.type, sourceTexture, alternateIcon.iPadProApp167px, 167, Path.Combine(iconDirectoryPath, "iPadProApp167px.png"));
                }

                SaveIcon(alternateIcon.type, alternateIcon.source, alternateIcon.appStore1024px, 1024, Path.Combine(iconDirectoryPath, "appStore1024px.png"));
            }

            var pbxProjectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj", "project.pbxproj");
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            var targetGuid = pbxProject.GetUnityMainTargetGuid();
            var joinedIconNames = string.Join(" ", iconNames);
            pbxProject.SetBuildProperty(targetGuid, "ASSETCATALOG_COMPILER_ALTERNATE_APPICON_NAMES", joinedIconNames);

            pbxProject.WriteToFile(pbxProjectPath);
        }

        private static void SaveIcon(AlternateIconType type, Texture2D sourceTexture, Texture2D manualTexture, int size, string savePath)
        {
            if (type == AlternateIconType.AutoGenerate)
            {
	            var iconTexture = TextureAssetUtils.ScaleTexture(sourceTexture, size, size);
                var pngBytes = iconTexture.EncodeToPNG();
                File.WriteAllBytes(savePath, pngBytes);
            }
            else
            {
                if (manualTexture == null) return;
                var path = AssetDatabase.GetAssetPath(manualTexture);
                File.Copy(path, savePath, true);
            }
        }

        private const string ContentsJsonText = @"{
  ""images"" : [
	iPhoneContents
	iPadContents
	{
	  ""filename"" : ""appStore1024px.png"",
	  ""idiom"" : ""ios-marketing"",
	  ""scale"" : ""1x"",
	  ""size"" : ""1024x1024""
	}
  ],
  ""info"" : {
    ""author"" : ""xcode"",
    ""version"" : 1
  }
}
";
	
		private const string ContentsiPhoneJsonText = @"{
	  ""filename"" : ""iPhoneNotification40px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPhoneNotification60px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPhoneSettings58px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPhoneSettings87px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPhoneSpotlight80px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPhoneSpotlight120px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPhoneApp120px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""2x"",
	  ""size"" : ""60x60""
	},
	{
	  ""filename"" : ""iPhoneApp180px.png"",
	  ""idiom"" : ""iphone"",
	  ""scale"" : ""3x"",
	  ""size"" : ""60x60""
	},
";

		private const string ContentsiPadJsonText = @"{
	  ""filename"" : ""iPadNotifications20px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPadNotifications40px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""20x20""
	},
	{
	  ""filename"" : ""iPadSettings29px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPadSettings58px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""29x29""
	},
	{
	  ""filename"" : ""iPadSpotlight40px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPadSpotlight80px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""40x40""
	},
	{
	  ""filename"" : ""iPadApp76px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""1x"",
	  ""size"" : ""76x76""
	},
	{
	  ""filename"" : ""iPadApp152px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""76x76""
	},
	{
	  ""filename"" : ""iPadProApp167px.png"",
	  ""idiom"" : ""ipad"",
	  ""scale"" : ""2x"",
	  ""size"" : ""83.5x83.5""
	},
";
	}
}
#endif
