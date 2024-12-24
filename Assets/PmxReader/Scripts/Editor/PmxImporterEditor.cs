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
            var boneObjectProp = serializedObject.FindProperty("BoneRoot");

            if (avatarProp.objectReferenceValue != null && GUILayout.Button("Reset Avatar"))
            {
                var rootObject = boneObjectProp.objectReferenceValue as GameObject;
                if (rootObject == null)
                {
                    Debug.LogError("Root Object is null");
                    return;
                }

                var avatar = avatarProp.objectReferenceValue as Avatar;
                if (avatar == null)
                {
                    Debug.LogError("Avatar is null");
                    return;
                }
                
                AvatarEditor.ShowWindow(rootObject, OnSaveCallBack, avatar);
            }
        }

        private void OnSaveCallBack(HumanDescription humanDescription)
        {
            var avatarProp = serializedObject.FindProperty("ModeAvatar");
            var rootObject = serializedObject.FindProperty("RootObject").objectReferenceValue as GameObject;
            var rootBone = serializedObject.FindProperty("BoneRoot").objectReferenceValue as GameObject;
            var avatar = AvatarBuilder.BuildHumanAvatar(rootBone, humanDescription);
            AssetDatabase.CreateAsset(avatar, AssetDatabase.GenerateUniqueAssetPath("Assets/Avatar.asset"));
            AssetDatabase.Refresh();
            avatar = AssetDatabase.LoadAssetAtPath<Avatar>("Assets/Avatar.asset");

            avatar.name = $"{rootObject.name}_Avatar";
            avatarProp.objectReferenceValue = avatar;
            serializedObject.ApplyModifiedProperties();

            // Reimport the asset
            var importer = target as PmxImporter;
            if (importer == null) return;
            EditorUtility.SetDirty(target);
            importer.SaveAndReimport();

            AssetDatabase.DeleteAsset("Assets/Avatar.asset");
            AssetDatabase.Refresh();
        }
    }
}
