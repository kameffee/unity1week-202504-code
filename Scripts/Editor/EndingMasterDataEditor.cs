using System.Linq;
using NUnit.Framework;
using Unity1week202504.Data.Ending;
using Unity1week202504.Data.Memories;
using Unity1week202504.InGame.Memories;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace Unity1week202504.Editor
{
    [CustomEditor(typeof(EndingMasterData))]
    public class EndingMasterDataEditor : UnityEditor.Editor
    {
        private const string MemoryMasterDataSourcePath =
            "Assets/Application/ScriptableObjects/MasterData/Memory/MemoryMasterDataSource.asset";

        private MemoryMasterDataSource _memoryMasterDataSource;

        private void OnEnable()
        {
            _memoryMasterDataSource ??=
                AssetDatabase.LoadAssetAtPath<MemoryMasterDataSource>(MemoryMasterDataSourcePath);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Assert.IsNotNull(_memoryMasterDataSource, "MemoryMasterDataSource is null");

            var iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    EditorGUILayout.PropertyField(iterator, true);

                if (iterator.propertyPath.StartsWith("_condition"))
                {
                    var value = iterator.intValue;
                    var index = _memoryMasterDataSource.All
                        .Select(x => x.Id)
                        .ToList()
                        .IndexOf(new MemoryId(value));

                    var options = _memoryMasterDataSource.All
                        .Select(x => x.DisplayName)
                        .Prepend("Any")
                        .ToArray();
    
                    var currentIndex = index >= 0 ? index + 1 : 0;
    
                    var newIndex = EditorGUILayout.Popup(
                        "一覧指定",
                        currentIndex,
                        options);
    
                    if (newIndex != currentIndex)
                    {
                        if (newIndex == 0)
                        {
                            iterator.intValue = -1;
                        }
                        else
                        {
                            iterator.intValue = _memoryMasterDataSource.All[newIndex - 1].Id.AsPrimitive();
                        }
        
                        EditorUtility.SetDirty(target);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}