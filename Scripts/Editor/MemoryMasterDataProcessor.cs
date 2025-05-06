using System.IO;
using System.Linq;
using Unity1week202504.Data.Memories;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    /// <summary>
    /// MemoryMasterDataが追加または変更されたときに自動的にMemoryMasterDataSourceを更新するエディタ拡張
    /// </summary>
    public class MemoryMasterDataProcessor : AssetPostprocessor
    {
        private const string MemoryMasterDataPath = "Assets/Application/ScriptableObjects/MasterData/Memory";
        private const string MemoryMasterDataSourcePath = "Assets/Application/ScriptableObjects/MasterData/Memory/MemoryMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Memory関連のアセットが変更されたかチェック
            var isChanged = HasMemoryMasterDataChanged(importedAssets)
                            || HasMemoryMasterDataChanged(deletedAssets)
                            || HasMemoryMasterDataChanged(movedAssets)
                            || HasMemoryMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、MemoryMasterDataSourceを更新
            if (isChanged)
            {
                UpdateMemoryMasterDataSource();
            }
        }
        
        private static bool HasMemoryMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(MemoryMasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("MemoryMasterDataSource"));
        }

        /// <summary>
        /// MemoryMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdateMemoryMasterDataSource()
        {
            // MemoryMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<MemoryMasterDataSource>(MemoryMasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(MemoryMasterDataPath);
                dataSource = ScriptableObject.CreateInstance<MemoryMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, MemoryMasterDataSourcePath);
                Debug.Log($"Created MemoryMasterDataSource at {MemoryMasterDataSourcePath}");
            }

            // MemoryMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:MemoryMasterData", new[] { MemoryMasterDataPath });
            var memoryMasterDataList = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.Contains("MemoryMasterDataSource")) // DataSourceは除外
                .Select(path => AssetDatabase.LoadAssetAtPath<MemoryMasterData>(path))
                .Where(data => data != null)
                .ToArray();

            // MemoryMasterDataSourceに設定
            SerializedObject serializedObject = new SerializedObject(dataSource);
            SerializedProperty memoryMasterDataProperty = serializedObject.FindProperty("_memoryMasterData");
            
            memoryMasterDataProperty.arraySize = memoryMasterDataList.Length;
            for (int i = 0; i < memoryMasterDataList.Length; i++)
            {
                memoryMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = memoryMasterDataList[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Updated MemoryMasterDataSource with {memoryMasterDataList.Length} items");
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
        [MenuItem("Tools/MasterData/Update Memory Master Data Source")]
        private static void ManualUpdateMemoryMasterDataSource()
        {
            UpdateMemoryMasterDataSource();
        }
    }
} 