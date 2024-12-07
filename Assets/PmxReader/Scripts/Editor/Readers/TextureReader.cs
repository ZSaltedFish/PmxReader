using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ZKnight.PmxReader.Editor
{
    public class TextureReader
    {
        public Texture2D DefaultTexture
        {
            get
            {
                if (_defaultTexture == null)
                {
                    _defaultTexture = new Texture2D(1, 1);
                    _defaultTexture.SetPixel(0, 0, Color.white);
                    _defaultTexture.Apply();
                }
                return _defaultTexture;
            }
        }
        private string[] _texturePaths;
        private string _folderPath;
        private PmxHead _head;
        private Texture2D _defaultTexture;

        public Texture2D[] Textures { get; private set; }

        public TextureReader(string folderPath, PmxHead head)
        {
            _folderPath = folderPath;
            _head = head;
        }

        public bool ReadTextures(BinaryReader reader)
        {
            var textureCount = reader.ReadInt32();
            _texturePaths = new string[textureCount];
            Textures = new Texture2D[textureCount];
            try
            {
                for (var i = 0; i < textureCount; ++i)
                {
                    var texturePath = reader.ReadString(_head);
                    var path = Path.Combine(_folderPath, texturePath).Replace('\\', '/');
                    _texturePaths[i] = path;
                    if (!File.Exists(path))
                    {
                        Textures[i] = DefaultTexture;
                        Debug.LogWarning($"Texture not found: {path}");
                        continue;
                    }

                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (texture == null)
                    {
                        Textures[i] = DefaultTexture;
                        Debug.LogWarning($"Failed to load texture: {path}");
                        continue;
                    }

                    Textures[i] = texture;
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