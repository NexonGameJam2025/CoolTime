#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.Table
{
    [CustomEditor(typeof(Table<,>), true)]
    public class TableInspector : Editor
    {
        UnityEditorInternal.ReorderableList mDataList;

        public void OnEnable()
        {
            var property = serializedObject.FindProperty("mDataList");
            mDataList = ReorderableListUtility.CreateAutoLayout(property);
            mDataList.serializedProperty.isExpanded = false;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ReorderableListUtility.DoLayoutListWithFoldout(mDataList, "Datas");
            if (GUILayout.Button("Modify Table"))
            {
                TableEditWindow.ShowUI((Table)this.serializedObject.targetObject);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif