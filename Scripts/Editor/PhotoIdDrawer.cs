using System.Collections.Generic;
using System.Linq;
using Alchemy.Editor;
using Alchemy.Editor.Elements;
using Unity1week202504.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

namespace Unity1week202504.Editor
{
    [CustomAttributeDrawer(typeof(PhotoIdAttribute))]
    public sealed class PhotoIdDrawer : AlchemyAttributeDrawer
    {
        private DropdownField _dropdownField;
        private InlineEditorObjectField _photoMasterDataField;

        public override void OnCreateElement()
        {
            var findAssets = AssetDatabase.FindAssets($"t:{nameof(PhotoMasterData)}");
            var photoMasterDatas = findAssets.AsValueEnumerable()
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PhotoMasterData>)
                .OrderBy(x => x.Id.AsPrimitive())
                .ToList();

            var dropDownChoices = photoMasterDatas
                .Select(ToDropDownDisplayName)
                .ToList();
            
            var selected = photoMasterDatas.Find(data => data.Id.AsPrimitive() == SerializedProperty.intValue);
            var defaultIndex = photoMasterDatas.IndexOf(selected);
            if (defaultIndex == -1)
            {
                // 選択肢が見つからない場合は、最初の要素を選択
                defaultIndex = 0;
            }

            _dropdownField = new DropdownField(
                "一覧から指定",
                dropDownChoices,
                defaultIndex: defaultIndex,
                formatSelectedValueCallback: s =>
                {
                    // 先頭の数字を取得
                    // 例: "1 - PhotoName" -> "1"
                    var idStr = s.Split(' ').FirstOrDefault();
                    if (int.TryParse(idStr, out var id))
                    {
                        var masterData = photoMasterDatas.Find(data => data.Id.AsPrimitive() == id);
                        return ToDropDownDisplayName(masterData);
                    }

                    return s;
                });

            _dropdownField.RegisterValueChangedCallback(v =>
            {
                SerializedObject.Update();
                var idStr = v.newValue.Split(' ').FirstOrDefault();
                if (int.TryParse(idStr, out var id))
                {
                    SerializedProperty.intValue = id;
                }

                SerializedProperty.serializedObject.ApplyModifiedProperties();
            });

            TargetElement.RegisterCallback<ChangeEvent<int>>(evt =>
            {
                var target= photoMasterDatas.Find(data => data.Id.AsPrimitive() == evt.newValue);
                _dropdownField.value = target == null
                    ? "None"
                    : ToDropDownDisplayName(target);
            });

            var parent = TargetElement.parent;

            // 親要素にラベルとドロップダウンを追加
            parent.Insert(parent.IndexOf(TargetElement) + 1, _dropdownField);
        }

        private static string ToDropDownDisplayName(PhotoMasterData masterData)
        {
            var photoName = string.IsNullOrEmpty(masterData.PhotoName) ? "None" : masterData.PhotoName;
            return $"{masterData.Id.AsPrimitive()} - {photoName}";
        }
    }
}