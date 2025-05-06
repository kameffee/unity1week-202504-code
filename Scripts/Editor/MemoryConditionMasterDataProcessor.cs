using System.IO;
using System.Linq;
using Unity1week202504.Data;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    /// <summary>
    /// MemoryConditionMasterDataが追加または変更されたときに自動的にMemoryConditionMasterDataSourceを更新するエディタ拡張
    /// </summary>
    public class MemoryConditionMasterDataProcessor : AssetPostprocessor
    {
        private const string MemoryConditionMasterDataPath = "Assets/Application/ScriptableObjects/MasterData/MemoryCondition";
        private const string MemoryConditionMasterDataSourcePath = "Assets/Application/ScriptableObjects/MasterData/MemoryCondition/MemoryConditionMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // MemoryCondition関連のアセットが変更されたかチェック
            var isChanged = HasMemoryConditionMasterDataChanged(importedAssets)
                            || HasMemoryConditionMasterDataChanged(deletedAssets)
                            || HasMemoryConditionMasterDataChanged(movedAssets)
                            || HasMemoryConditionMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、MemoryConditionMasterDataSourceを更新
            if (isChanged)
            {
                UpdateMemoryConditionMasterDataSource();
            }
        }
        
        private static bool HasMemoryConditionMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(MemoryConditionMasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("MemoryConditionMasterDataSource"));
        }

        /// <summary>
        /// MemoryConditionMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdateMemoryConditionMasterDataSource()
        {
            // MemoryConditionMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<MemoryConditionMasterDataSource>(MemoryConditionMasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(MemoryConditionMasterDataPath);
                dataSource = ScriptableObject.CreateInstance<MemoryConditionMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, MemoryConditionMasterDataSourcePath);
                Debug.Log($"Created MemoryConditionMasterDataSource at {MemoryConditionMasterDataSourcePath}");
            }

            // MemoryConditionMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:MemoryConditionMasterData", new[] { MemoryConditionMasterDataPath });
            var memoryConditionMasterDataList = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.Contains("MemoryConditionMasterDataSource")) // DataSourceは除外
                .Select(path => AssetDatabase.LoadAssetAtPath<MemoryConditionMasterData>(path))
                .Where(data => data != null)
                .ToArray();

            // MemoryConditionMasterDataSourceに設定
            SerializedObject serializedObject = new SerializedObject(dataSource);
            SerializedProperty memoryConditionMasterDataProperty = serializedObject.FindProperty("_memoryConditionMasterData");
            
            memoryConditionMasterDataProperty.arraySize = memoryConditionMasterDataList.Length;
            for (int i = 0; i < memoryConditionMasterDataList.Length; i++)
            {
                memoryConditionMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = memoryConditionMasterDataList[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Updated MemoryConditionMasterDataSource with {memoryConditionMasterDataList.Length} items");
        }

        /// <summary>
        /// 指定されたパスにディレクトリが存在しない場合、作成する
        /// </summary>
        private static void CreateDirectoryIfNeeded(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath)) return;

            // ディレクトリが存在しない場合は作成
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        // メニューアイテムから手動で更新する機能
        [MenuItem("Tools/MasterData/Update Memory Condition Master Data Source")]
        private static void ManualUpdateMemoryConditionMasterDataSource()
        {
            UpdateMemoryConditionMasterDataSource();
        }
    }
} 