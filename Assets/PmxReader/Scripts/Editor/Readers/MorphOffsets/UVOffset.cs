using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class UVOffset : IMorphOffset
    {
        public int VertexIndex { get; set; }
        public Vector2 Offset { get; set; }

        public void Read(BinaryReader reader, PmxHead head)
        {
            VertexIndex = head.ReadVertexIndex(reader);
            Offset = reader.ReadVector2();
        }

        public void Apply(Mesh mesh)
        {
        }
    }
}