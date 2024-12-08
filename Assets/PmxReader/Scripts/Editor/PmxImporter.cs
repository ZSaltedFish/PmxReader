using UnityEditor.AssetImporters;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    [ScriptedImporter(1, "pmx")]
    public class PmxImporter : ScriptedImporter
    {
        public float Factor = 0.1f;
        public Transform[] ModelTransforms;
        public Avatar ModeAvatar;
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var modelReader = new ModelReader(Factor);
            modelReader.Load(ctx.assetPath);
            ctx.AddObjectToAsset("mesh", modelReader.Mesh);

            var materials = modelReader.Materials;
            for (var i = 0; i < materials.Length; ++i)
            {
                ctx.AddObjectToAsset($"material{i}", materials[i]);
            }

            var root = modelReader.RootObject;
            if (root.TryGetComponent<Animator>(out var anim))
            {
                var humanoidCheck = new HumanoidCheck(modelReader.Bones, anim);
                if (humanoidCheck.TryCreateHumanoid(out var avatar, modelReader.BoneRoot.transform))
                {
                    avatar.name = $"{modelReader.Name}_Avatar";
                    ctx.AddObjectToAsset("avatar", avatar);
                    anim.avatar = avatar;
                    ModeAvatar = anim.avatar;
                }
            }
            ModelTransforms = root.GetComponentsInChildren<Transform>();
            ctx.AddObjectToAsset("root", root);
            ctx.SetMainObject(root);
        }
    }
}