using System.IO;
using System.Linq;
using Unity1week202504.Data;
using Unity1week202504.Data.Ending;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    /// <summary>
    /// EndingMasterDataが追加または変更されたときに自動的にEndingMasterDataSourceを更新するエディタ拡張
    /// </summary>
    public class EndingMasterDataProcessor : AssetPostprocessor
    {
        private const string EndingMasterDataPath = "Assets/Application/ScriptableObjects/MasterData/Ending";
        private const string EndingMasterDataSourcePath = "Assets/Application/ScriptableObjects/MasterData/Ending/EndingMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Ending関連のアセットが変更されたかチェック
            var isChanged = HasEndingMasterDataChanged(importedAssets)
                            || HasEndingMasterDataChanged(deletedAssets)
                            || HasEndingMasterDataChanged(movedAssets)
                            || HasEndingMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、EndingMasterDataSourceを更新
            if (isChanged)
            {
                UpdateEndingMasterDataSource();
            }
        }
        
        private static bool HasEndingMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(EndingMasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("EndingMasterDataSource"));
        }

        /// <summary>
        /// EndingMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdateEndingMasterDataSource()
        {
            // EndingMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<EndingMasterDataSource>(EndingMasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(EndingMasterDataPath);
                dataSource = ScriptableObject.CreateInstance<EndingMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, EndingMasterDataSourcePath);
                Debug.Log($"Created EndingMasterDataSource at {EndingMasterDataSourcePath}");
            }

            // EndingMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:EndingMasterData", new[] { EndingMasterDataPath });
            var endingMasterDataList = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.Contains("EndingMasterDataSource")) // DataSourceは除外
                .Select(path => AssetDatabase.LoadAssetAtPath<EndingMasterData>(path))
                .Where(data => data != null)
                .ToArray();

            // EndingMasterDataSourceに設定
            SerializedObject serializedObject = new SerializedObject(dataSource);
            SerializedProperty endingMasterDataProperty = serializedObject.FindProperty("_endingMasterData");
            
            endingMasterDataProperty.arraySize = endingMasterDataList.Length;
            for (int i = 0; i < endingMasterDataList.Length; i++)
            {
                endingMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = endingMasterDataList[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Updated EndingMasterDataSource with {endingMasterDataList.Length} items");
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
        [MenuItem("Tools/MasterData/Update Ending Master Data Source")]
        private static void ManualUpdateEndingMasterDataSource()
        {
            UpdateEndingMasterDataSource();
        }
    }
} 