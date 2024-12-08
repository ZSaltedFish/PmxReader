//using UnityEditor;
//using UnityEngine;

//namespace ZKnight.PmxReader.Editor
//{
//    [CustomEditor(typeof(PmxImporter))]
//    public class PmxImporterEditor : UnityEditor.Editor
//    {
//        private bool _isChanged = false;
//        private SerializedObject _copy;

//        private void OnEnable()
//        {
//            _copy = serializedObject;
//        }

//        public override void OnInspectorGUI()
//        {
//            var currentGroup = Undo.GetCurrentGroup();
//            Undo.IncrementCurrentGroup();
//            using var check = new EditorGUI.ChangeCheckScope();
//            _copy.Update();
//            var factorProp = _copy.FindProperty("Factor");
//            EditorGUILayout.PropertyField(factorProp);

//            using (new EditorGUI.DisabledScope(!_isChanged))
//            {
//                using (new EditorGUILayout.HorizontalScope())
//                {
//                    GUILayout.FlexibleSpace();
//                    if (GUILayout.Button("Revert", GUILayout.Width(80)))
//                    {
//                        Revert();
//                    }

//                    if (GUILayout.Button("Apply", GUILayout.Width(80)))
//                    {
//                        Apply();
//                    }
//                }
//            }

//            if (check.changed)
//            {
//                _isChanged = true;
//                _copy.ApplyModifiedProperties();
//                Undo.RecordObject(target, "Changed import settings");
//            }

//            Undo.CollapseUndoOperations(currentGroup);
//        }

//        public void OnDestroy()
//        {
//            if (_isChanged)
//            {
//                var displayTitle = "Unapplied import settings";
//                var displayString = $"Unapplied import settings for '{target.name}'";
//                if (EditorUtility.DisplayDialog(displayTitle, displayString, "Apply", "Revert"))
//                {
//                    Apply();
//                }
//                else
//                {
//                    Revert();
//                }
//            }
//        }

//        private void Revert()
//        {
//            _isChanged = false;
//            Undo.PerformUndo();
//        }

//        private void Apply()
//        {
//            _isChanged = false;
//            var importer = target as PmxImporter;
//            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(importer));
//        }
//    }
//}
