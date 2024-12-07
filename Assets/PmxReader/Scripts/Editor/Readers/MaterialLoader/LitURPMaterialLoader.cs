using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class LitURPMaterialLoader : IMaterialLoader
    {
        public IMaterialLoader.MaterialData GetData(BinaryReader reader, PmxHead head)
        {
            return new IMaterialLoader.MaterialData(reader, head);
        }

        public Material LoadMaterial(IMaterialLoader.MaterialData materialData)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            return mat;
        }
    }
}