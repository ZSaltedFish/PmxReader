using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class MeshReader
    {
        public Mesh Mesh => _mesh;
        private PmxHead _head;
        private Mesh _mesh;
        
        public MeshReader(PmxHead head)
        {
            _head = head;
        }

        public bool ReadMesh(BinaryReader reader)
        {
            try
            {
                int vertexCount = reader.ReadInt32();
                var vertices = new Vector3[vertexCount];
                var normals = new Vector3[vertexCount];
                var uvs = new Vector2[vertexCount];
                var exVect4 = new Vector4[_head.ExVect4Count * vertexCount];
                var boneWeights = new BoneWeight[vertexCount];

                for (var i = 0; i < vertexCount; ++i)
                {
                    vertices[i] = reader.ReadVector3();
                    normals[i] = reader.ReadVector3();
                    uvs[i] = reader.ReadVector2();
                    for (var j = 0; j < _head.ExVect4Count; ++j)
                    {
                        exVect4[i * _head.ExVect4Count + j] = reader.ReadVector4();
                    }

                    var boneWeightType = reader.ReadByte();
                    var boneWeight = boneWeightType switch
                    {
                        0 => BoneWeightBDEF1(reader),
                        1 => BoneWeightBDEF2(reader),
                        2 => BoneWeightBDEF4(reader),
                        3 => BoneWeightSDEF(reader),
                        4 => BoneWeightBDEF4(reader),
                        _ => throw new System.Exception($"Unsupport BoneWeight:{boneWeightType}"),
                    };
                    boneWeights[i] = boneWeight;
                    var edgeMag = reader.ReadSingle(); // Edge Magnitude (Not used)
                    if (edgeMag < 0.0f || edgeMag > 1.0f)
                    {
                        throw new System.Exception("Invalid edge magnitude.");
                    }
                }

                var traingles = TriangleReader(reader);
                _mesh = new Mesh
                {
                    name = _head.ModelName,
                    vertices = vertices,
                    normals = normals,
                    uv = uvs,
                    triangles = traingles
                };

                _mesh.RecalculateBounds();
                _mesh.boneWeights = boneWeights;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        private int[] TriangleReader(BinaryReader reader)
        {
            var triangleCount = reader.ReadInt32();
            var triangles = new int[triangleCount];
            for (var i = 0; i < triangleCount; ++i)
            {
                triangles[i] = _head.ReadVertexIndex(reader);
            }
            return triangles;
        }

        private BoneWeight BoneWeightBDEF1(BinaryReader reader)
        {
            var boneIndex = _head.ReadBoneIndex(reader);
            var boneWeight = 1.0f;
            var boneInfo = new BoneWeight
            {
                boneIndex0 = boneIndex,
                weight0 = boneWeight
            };
            return boneInfo;
        }

        private BoneWeight BoneWeightBDEF2(BinaryReader reader)
        {
            var boneIndex0 = _head.ReadBoneIndex(reader);
            var boneIndex1 = _head.ReadBoneIndex(reader);
            var boneWeight = reader.ReadSingle();
            var boneInfo = new BoneWeight
            {
                boneIndex0 = boneIndex0,
                boneIndex1 = boneIndex1,
                weight0 = boneWeight,
                weight1 = 1.0f - boneWeight
            };
            return boneInfo;
        }

        /// <summary>
        /// Use for BDEF4 and QDEF
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private BoneWeight BoneWeightBDEF4(BinaryReader reader)
        {
            var boneIndex0 = _head.ReadBoneIndex(reader);
            var boneIndex1 = _head.ReadBoneIndex(reader);
            var boneIndex2 = _head.ReadBoneIndex(reader);
            var boneIndex3 = _head.ReadBoneIndex(reader);
            var boneWeight0 = reader.ReadSingle();
            var boneWeight1 = reader.ReadSingle();
            var boneWeight2 = reader.ReadSingle();
            var boneWeight3 = reader.ReadSingle();
            var boneInfo = new BoneWeight
            {
                boneIndex0 = boneIndex0,
                boneIndex1 = boneIndex1,
                boneIndex2 = boneIndex2,
                boneIndex3 = boneIndex3,
                weight0 = boneWeight0,
                weight1 = boneWeight1,
                weight2 = boneWeight2,
                weight3 = boneWeight3
            };
            return boneInfo;
        }

        private BoneWeight BoneWeightSDEF(BinaryReader reader)
        {
            var boneIndex0 = _head.ReadBoneIndex(reader);
            var boneIndex1 = _head.ReadBoneIndex(reader);
            var boneWeight = reader.ReadSingle();
            var boneC = reader.ReadVector3();
            var boneR0 = reader.ReadVector3();
            var boneR1 = reader.ReadVector3();
            var boneInfo = new BoneWeight
            {
                boneIndex0 = boneIndex0,
                boneIndex1 = boneIndex1,
                weight0 = boneWeight,
                weight1 = 1.0f - boneWeight,
            };
            return boneInfo;
        }
    }
}