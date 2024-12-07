using UnityEditor.AssetImporters;
using UnityEngine;

namespace ZKnight.PmxReader.Editor
{
    [ScriptedImporter(1, "pmx")]
    public class PmxImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var modelReader = new ModelReader();
            modelReader.Load(ctx.assetPath);
            ctx.AddObjectToAsset("mesh", modelReader.Mesh);

            var game = new GameObject(modelReader.Name);
            game.AddComponent<MeshFilter>().sharedMesh = modelReader.Mesh;
            game.AddComponent<MeshRenderer>();
            ctx.AddObjectToAsset("game", game);
            ctx.SetMainObject(game);
        }
    }
}