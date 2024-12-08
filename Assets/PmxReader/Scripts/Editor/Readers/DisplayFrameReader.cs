using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class DisplayFrameReader
    {
        public class DisplayFrameElement
        {
            public int Index { get; set; }
            public byte FrameType { get; set; }
        }
        public class DisplayFrameData
        {
            public string LocalizedName { get; set; }
            public string EnglishName { get; set; }
            public byte SpecialFrameFlag { get; set; }
            public int ElementCount { get; set; }
            public DisplayFrameElement[] Elements { get; set; }
        }

        private PmxHead _head;
        public DisplayFrameReader(PmxHead head)
        {
            _head = head;
        }

        public bool ReadDisplayFrame(BinaryReader reader)
        {
            try
            {
                var count = reader.ReadInt32();
                for (var i = 0; i < count; ++i)
                {
                    var displayFrame = new DisplayFrameData
                    {
                        LocalizedName = reader.ReadString(_head),
                        EnglishName = reader.ReadString(_head),
                        SpecialFrameFlag = reader.ReadByte(),
                        ElementCount = reader.ReadInt32(),
                    };
                    displayFrame.Elements = new DisplayFrameElement[displayFrame.ElementCount];
                    for (var j = 0; j < displayFrame.ElementCount; ++j)
                    {
                        var element = new DisplayFrameElement
                        {
                            FrameType = reader.ReadByte(),
                        };
                        if (element.FrameType == 0)
                        {
                            element.Index = _head.ReadBoneIndex(reader);
                        }
                        else if (element.FrameType == 1)
                        {
                            element.Index = _head.ReadMorphIndex(reader);
                        }
                        displayFrame.Elements[j] = element;
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
                return false;
            }
        }
    }
}