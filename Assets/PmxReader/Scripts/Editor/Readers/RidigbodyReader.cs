using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class RigidbodyReader
    {

        public class RigidbodyData
        {
            public string Name { get; set; }
            public string EnglishName { get; set; }
            public int BoneIndex { get; set; }
            public byte Group { get; set; }
            public ushort Mask { get; set; }
            public byte Shape { get; set; }
            public Vector3 Size { get; set; }
            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public float Mass { get; set; }
            public float LinearDamping { get; set; }
            public float AngularDamping { get; set; }
            public float Restitution { get; set; }
            public float Friction { get; set; }
            public byte Type { get; set; }
        }
        private PmxHead _head;
        private RigidbodyData[] _rigidbodies;
        public RigidbodyReader(PmxHead head)
        {
            _head = head;
        }

        public bool ReaderRidigbody(BinaryReader reader, GameObject[] bones)
        {
            try
            {
                var count = reader.ReadInt32();
                _rigidbodies = new RigidbodyData[count];
                for (var i = 0; i < count; ++i)
                {
                    var rigidbody = new RigidbodyData
                    {
                        Name = reader.ReadString(_head),
                        EnglishName = reader.ReadString(_head),
                        BoneIndex = _head.ReadBoneIndex(reader),
                        Group = reader.ReadByte(),
                        Mask = reader.ReadUInt16(),
                        Shape = reader.ReadByte(),
                        Size = reader.ReadVector3(),
                        Position = reader.ReadVector3(),
                        Rotation = reader.ReadVector3(),
                        Mass = reader.ReadSingle(),
                        LinearDamping = reader.ReadSingle(),
                        AngularDamping = reader.ReadSingle(),
                        Restitution = reader.ReadSingle(),
                        Friction = reader.ReadSingle(),
                        Type = reader.ReadByte(),
                    };

                    _rigidbodies[i] = rigidbody;
                    Debug.Log($"Rigidbody: {rigidbody.Name}");
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