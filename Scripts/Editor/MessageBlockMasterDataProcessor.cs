using System.IO;
using System.Linq;
using Unity1week202504.Data;
using Unity1week202504.Data.Messages;
using UnityEditor;
using UnityEngine;

namespace Unity1week202504.Editor
{
    /// <summary>
    /// MessageBlockMasterDataが追加または変更されたときに自動的にMessageBlockMasterDataSourceを更新するエディタ拡張
    /// </summary>
    public class MessageBlockMasterDataProcessor : AssetPostprocessor
    {
        private const string MessageBlockMasterDataPath = "Assets/Application/ScriptableObjects/MasterData/MessageBlock";
        private const string MessageBlockMasterDataSourcePath = "Assets/Application/ScriptableObjects/MasterData/MessageBlock/MessageBlockMasterDataSource.asset";

        // Asset変更時に呼ばれるコールバック
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // MessageBlock関連のアセットが変更されたかチェック
            var isChanged = HasMessageBlockMasterDataChanged(importedAssets)
                            || HasMessageBlockMasterDataChanged(deletedAssets)
                            || HasMessageBlockMasterDataChanged(movedAssets)
                            || HasMessageBlockMasterDataChanged(movedFromAssetPaths);

            // 変更があった場合、MessageBlockMasterDataSourceを更新
            if (isChanged)
            {
                UpdateMessageBlockMasterDataSource();
            }
        }
        
        private static bool HasMessageBlockMasterDataChanged(string[] paths)
        {
            return paths.Any(path => path.StartsWith(MessageBlockMasterDataPath)
                                     && path.EndsWith(".asset")
                                     && !path.Contains("MessageBlockMasterDataSource"));
        }

        /// <summary>
        /// MessageBlockMasterDataSourceアセットを更新する
        /// </summary>
        private static void UpdateMessageBlockMasterDataSource()
        {
            // MessageBlockMasterDataSourceアセットの存在確認
            var dataSource = AssetDatabase.LoadAssetAtPath<MessageBlockMasterDataSource>(MessageBlockMasterDataSourcePath);
            if (dataSource == null)
            {
                // 存在しない場合は新規作成
                CreateDirectoryIfNeeded(MessageBlockMasterDataPath);
                dataSource = ScriptableObject.CreateInstance<MessageBlockMasterDataSource>();
                AssetDatabase.CreateAsset(dataSource, MessageBlockMasterDataSourcePath);
                Debug.Log($"Created MessageBlockMasterDataSource at {MessageBlockMasterDataSourcePath}");
            }

            // MessageBlockMasterDataアセットの一覧を取得
            string[] guids = AssetDatabase.FindAssets("t:MessageBlockMasterData", new[] { MessageBlockMasterDataPath });
            var messageBlockMasterDataList = guids
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.Contains("MessageBlockMasterDataSource")) // DataSourceは除外
                .Select(path => AssetDatabase.LoadAssetAtPath<MessageBlockMasterData>(path))
                .Where(data => data != null)
                .ToArray();

            // MessageBlockMasterDataSourceに設定
            SerializedObject serializedObject = new SerializedObject(dataSource);
            SerializedProperty messageBlockMasterDataProperty = serializedObject.FindProperty("_messageBlockMasterData");
            
            messageBlockMasterDataProperty.arraySize = messageBlockMasterDataList.Length;
            for (int i = 0; i < messageBlockMasterDataList.Length; i++)
            {
                messageBlockMasterDataProperty.GetArrayElementAtIndex(i).objectReferenceValue = messageBlockMasterDataList[i];
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(dataSource);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"Updated MessageBlockMasterDataSource with {messageBlockMasterDataList.Length} items");
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
        [MenuItem("Tools/MasterData/Update MessageBlock Master Data Source")]
        private static void ManualUpdateMessageBlockMasterDataSource()
        {
            UpdateMessageBlockMasterDataSource();
        }
    }
} 