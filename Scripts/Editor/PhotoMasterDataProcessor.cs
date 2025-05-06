using System.IO;
using System.Linq;
using Unity1week202504.Data;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    /// <summary>
    /// PhotoMasterDataが追加または変更されたときに自動的にPhotoMasterDataSourceを更新するエディタ拡張
    /// </summary>
    public class PhotoMasterDataProcessor : AssetPostprocessor
    {
        private const string PhotoMasterDataPath = "Assets/Application/ScriptableObjects/MasterData/Photo";
        private const string PhotoMasterDataSourcePath = "Assets/Application/ScriptableObjects/MasterData/Photo/PhotoMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Photo関連のアセットが変更されたかチェック
            var isChanged = HasPhotoMasterDataChanged(importedAssets)
                            || HasPhotoMasterDataChanged(deletedAssets)
                            || HasPhotoMasterDataChanged(movedAssets)
                            || HasPhotoMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、PhotoMasterDataSourceを更新
            if (isChanged)
            {
                UpdatePhotoMasterDataSource();
            }
        }
        
        private static bool HasPhotoMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(PhotoMasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("PhotoMasterDataSource"));
        }

        /// <summary>
        /// PhotoMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdatePhotoMasterDataSource()
        {
            // PhotoMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<PhotoMasterDataSource>(PhotoMasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(PhotoMasterDataPath);
                dataSource = ScriptableObject.CreateInstance<PhotoMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, PhotoMasterDataSourcePath);
                Debug.Log($"Created PhotoMasterDataSource at {PhotoMasterDataSourcePath}");
            }

            // PhotoMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:PhotoMasterData", new[] { PhotoMasterDataPath });
            var photoMasterDataList = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.Contains("PhotoMasterDataSource")) // DataSourceは除外
                .Select(path => AssetDatabase.LoadAssetAtPath<PhotoMasterData>(path))
                .Where(data => data != null)
                .ToArray();

            // PhotoMasterDataSourceに設定
            SerializedObject serializedObject = new SerializedObject(dataSource);
            SerializedProperty photoMasterDataProperty = serializedObject.FindProperty("_photoMasterData");
            
            photoMasterDataProperty.arraySize = photoMasterDataList.Length;
            for (int i = 0; i < photoMasterDataList.Length; i++)
            {
                photoMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = photoMasterDataList[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Updated PhotoMasterDataSource with {photoMasterDataList.Length} items");
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
        [MenuItem("Tools/MasterData/Update Photo Master Data Source")]
        private static void ManualUpdatePhotoMasterDataSource()
        {
            UpdatePhotoMasterDataSource();
        }
    }
} 