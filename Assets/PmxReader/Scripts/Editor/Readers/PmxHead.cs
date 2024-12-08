using System;
using System.IO;
using System.Text;

namespace ZKnight.PmxReader.Editor
{
    public class PmxHead
    {
        public string Magic { get; internal set; }       // "PMX " if not, invalid PMX file
        public float Version { get; internal set; }      // 2.0 or 2.1
        public byte GlobalInfoCount { get; internal set; }  // Number of global information. 8 for 2.0, 10 for 2.1
        public byte[] GlobalInfo { get; internal set; }   // Global information
        public string ModelName { get; internal set; }
        public string GlobalName { get; internal set; }
        public string GlobalModelInfo { get; internal set; }
        public string ModelInfo { get; internal set; }

        public Encoding Encoding
        {
            get
            {
                if (GlobalInfo == null) return Encoding.UTF8;
                byte encoding = GlobalInfo[0];
                if (encoding == 0) return Encoding.Unicode;        // UTF-16 LE
                return Encoding.UTF8;
            }
        }

        public int ExVect4Count => GlobalInfo[1];
        public int VectexIndexSize => GlobalInfo[2];
        public int BoneIndexSize => GlobalInfo[5];
        public int TextureIndexSize => GlobalInfo[3];
        public int MaterialIndexSize => GlobalInfo[4];
        public int MorphIndexSize => GlobalInfo[6];
        public int RigidBodyIndexSize => GlobalInfo[7];

        public int ReadVertexIndex(BinaryReader reader)
        {
            return VectexIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadUInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{VectexIndexSize}")
            };
        }

        public int ReadBoneIndex(BinaryReader reader)
        {
            return BoneIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{BoneIndexSize}")
            };
        }

        public int ReadTextureIndex(BinaryReader reader)
        {
            return TextureIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{TextureIndexSize}")
            };
        }

        public int ReadMaterialIndex(BinaryReader reader)
        {
            return MaterialIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{MaterialIndexSize}")
            };
        }

        public int ReadMorphIndex(BinaryReader reader)
        {
            return MorphIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{MorphIndexSize}")
            };
        }

        public int ReadRigidBodyIndex(BinaryReader reader)
        {
            return RigidBodyIndexSize switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                4 => reader.ReadInt32(),
                _ => throw new Exception($"Invalid VertexIndexSize:{RigidBodyIndexSize}")
            };
        }
    }
}