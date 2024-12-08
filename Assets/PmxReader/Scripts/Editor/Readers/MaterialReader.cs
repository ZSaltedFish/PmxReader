using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace ZKnight.PmxReader.Editor
{
    public class MaterialReader
    {
        public Material[] Materials => _materials;
        private PmxHead _head;
        private Material[] _materials;
        private TextureReader _textureReader;
        private MeshReader _meshReader;

        public MaterialReader(PmxHead head, TextureReader tex, MeshReader meshReader)
        {
            _head = head;
            _textureReader = tex;
            _meshReader = meshReader;
        }

        public bool ReadMaterial(BinaryReader reader)
        {
            try
            {
                var mesh = _meshReader.Mesh;
                var materialCount = reader.ReadInt32();
                mesh.subMeshCount = materialCount;
                _materials = new Material[materialCount];
                var subMeshIndex = 0;
                for (var i = 0; i < materialCount; ++i)
                {
                    var loader = new LitURPMaterialLoader();
                    var data = loader.GetData(reader, _head);
                    _materials[i] = loader.LoadMaterial(data, _textureReader);

                    var vertexCount = new int[data.VertexCount];
                    SubMeshDescriptor subMesh = new(subMeshIndex, data.VertexCount);
                    subMeshIndex += data.VertexCount;
                    mesh.SetSubMesh(i, subMesh, MeshUpdateFlags.DontRecalculateBounds);
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}