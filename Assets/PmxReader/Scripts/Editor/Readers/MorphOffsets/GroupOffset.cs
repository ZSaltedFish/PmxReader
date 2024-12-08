using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class GroupOffset : IMorphOffset
    {
        public int MorphIndex { get; set; }
        public float MorphValue { get; set; }

        public void Read(BinaryReader reader, PmxHead head)
        {
            MorphIndex = head.ReadMorphIndex(reader);
            MorphValue = reader.ReadSingle();
        }

        public void Apply(Mesh mesh)
        {
        }
    }
}