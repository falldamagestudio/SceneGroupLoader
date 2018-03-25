using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SceneGroupLoader
{
    /// <summary>
    /// Pretty scene reference
    /// taken from https://answers.unity.com/questions/242794/inspector-field-for-scene-asset.html?childToView=1374414#comment-1374414
    /// </summary>
    [Serializable]
    public class SceneField
    {
        [SerializeField] private UnityEngine.Object sceneAsset;
        [SerializeField] private string sceneName = "";

        public string SceneName
        {
            get { return sceneName; }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAsset.objectReferenceValue = value;
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                        var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                        var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                        scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                        sceneName.stringValue = scenePath;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}