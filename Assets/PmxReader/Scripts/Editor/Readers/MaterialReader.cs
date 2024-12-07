using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class MaterialReader
    {
        public Material[] Materials => _materials;
        private PmxHead _head;
        private Material[] _materials;

        public MaterialReader(PmxHead head)
        {
            _head = head;
        }

        public bool ReadMaterial(BinaryReader reader)
        {
            try
            {
                var materialCount = reader.ReadInt32();
                _materials = new Material[materialCount];
                for (var i = 0; i < materialCount; ++i)
                {
                    var loader = new LitURPMaterialLoader();
                    var data = loader.GetData(reader, _head);
                    _materials[i] = loader.LoadMaterial(data);
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