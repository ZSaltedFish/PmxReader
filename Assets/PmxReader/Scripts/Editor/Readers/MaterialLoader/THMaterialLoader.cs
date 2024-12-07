using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class THMaterialLoader : IMaterialLoader
    {
        public IMaterialLoader.MaterialData GetData(BinaryReader reader, PmxHead head)
        {
            return new IMaterialLoader.MaterialData(reader, head);
        }

        public Material LoadMaterial(IMaterialLoader.MaterialData materialData, TextureReader reader)
        {
            var mat = new Material(Shader.Find("THRenderer/THLight"))
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

            var edgeSize = materialData.EdgeSize;
            mat.SetFloat("_CartoonFresnel", edgeSize);

            mat.DisableKeyword("_NORMAL_TEX_ENABLE");
            mat.DisableKeyword("_PBR_TEXTURE_USED");
            mat.DisableKeyword("_CLIPPING");
            mat.EnableKeyword("_FRESNEL_ENABLE");

            mat.SetFloat("_OutlineSize", 0.5f);
            mat.SetFloat("_AUTO_SCREEN_SIZE_OUTLINE", 1f);
            mat.SetFloat("_AUTO_SCREEN_SIZE_OUTLINE_WIDTH", 0.5f);
            mat.SetFloat("_FRESNEL_ENABLE", 0f);
            mat.SetFloat("_Smoothness", 0f);
            mat.SetFloat("_CullMode", (float)UnityEngine.Rendering.CullMode.Back);
            mat.renderQueue = 3000;

            return mat;
        }
    }
}
