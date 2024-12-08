using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class ImpulseOffset : IMorphOffset
    {
        public int RigidBodyIndex { get; set; }
        public byte LocalFlag { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Torque { get; set; }

        public void Read(BinaryReader reader, PmxHead head)
        {
            RigidBodyIndex = head.ReadRigidBodyIndex(reader);
            LocalFlag = reader.ReadByte();
            Velocity = reader.ReadVector3();
            Torque = reader.ReadVector3();
        }

        public void Apply(Mesh mesh)
        {
        }
    }
}