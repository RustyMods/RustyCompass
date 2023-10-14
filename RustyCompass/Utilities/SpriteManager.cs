using System.Reflection;
using UnityEngine;

namespace RustyCompass.Utilities;

public static class SpriteManager
{
    public static Sprite? RegisterSprite(string fileName, string folderName = "icons")
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        string path = $"{RustyCompassPlugin.ModName}.{folderName}.{fileName}";
        using (var stream = assembly.GetManifestResourceStream(path))
        {
            if (stream != null)
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(buffer))
                {
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                }
            }
        }

        return null;
    }
}