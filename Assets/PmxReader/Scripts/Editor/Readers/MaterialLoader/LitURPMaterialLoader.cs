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

        public Material LoadMaterial(IMaterialLoader.MaterialData materialData, TextureReader reader)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"))
            {
                name = materialData.LocalizedName,
                color = materialData.Diffuse,
            };
            var diffuseIndex = materialData.TextureIndex;
            if (diffuseIndex >= 0 && diffuseIndex < reader.Textures.Length)
            {
                var diffuseTex = reader.Textures[materialData.TextureIndex];
                mat.mainTexture = diffuseTex;
            }

            var toonIndex = materialData.ToonTextureIndex;
            if (toonIndex >= 0 && toonIndex < reader.Textures.Length)
            {
                var toonTex = reader.Textures[materialData.ToonTextureIndex];
                mat.SetTexture("_ShadowTex", toonTex);
            }
            mat.SetFloat("_Smoothness", 0f);
            return mat;
        }
    }
}