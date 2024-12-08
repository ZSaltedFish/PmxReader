using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class MaterialOffset : IMorphOffset
    {
        public int MaterialIndex { get; set; }
        public byte OffsetType { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public float SpecularPower { get; set; }
        public Color Ambient { get; set; }
        public Color EdgeColor { get; set; }
        public float EdgeSize { get; set; }
        public Vector4 Texture { get; set; }
        public Vector4 SphereTexture { get; set; }
        public byte SphereMode { get; set; }

        public void Read(BinaryReader reader, PmxHead head)
        {
            MaterialIndex = head.ReadMaterialIndex(reader);
            OffsetType = reader.ReadByte();
            Diffuse = reader.ReadColorRGBA();
            Specular = reader.ReadColorRGB();
            SpecularPower = reader.ReadSingle();
            Ambient = reader.ReadColorRGB();
            EdgeColor = reader.ReadColorRGBA();
            EdgeSize = reader.ReadSingle();
            Texture = reader.ReadVector4();
            SphereTexture = reader.ReadVector4();
            SphereMode = reader.ReadByte();
        }

        public void Apply(Mesh mesh)
        {
        }
    }
}