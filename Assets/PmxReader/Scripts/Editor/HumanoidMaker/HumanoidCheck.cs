using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class HumanoidCheck
    {
        private Avatar _avatar;
        private Dictionary<string, Transform> _bones;
        private Animator _animator;

        public HumanoidCheck(GameObject[] gameObjects, Animator animator)
        {
            _bones = new Dictionary<string, Transform>();
            foreach (var go in gameObjects)
            {
                var name = go.name;
                if (_bones.ContainsKey(name))
                {
                    throw new Exception($"Duplicate bone name: {name}");
                }

                _bones.Add(name, go.transform);
            }
            _animator = animator;
        }

        public bool TryCreateHumanoid(out Avatar avatar, Transform armBone)
        {
            try
            {
                var skeletonList = new List<SkeletonBone>();
                var humanoids = new List<HumanBone>();
                
                foreach (var humanBone in Enum.GetValues(typeof(HumanBodyBones)))
                {
                    var humanBoneName = HumanBoneName2PmxBoneName((HumanBodyBones) humanBone);
                    if (string.IsNullOrEmpty(humanBoneName))
                    {
                        continue;
                    }

                    if (!_bones.TryGetValue(humanBoneName, out var bone))
                    {
                        Debug.Log($"Bone not found: {humanBoneName}");
                        continue;
                    }
                    var Hbone = new HumanBone
                    {
                        boneName = humanBoneName,
                        humanName = humanBone.ToString(),
                        limit = new HumanLimit
                        {
                            useDefaultValues = true
                        }
                    };

                    humanoids.Add(Hbone);
                }

                var parent = armBone;
                var parentBone = new SkeletonBone
                {
                    name = parent.name,
                    position = parent.localPosition,
                    rotation = parent.localRotation,
                    scale = parent.localScale
                };
                skeletonList.Add(parentBone);
                foreach (var bone in _bones)
                {
                    var skeletonBone = new SkeletonBone
                    {
                        name = bone.Key,
                        position = bone.Value.localPosition,
                        rotation = bone.Value.localRotation,
                        scale = bone.Value.localScale
                    };
                    skeletonList.Add(skeletonBone);
                }
                var humanDescription = new HumanDescription
                {
                    human = humanoids.ToArray(),
                    skeleton = skeletonList.ToArray()
                };
                _avatar = AvatarBuilder.BuildHumanAvatar(_animator.gameObject, humanDescription);
                _animator.avatar = _avatar;
                avatar = _avatar;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                avatar = null;
                return false;
            }
        }

        private static string HumanBoneName2PmxBoneName(HumanBodyBones humanBone)
        {
            return humanBone switch
            {
                HumanBodyBones.Hips => "センター",
                HumanBodyBones.Spine => "上半身",
                HumanBodyBones.Chest => "上半身2",
                HumanBodyBones.UpperChest => "首",
                HumanBodyBones.Neck => "",
                HumanBodyBones.Head => "頭",
                HumanBodyBones.LeftShoulder => "左肩",
                HumanBodyBones.RightShoulder => "右肩",
                HumanBodyBones.LeftUpperArm => "左腕",
                HumanBodyBones.RightUpperArm => "右腕",
                HumanBodyBones.LeftLowerArm => "左ひじ",
                HumanBodyBones.RightLowerArm => "右ひじ",
                HumanBodyBones.LeftHand => "左手首",
                HumanBodyBones.RightHand => "右手首",
                HumanBodyBones.LeftIndexProximal => "左親指１",
                HumanBodyBones.RightIndexProximal => "右親指１",
                HumanBodyBones.LeftIndexIntermediate => "左親指２",
                HumanBodyBones.RightIndexIntermediate => "右親指２",
                HumanBodyBones.LeftIndexDistal => "左親指３",
                HumanBodyBones.RightIndexDistal => "右親指３",
                HumanBodyBones.LeftThumbProximal => "左人指１",
                HumanBodyBones.RightThumbProximal => "右人指１",
                HumanBodyBones.LeftThumbIntermediate => "左人指２",
                HumanBodyBones.RightThumbIntermediate => "右人指２",
                HumanBodyBones.LeftThumbDistal => "左人指３",
                HumanBodyBones.RightThumbDistal => "右人指３",
                HumanBodyBones.LeftMiddleProximal => "左中指１",
                HumanBodyBones.RightMiddleProximal => "右中指１",
                HumanBodyBones.LeftMiddleIntermediate => "左中指２",
                HumanBodyBones.RightMiddleIntermediate => "右中指２",
                HumanBodyBones.LeftMiddleDistal => "左中指３",
                HumanBodyBones.RightMiddleDistal => "右中指３",
                HumanBodyBones.LeftRingProximal => "左薬指１",
                HumanBodyBones.RightRingProximal => "右薬指１",
                HumanBodyBones.LeftRingIntermediate => "左薬指２",
                HumanBodyBones.RightRingIntermediate => "右薬指２",
                HumanBodyBones.LeftRingDistal => "左薬指３",
                HumanBodyBones.RightRingDistal => "右薬指３",
                HumanBodyBones.LeftLittleProximal => "左小指１",
                HumanBodyBones.RightLittleProximal => "右小指１",
                HumanBodyBones.LeftLittleIntermediate => "左小指２",
                HumanBodyBones.RightLittleIntermediate => "右小指２",
                HumanBodyBones.LeftLittleDistal => "左小指３",
                HumanBodyBones.RightLittleDistal => "右小指３",
                HumanBodyBones.LeftUpperLeg => "左足",
                HumanBodyBones.RightUpperLeg => "右足",
                HumanBodyBones.LeftLowerLeg => "左ひざ",
                HumanBodyBones.RightLowerLeg => "右ひざ",
                HumanBodyBones.LeftFoot => "左足首",
                HumanBodyBones.RightFoot => "右足首",
                HumanBodyBones.LeftToes => "左つま先",
                HumanBodyBones.RightToes => "右つま先",
                HumanBodyBones.LeftEye => "左目",
                HumanBodyBones.RightEye => "右目",
                HumanBodyBones.Jaw => "あご",
                HumanBodyBones.LastBone => "",
                _ => ""
            };
        }
    }
}
