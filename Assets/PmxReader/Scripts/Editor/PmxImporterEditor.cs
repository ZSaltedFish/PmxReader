using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    [CustomEditor(typeof(PmxImporter))]
    public class PmxImporterEditor : AssetImporterEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var avatarProp = serializedObject.FindProperty("ModeAvatar");

            if (avatarProp.objectReferenceValue != null && GUILayout.Button("Reset Avatar"))
            {
                Debug.Log("Reset Avatar");
            }
        }
    }
}
