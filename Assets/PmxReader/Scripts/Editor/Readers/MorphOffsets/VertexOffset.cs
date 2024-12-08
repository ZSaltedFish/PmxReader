using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class VertexOffset : IMorphOffset
    {
        public int VertexIndex { get; set; }
        public Vector3 Offset { get; set; }
        public void Apply(Mesh mesh)
        {
            
        }

        public void Read(BinaryReader reader, PmxHead head)
        {
            VertexIndex = head.ReadVertexIndex(reader);
            Offset = reader.ReadVector3();
        }

        public static void GroupApply(Mesh mesh, MorphReader.MorphData data)
        {
            var offsets = data.Offsets.OfType<VertexOffset>().ToArray();
            var newVertices = new Vector3[mesh.vertexCount];
            var normals = new Vector3[mesh.vertexCount];
            for (var i = 0; i < offsets.Length; ++i)
            {
                var offset = offsets[i];
                newVertices[offset.VertexIndex] = offset.Offset;
            }
            var tangents = new Vector3[mesh.vertexCount];
            mesh.AddBlendShapeFrame(data.LocalizedName, 100f, newVertices.ToArray(), normals, tangents);
        }
    }
}