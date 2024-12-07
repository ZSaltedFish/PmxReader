using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public interface IMaterialLoader
    {
        public class MaterialData
        {
            public string LocalizedName { get; set; }
            public string GlobalizedName { get; set; }
            public Color Diffuse { get; set; }          // Diffuse color
            public Color Specular { get; set; }         // Specular color
            public float SpecularPower { get; set; }    // Specular power
            public Color Ambient { get; set; }          // Ambient color
            public byte Flag { get; set; }              // Bit flag
            public Color EdgeColor { get; set; }        // Edge color
            public float EdgeSize { get; set; }         // Edge size
            public int TextureIndex { get; set; }       // Texture index
            public int SphereTextureIndex { get; set; } // Sphere texture index
            public byte SphereMode { get; set; }        // Sphere mode
            public byte ToonFlag { get; set; }          // Toon flag
            public int ToonTextureIndex { get; set; }   // Toon texture index
            public string Memo { get; set; }            // Memo
            public int VertexCount { get; set; }        // Vertex count

            public MaterialData(BinaryReader reader, PmxHead head)
            {
                LocalizedName = reader.ReadString(head);
                GlobalizedName = reader.ReadString(head);
                Diffuse = reader.ReadColorRGBA();
                Specular = reader.ReadColorRGB();
                SpecularPower = reader.ReadSingle();
                Ambient = reader.ReadColorRGB();
                Flag = reader.ReadByte();
                EdgeColor = reader.ReadColorRGBA();
                EdgeSize = reader.ReadSingle();
                TextureIndex = head.ReadTextureIndex(reader);
                SphereTextureIndex = head.ReadTextureIndex(reader);
                SphereMode = reader.ReadByte();
                ToonFlag = reader.ReadByte();
                ToonTextureIndex = head.ReadTextureIndex(reader);
                Memo = reader.ReadString(head);
                VertexCount = reader.ReadInt32();
            }
        }
        MaterialData GetData(BinaryReader reader, PmxHead head);
        Material LoadMaterial(MaterialData materialData, TextureReader tex);
    }
}