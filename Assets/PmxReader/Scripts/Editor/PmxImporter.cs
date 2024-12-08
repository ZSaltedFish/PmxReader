using UnityEditor.AssetImporters;

namespace ZKnight.PmxReader.Editor
{
    [ScriptedImporter(1, "pmx")]
    public class PmxImporter : ScriptedImporter
    {
        public float Factor = 0.1f;
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
            ctx.AddObjectToAsset("root", root);
            ctx.SetMainObject(root);
        }
    }
}