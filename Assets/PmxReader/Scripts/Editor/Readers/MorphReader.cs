using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class MorphReader
    {
        public class MorphData
        {
            public string LocalizedName { get; set; }
            public string EnglishName { get; set; }
            public byte Panel { get; set; }
            public byte MorphType { get; set; }
            public int OffsetCount { get; set; }
            public IMorphOffset[] Offsets { get; set; }
        }

        private PmxHead _head;
        public MorphReader(PmxHead head)
        {
            _head = head;
        }

        public bool ReadMorph(BinaryReader reader, Mesh mesh, GameObject[] bones)
        {
            try
            {
                var count = reader.ReadInt32();
                for (var i = 0; i < count; ++i)
                {
                    var morph = new MorphData
                    {
                        LocalizedName = reader.ReadString(_head),
                        EnglishName = reader.ReadString(_head),
                        Panel = reader.ReadByte(),
                        MorphType = reader.ReadByte(),
                        OffsetCount = reader.ReadInt32(),
                    };
                    morph.Offsets = new IMorphOffset[morph.OffsetCount];
                    for (var j = 0; j < morph.OffsetCount; ++j)
                    {
                        var typeId = morph.MorphType;
                        var offset = IMorphOffset.GetMorphOffset(typeId);
                        offset.Read(reader, _head);
                        morph.Offsets[j] = offset;
                    }
                    if (mesh.GetBlendShapeIndex(morph.LocalizedName) != -1) continue;
                    IMorphOffset.GroupApply(mesh, morph, bones);
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}