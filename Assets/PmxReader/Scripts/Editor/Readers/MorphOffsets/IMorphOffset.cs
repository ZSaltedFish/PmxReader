using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public interface IMorphOffset
    {
        void Read(BinaryReader reader, PmxHead head);
        void Apply(Mesh mesh);
        public static IMorphOffset GetMorphOffset(int type)
        {
            return type switch
            {
                0 => new GroupOffset(),
                1 => new VertexOffset(),
                2 => new BoneOffset(),
                3 => new UVOffset(),
                4 => new UVOffset(),
                5 => new UVOffset(),
                6 => new UVOffset(),
                7 => new UVOffset(),
                8 => new MaterialOffset(),
                9 => new FlipOffset(),
                10 => new ImpulseOffset(),
                _ => throw new System.Exception($"Unsupported morph offset type: {type}")
            };
        }

        public static void GroupApply(Mesh mesh, MorphReader.MorphData data, GameObject[] bones)
        {
            switch (data.MorphType)
            {
                case 1:
                    VertexOffset.GroupApply(mesh, data);
                    break;
                case 2:
                    BoneOffset.GroupApply(mesh, data, bones);
                    break;
                default:
                    Debug.LogWarning($"Unsupported morph type: {data.MorphType}");
                    break;
            }
        }
    }
}