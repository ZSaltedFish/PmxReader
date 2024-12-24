using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    public class NodeReader
    {
        public class IndexOrVector3
        {
            public int Index { get; set; } = -1;
            public Vector3 Position { get; set; }

            public void Read(BinaryReader reader, PmxHead head, bool isIndex)
            {
                if (isIndex)
                {
                    Index = head.ReadBoneIndex(reader);
                }
                else
                {
                    Position = reader.ReadVector3();
                }
            }
        }

        public class IKLink
        {
            public int BoneIndex { get; set; }
            public bool IsLimited { get; set; }
            public Vector3 MinAngle { get; set; }
            public Vector3 MaxAngle { get; set; }
        }

        public class Node
        {
            public string LocalName { get; set; }
            public string GlobalName { get; set; }
            public Vector3 Position { get; set; }
            public int Parent { get; set; }
            public int TransformHierarchy { get; set; }
            public byte[] BoneFlag { get; set; }

            public IndexOrVector3 TailBoneType { get; set; } = new IndexOrVector3();

            #region Inherit
            public int InheritBoneIndex { get; set; }
            public float InheritRate { get; set; }
            #endregion

            #region FixedAxis
            public Vector3 FixedAxis { get; set; }
            #endregion

            #region LocalAxis
            public Vector3 LocalAxisX { get; set; }
            public Vector3 LocalAxisZ { get; set; }
            #endregion

            #region ExternalParentTransform
            public int ExternalParentTransform { get; set; }
            #endregion

            #region IK
            public int IKTarget { get; set; }
            public int IKLoop { get; set; }
            public float IKRotateLimit { get; set; }
            public int IKLinkCount { get; set; }
            public IKLink[] IKLinks { get; set; }
            #endregion

            /// <summary>
            /// false为连接相对位置，true为连接子骨骼
            /// </summary>
            /// <returns></returns>
            public bool BoneTailPosition()
            {
                return (BoneFlag[0] & 0x1) != 0;
            }

            public bool Rotateable()
            {
                return (BoneFlag[0] & 0x2) != 0;
            }

            public bool Moveable()
            {
                return (BoneFlag[0] & 0x4) != 0;
            }

            public bool Visible()
            {
                return (BoneFlag[0] & 0x8) != 0;
            }

            public bool Enabled()
            {
                return (BoneFlag[0] & 0x10) != 0;
            }

            public bool IsIK()
            {
                return (BoneFlag[0] & 0x20) != 0;
            }

            public bool IsInheritRotation()
            {
                return (BoneFlag[1] & 0x1) != 0;
            }

            public bool IsInheritMove()
            {
                return (BoneFlag[1] & 0x2) != 0;
            }

            public bool IsFixedAxis()
            {
                return (BoneFlag[1] & 0x4) != 0;
            }

            public bool IsLocalAxis()
            {
                return (BoneFlag[1] & 0x8) != 0;
            }

            public bool IsTransformAfterPhysics()
            {
                return (BoneFlag[1] & 0x10) != 0;
            }

            public bool IsExternalParentTransform()
            {
                return (BoneFlag[1] & 0x20) != 0;
            }
        }

        public GameObject Root => _root;
        public Node[] Nodes => _nodes;
        public GameObject[] Bones => _gameObjects;
        public GameObject BoneRoot => _nodeRoot;

        private readonly PmxHead _head;
        private GameObject _root, _nodeRoot, _renderRoot;
        private Node[] _nodes;
        private GameObject[] _gameObjects;
        private readonly float _factor;

        public NodeReader(PmxHead head, float factor)
        {
            _head = head;
            _factor = factor;
        }

        public bool ReadNode(BinaryReader reader)
        {
            try
            {
                var nodeCount = reader.ReadInt32();
                _nodes = new Node[nodeCount];
                for (var i = 0; i < nodeCount; ++i)
                {
                    var node = new Node();
                    _nodes[i] = node;
                    node.LocalName = reader.ReadString(_head);
                    node.GlobalName = reader.ReadString(_head);
                    node.Position = reader.ReadVector3() * _factor;
                    node.Parent = _head.ReadBoneIndex(reader);
                    node.TransformHierarchy = reader.ReadInt32();
                    node.BoneFlag = reader.ReadBytes(2);

                    node.TailBoneType.Read(reader, _head, node.BoneTailPosition());

                    if (node.IsInheritMove() || node.IsInheritRotation())
                    {
                        node.InheritBoneIndex = _head.ReadBoneIndex(reader);
                        node.InheritRate = reader.ReadSingle();
                    }

                    if (node.IsFixedAxis())
                    {
                        node.FixedAxis = reader.ReadVector3();
                    }

                    if (node.IsLocalAxis())
                    {
                        node.LocalAxisX = reader.ReadVector3();
                        node.LocalAxisZ = reader.ReadVector3();
                    }

                    if (node.IsExternalParentTransform())
                    {
                        node.ExternalParentTransform = _head.ReadBoneIndex(reader);
                    }

                    if (node.IsIK())
                    {
                        node.IKTarget = _head.ReadBoneIndex(reader);
                        node.IKLoop = reader.ReadInt32();
                        node.IKRotateLimit = reader.ReadSingle();
                        node.IKLinkCount = reader.ReadInt32();
                        node.IKLinks = new IKLink[node.IKLinkCount];
                        for (var j = 0; j < node.IKLinkCount; ++j)
                        {
                            var link = new IKLink();
                            node.IKLinks[j] = link;
                            link.BoneIndex = _head.ReadBoneIndex(reader);
                            link.IsLimited = reader.ReadBoolean();
                            if (link.IsLimited)
                            {
                                link.MinAngle = reader.ReadVector3();
                                link.MaxAngle = reader.ReadVector3();
                            }
                        }
                    }
                }

                CreateGameObject();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        private void CreateGameObject()
        {
            var gameArray = new GameObject[_nodes.Length];
            _root = new GameObject(_head.ModelName);
            _ = _root.AddComponent<Animator>();
            _nodeRoot = new GameObject($"{_head.ModelName}_arm");
            _renderRoot = new GameObject($"{_head.ModelName}_mesh");

            _nodeRoot.transform.SetParent(_root.transform);
            _renderRoot.transform.SetParent(_root.transform);
            _root.transform.localPosition = Vector3.zero;
            _root.transform.localRotation = Quaternion.identity;
            _root.transform.localScale = Vector3.one;

            _gameObjects = gameArray;
            for (var i = 0; i < _nodes.Length; ++i)
            {
                var node = _nodes[i];
                var go = new GameObject(node.LocalName);
                gameArray[i] = go;
                go.transform.localPosition = node.Position;
                go.hideFlags = node.Visible() ? HideFlags.None : HideFlags.HideInHierarchy;
            }

            for (int  i = 0;  i < _nodes.Length; ++i)
            {
                var node = _nodes[i];
                var go = gameArray[i];

                if (node.Parent >= 0)
                {
                    go.transform.SetParent(gameArray[node.Parent].transform);
                }
                else
                {
                    go.transform.SetParent(_nodeRoot.transform);
                }
            }
            _nodeRoot.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        public bool SetRenderer(Mesh mesh, Material[] materials)
        {
            try
            {
                var renderer = _renderRoot.AddComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = mesh;
                renderer.sharedMaterials = materials;

                renderer.rootBone = _nodeRoot.transform;

                var weights = mesh.boneWeights;
                var parentBone = _nodeRoot.transform;
                var bindPoses = new List<Matrix4x4>();
                var bones = new List<Transform>();
                foreach (var go in _gameObjects)
                {
                    var bone = go.transform;
                    var binePose = bone.worldToLocalMatrix * parentBone.localToWorldMatrix;
                    bindPoses.Add(binePose);
                    bones.Add(bone);
                }
                mesh.bindposes = bindPoses.ToArray();
                renderer.bones = bones.ToArray();

                renderer.ResetBounds();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
    }
}
