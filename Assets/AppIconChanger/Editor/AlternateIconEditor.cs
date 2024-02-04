using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AppIconChanger.Editor
{
    [CustomEditor(typeof(AlternateIcon))]
    public class AlternateIconEditor : UnityEditor.Editor
    {
        private static readonly List<(string FieldName, string Label, int Size)> sManualIcons = new List<(string, string, int)>
        {
            ("iPhoneNotification40px", "x2 - 40px", 40),
            ("iPhoneNotification60px", "x3 - 60px", 60)
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var typed = (AlternateIcon) target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("iconName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));

            if (typed.type == AlternateIconType.AutoGenerate)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("source"));
                VerifyTextureOrShowHelpBox(typed.source, 1024);
            }
            else
            {
                EditorGUILayout.Space();
                {
                    EditorGUILayout.LabelField("iPhone Notification");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneNotification40px"), new GUIContent("x2 - 40px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneNotification40px, 40);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneNotification60px"), new GUIContent("x3 - 60px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneNotification60px, 60);
                }
                {
                    EditorGUILayout.LabelField("iPhone Settings");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSettings58px"), new GUIContent("x2 - 58px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneSettings58px, 58);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSettings87px"), new GUIContent("x3 - 87px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneSettings87px, 87);
                }
                {
                    EditorGUILayout.LabelField("iPhone Spotlight");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSpotlight80px"), new GUIContent("x2 - 80px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneSpotlight80px, 80);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneSpotlight120px"), new GUIContent("x3 - 120px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneSpotlight120px, 120);
                }
                {
                    EditorGUILayout.LabelField("iPhone App");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneApp120px"), new GUIContent("x2 - 120px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneApp120px, 120);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPhoneApp180px"), new GUIContent("x3 - 180px"));
                    VerifyTextureOrShowHelpBox(typed.iPhoneApp180px, 180);
                }
                {
                    EditorGUILayout.LabelField("iPad Notifications");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadNotifications20px"), new GUIContent("x1 - 20px"));
                    VerifyTextureOrShowHelpBox(typed.iPadNotifications20px, 20);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadNotifications40px"), new GUIContent("x2 - 40px"));
                    VerifyTextureOrShowHelpBox(typed.iPadNotifications40px, 40);
                }
                {
                    EditorGUILayout.LabelField("iPad Settings");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSettings29px"), new GUIContent("x1 - 29px"));
                    VerifyTextureOrShowHelpBox(typed.iPadSettings29px, 29);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSettings58px"), new GUIContent("x2 - 58px"));
                    VerifyTextureOrShowHelpBox(typed.iPadSettings58px, 58);
                }
                {
                    EditorGUILayout.LabelField("iPad Spotlight");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSpotlight40px"), new GUIContent("x1 - 40px"));
                    VerifyTextureOrShowHelpBox(typed.iPadSpotlight40px, 40);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadSpotlight80px"), new GUIContent("x2 - 80px"));
                    VerifyTextureOrShowHelpBox(typed.iPadSpotlight80px, 80);
                }
                {
                    EditorGUILayout.LabelField("iPad App");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadApp76px"), new GUIContent("x1 - 76px"));
                    VerifyTextureOrShowHelpBox(typed.iPadApp76px, 76);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadApp152px"), new GUIContent("x2 - 152px"));
                    VerifyTextureOrShowHelpBox(typed.iPadApp152px, 152);
                }
                {
                    EditorGUILayout.LabelField("iPadPro App");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("iPadProApp167px"), new GUIContent("x2 - 167px"));
                    VerifyTextureOrShowHelpBox(typed.iPadProApp167px, 167);
                }
                {
                    EditorGUILayout.LabelField("AppStore");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("appStore1024px"), new GUIContent("x1 - 1024px"));
                    VerifyTextureOrShowHelpBox(typed.appStore1024px, 1024);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void VerifyTextureOrShowHelpBox(Texture2D tex, int size)
        {
            if (TextureAssetUtils.VerifyTexture(tex, size, out var errors)) return;

            foreach (var error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
        }
    }
}
