#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.Table
{
    [CustomEditor(typeof(Tables))]
    public class TablesInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tableList"), true);
            if (GUILayout.Button("Add Table"))
            {
                TableCreatorWindow.ShowUI();
            }
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Table/Create Tables")]
        static void CreateExampleAsset()
        {
            TableCreatorWindow.ShowUI();
        }
    }
}
#endif