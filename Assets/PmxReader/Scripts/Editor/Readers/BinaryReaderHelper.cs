using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public static class BinaryReaderHelper
    {
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public static string ReadASCII(this BinaryReader reader, int length)
        {
            var bytes = reader.ReadBytes(length);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string ReadString(this BinaryReader reader, PmxHead head)
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            return head.Encoding.GetString(bytes);
        }

        public static Color ReadColorRGBA(this BinaryReader reader)
        {
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            var a = reader.ReadSingle();
            return new Color(r, g, b, a);
        }

        public static Color ReadColorRGB(this BinaryReader reader)
        {
            var r = reader.ReadSingle();
            var g = reader.ReadSingle();
            var b = reader.ReadSingle();
            return new Color(r, g, b);
        }
    }
}