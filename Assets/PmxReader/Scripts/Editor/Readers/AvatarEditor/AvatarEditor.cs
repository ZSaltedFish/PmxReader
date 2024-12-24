using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public static class AvatarEditorStyle
    {
        public static GUIStyle SelectedStyle
        {
            get
            {
                if (_selectedStyle != null) return _selectedStyle;
                _selectedStyle = new GUIStyle(EditorStyles.objectField);
                _selectedStyle.normal.background = _selectedStyle.active.background;
                return _selectedStyle;
            }
        }

        private static GUIStyle _selectedStyle;
    }
    public class AvatarEditor : EditorWindow
    {
        private GameObject _boneRoot;
        private Avatar _creatingAvatar;
        private HumanDescription _humanDescription;
        private Dictionary<string, Transform> _boneMap;
        private Vector2 _scrollPosition, _selectionBonePosition;
        private Action<HumanDescription> _saveCallback;
        private Dictionary<HumanBodyBones, Transform> _boneTransforms;
        private HumanBodyBones _selectedBone;
        private string _filter = "";

        public bool Init()
        {
            if (_boneRoot == null)
            {
                Debug.LogError("Root Object is null");
                return false;
            }

            if (_humanDescription.human == null || _humanDescription.skeleton == null)
            {
                _humanDescription = new HumanDescription();
            }

            _boneMap = new Dictionary<string, Transform>();
            _boneTransforms = new Dictionary<HumanBodyBones, Transform>();
            foreach (var bone in _boneRoot.GetComponentsInChildren<Transform>())
            {
                _boneMap[bone.name] = bone;
            }

            foreach (var bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                var humanName = Enum.GetName(typeof(HumanBodyBones), bone);
                var boneTransform = _humanDescription.human.FirstOrDefault(h => h.humanName == humanName);
                if (!string.IsNullOrEmpty(boneTransform.boneName) && _boneMap.ContainsKey(boneTransform.boneName))
                {
                    _boneTransforms[(HumanBodyBones) bone] = _boneMap[boneTransform.boneName];
                }
                else
                {
                    _boneTransforms[(HumanBodyBones) bone] = null;
                }
            }
            return true;
        }

        public void OnGUI()
        {
            if (_boneMap == null)
            {
                if (!Init()) return;
            }

            EditorGUILayout.LabelField($"Root Object: {_boneRoot.name}");
            
            using (new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
                {
                    _scrollPosition = scrollView.scrollPosition;
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(500f)))
                    {
                        var keys = new List<HumanBodyBones>(_boneTransforms.Keys);
                        foreach (var key in keys)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                var bone = _boneTransforms[key];
                                var rect = EditorGUILayout.GetControlRect();
                                if (_selectedBone == key)
                                {
                                    EditorGUI.DrawRect(rect, GUI.skin.settings.selectionColor);
                                }
                                _ = EditorGUI.ObjectField(rect, key.ToString(), bone, typeof(Transform), true) as Transform;
                                if (GUILayout.Button("Select", GUILayout.Width(50)))
                                {
                                    _selectedBone = key;
                                    if (_boneTransforms[key] != null)
                                    {
                                        _filter = _boneTransforms[key].name;
                                    }
                                }
                            }
                        }
                    }
                }
                using (var scrollView = new EditorGUILayout.ScrollViewScope(_selectionBonePosition, EditorStyles.helpBox))
                {
                    _selectionBonePosition = scrollView.scrollPosition;
                    _filter = EditorGUILayout.TextField("Filter", _filter);
                    
                    foreach (var bone in _boneMap)
                    {
                        if (!string.IsNullOrEmpty(_filter) && !bone.Key.Contains(_filter)) continue;
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.ObjectField("", bone.Value, typeof(Transform), true);
                            if (GUILayout.Button("Select", GUILayout.Width(50)))
                            {
                                _boneTransforms[_selectedBone] = bone.Value;
                            }
                        }
                    }
                }
            }

            if (GUILayout.Button("Save"))
            {
                if (_creatingAvatar != null)
                {
                    var humanDescription = new HumanDescription();
                    var humanoids = new List<HumanBone>();
                    var keys = new List<HumanBodyBones>(_boneTransforms.Keys);
                    foreach (var key in keys)
                    {
                        var bone = _boneTransforms[key];
                        var humanName = Enum.GetName(typeof(HumanBodyBones), key);
                        var boneTransform = new HumanBone
                        {
                            humanName = humanName
                        };
                        if (bone != null)
                        {
                            boneTransform.boneName = bone.name;
                            humanoids.Add(boneTransform);
                        }
                    }

                    var skeletons = new List<SkeletonBone>();
                    foreach (var bone in _boneMap)
                    {
                        var skeleton = new SkeletonBone
                        {
                            name = bone.Key,
                            position = bone.Value.localPosition,
                            rotation = bone.Value.localRotation,
                            scale = bone.Value.localScale
                        };
                        skeletons.Add(skeleton);
                    }
                    humanDescription.human = humanoids.ToArray();
                    humanDescription.skeleton = skeletons.ToArray();
                    _saveCallback?.Invoke(humanDescription);
                }
                Close();
            }
        }

        public static void ShowWindow(GameObject rootObject, Action<HumanDescription> saveCallback, Avatar avatar = null)
        {
            var window = GetWindow<AvatarEditor>();
            window.titleContent = new GUIContent("Avatar Editor");
            window._boneRoot = rootObject;
            window._creatingAvatar = avatar;
            if (avatar != null)
            {
                window._humanDescription = avatar.humanDescription;
            }
            window._saveCallback = saveCallback;
            window.Show();
        }
    }
}