using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class BoneOffset : IMorphOffset
    {
        public int BoneIndex { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public void Read(BinaryReader reader, PmxHead head)
        {
            BoneIndex = head.ReadBoneIndex(reader);
            Position = reader.ReadVector3();
            Rotation = reader.ReadQuaternion();
        }

        public void Apply(Mesh mesh)
        {
        }

        public static void GroupApply(Mesh mesh, MorphReader.MorphData data, GameObject[] bones)
        {
            if (mesh.GetBlendShapeIndex(data.LocalizedName) != -1) return;

            var offsets = data.Offsets.OfType<BoneOffset>().ToArray();
            if (offsets.Length == 0)
            {
                Debug.LogWarning($"No bone offsets found for {data.LocalizedName}");
                return;
            }
            List<Vector3> boneOldPositions = new List<Vector3>();
            List<Quaternion> boneOldRotations = new List<Quaternion>();
            for (var i = 0; i < offsets.Length; ++i)
            {
                var offset = offsets[i];
                var bone = bones[offset.BoneIndex];
                boneOldPositions.Add(bone.transform.localPosition);
                boneOldRotations.Add(bone.transform.localRotation);
                bone.transform.localPosition += offset.Position;
                bone.transform.localRotation *= offset.Rotation;
            }

            var newVertices = new Vector3[mesh.vertexCount];
            var normals = new Vector3[mesh.vertexCount];
            var tangents = new Vector3[mesh.vertexCount];

            for (var i = 0; i < newVertices.Length; ++i)
            {
                var boneWeight = mesh.boneWeights[i];
                var newVertix = Vector3.zero;
                for (var index = 0; index < 4; ++index)
                {
                    var boneIndex = GetBoneIndex(boneWeight, index);
                    if (boneIndex == -1) continue;
                    var bone = bones[boneIndex];
                    var weight = GetBoneWeight(boneWeight, index);
                    var boneMatrix = bone.transform.localToWorldMatrix.inverse;
                    newVertix += weight * boneMatrix.MultiplyPoint3x4(mesh.vertices[i]);
                }

                newVertices[i] = newVertix - mesh.vertices[i];
                normals[i] = Vector3.zero;
                tangents[i] = Vector3.zero;
            }

            mesh.AddBlendShapeFrame(data.LocalizedName, 100f, newVertices, normals, tangents);

            for (var i = 0; i < offsets.Length; ++i)
            {
                var offset = offsets[i];
                var bone = bones[offset.BoneIndex];
                bone.transform.localPosition = boneOldPositions[i];
                bone.transform.localRotation = boneOldRotations[i];
            }
        }

        private static float GetBoneWeight(BoneWeight boneWeight, int boneIndex)
        {
            return boneIndex switch
            {
                0 => boneWeight.weight0,
                1 => boneWeight.weight1,
                2 => boneWeight.weight2,
                3 => boneWeight.weight3,
                _ => 0f
            };
        }

        private static int GetBoneIndex(BoneWeight boneWeight, int boneIndex)
        {
            return boneIndex switch
            {
                0 => boneWeight.boneIndex0,
                1 => boneWeight.boneIndex1,
                2 => boneWeight.boneIndex2,
                3 => boneWeight.boneIndex3,
                _ => 0
            };
        }
    }
}   