using System;
using System.Linq;
using Unity1week202504.Data;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    public class MemoryConditionMasterDataEditorWindow : EditorWindow
    {
        private MemoryConditionMasterData[] _assets = Array.Empty<MemoryConditionMasterData>();
        private Vector2 _scrollPosition;

        [MenuItem("Tools/MemoryConditionMasterDataEditor")]
        private static void Open()
        {
            var window = GetWindow<MemoryConditionMasterDataEditorWindow>("MemoryConditionMasterDataEditorWindow");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button($"{nameof(MemoryConditionMasterData)}を取得"))
            {
                var dataStoreSource = AssetDatabase.FindAssets($"t:{nameof(MemoryConditionMasterData)}");
                if (dataStoreSource.Length == 0)
                {
                    Debug.LogError($"{nameof(MemoryConditionMasterData)} is not found.");
                    return;
                }


                _assets = AssetDatabase.FindAssets($"t:{nameof(MemoryConditionMasterData)}")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<MemoryConditionMasterData>)
                    .OrderBy(x => x.Id.AsPrimitive())
                    .ToArray();
            }

            if (!_assets.Any())
            {
                EditorGUILayout.LabelField("MonsterMasterDataStoreSource is not found.");
                return;
            }

            using (new GUILayout.HorizontalScope(GUILayout.Height(20)))
            {
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Assets", GUILayout.MaxWidth(50));
                VerticalLine();
                EditorGUILayout.LabelField("ID", GUILayout.Width(50));
                VerticalLine();
                EditorGUILayout.LabelField("Photo1", GUILayout.MaxWidth(50));
                VerticalLine();
                EditorGUILayout.LabelField("Photo2", GUILayout.MaxWidth(50));
                VerticalLine();
                EditorGUILayout.LabelField("Comment", GUILayout.MinWidth(50));
                // スクロールバー分の幅を確保
                GUILayout.Space(13);
            }

            HorizontalLine();

            using (var scrollView = new GUILayout.ScrollViewScope(_scrollPosition, false, true))
            {
                foreach (var data in _assets)
                {
                    using (new GUILayout.HorizontalScope(GUILayout.Height(20)))
                    {
                        GUILayout.Space(5);
                        var serializedObject = new SerializedObject(data);

                        serializedObject.Update();

                        EditorGUILayout.ObjectField(serializedObject.targetObject,
                            typeof(MemoryConditionMasterData), false,
                            GUILayout.MaxWidth(50));

                        const string id = "_id";
                        var idProperty = serializedObject.FindProperty(id);
                        EditorGUI.BeginChangeCheck();
                        int newIdValue = EditorGUILayout.IntField(idProperty.intValue, GUILayout.MaxWidth(50));
                        if (EditorGUI.EndChangeCheck())
                        {
                            idProperty.intValue = newIdValue;
                        }

                        VerticalLine();

                        var photoId1Property = serializedObject.FindProperty("_photoId1");
                        EditorGUI.BeginChangeCheck();
                        int newPhotoId1Value =
                            EditorGUILayout.IntField(photoId1Property.intValue, GUILayout.MaxWidth(50));
                        if (EditorGUI.EndChangeCheck())
                        {
                            photoId1Property.intValue = newPhotoId1Value;
                        }

                        VerticalLine();

                        var photoId2Property = serializedObject.FindProperty("_photoId2");
                        EditorGUI.BeginChangeCheck();
                        int newPhotoId2Value =
                            EditorGUILayout.IntField(photoId2Property.intValue, GUILayout.MaxWidth(50));
                        if (EditorGUI.EndChangeCheck())
                        {
                            photoId2Property.intValue = newPhotoId2Value;
                        }

                        VerticalLine();

                        var commentProperty = serializedObject.FindProperty("_comment");
                        EditorGUI.BeginChangeCheck();
                        string newCommentValue =
                            EditorGUILayout.TextField(commentProperty.stringValue, GUILayout.MinWidth(200));
                        if (EditorGUI.EndChangeCheck())
                        {
                            commentProperty.stringValue = newCommentValue;
                        }

                        VerticalLine();

                        // 変更があった場合に適用
                        if (serializedObject.hasModifiedProperties)
                        {
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
                }

                _scrollPosition = scrollView.scrollPosition;
            }
        }

        private void HorizontalLine()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }

        private void VerticalLine()
        {
            GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
        }
    }
}