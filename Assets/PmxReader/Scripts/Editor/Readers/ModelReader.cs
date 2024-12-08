using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class ModelReader
    {
        public string Name => _head.ModelName;
        public Mesh Mesh => _meshReader.Mesh;
        public Material[] Materials => _materialReader.Materials;
        public GameObject RootObject => _nodeReader.Root;

        private PmxHead _head;
        private MeshReader _meshReader;
        private TextureReader _textureReader;
        private MaterialReader _materialReader;
        private NodeReader _nodeReader;
        private float _factor;

        public ModelReader(float factor)
        {
            _factor = factor;
        }

        public bool Load(string path)
        {
            using var reader = new BinaryReader(File.OpenRead(path));
            if (!TryReadHead(reader)) return false;

            _meshReader = new MeshReader(_head);
            if (!_meshReader.ReadMesh(reader)) return false;

            _textureReader = new TextureReader(Path.GetDirectoryName(path), _head);
            if (!_textureReader.ReadTextures(reader)) return false;

            _materialReader = new MaterialReader(_head, _textureReader, _meshReader);
            if (!_materialReader.ReadMaterial(reader)) return false;

            _nodeReader = new NodeReader(_head, _factor);
            if (!_nodeReader.ReadNode(reader)) return false;
            if (!_nodeReader.SetRenderer(Mesh, Materials)) return false;

            return true;
        }

        private bool TryReadHead(BinaryReader reader)
        {
            _head = new PmxHead
            {
                Magic = reader.ReadASCII(4)
            };
            if (_head.Magic != "PMX ")
            {
                Debug.LogError("Invalid PMX file.");
                return false;
            }
            _head.Version = reader.ReadSingle();
            if (_head.Version != 2.0f && _head.Version != 2.1f)
            {
                Debug.LogError("Invalid PMX version.");
                return false;
            }
            _head.GlobalInfoCount = reader.ReadByte();
            _head.GlobalInfo = new byte[_head.GlobalInfoCount];
            for (var i = 0; i < _head.GlobalInfoCount; ++i)
            {
                _head.GlobalInfo[i] = reader.ReadByte();
            }

            _head.ModelName = reader.ReadString(_head);
            _head.GlobalName = reader.ReadString(_head);
            _head.ModelInfo = reader.ReadString(_head);
            _head.GlobalModelInfo = reader.ReadString(_head);

            return true;
        }
    }
}
