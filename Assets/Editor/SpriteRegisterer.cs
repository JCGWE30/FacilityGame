using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class SpriteRegisterer : AssetPostprocessor
{
    private static readonly string spritesPath = "Assets/Sprites";
    private static readonly string spriteListPath = "Assets/Sprites/SpriteHolder.asset";
    private static readonly string spriteEnumPath = "Assets/Sprites/SpriteEnum.cs";

    private static bool updating;
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (updating) return;
        foreach (string asset in importedAssets.Concat(deletedAssets).Concat(movedAssets))
        {
            if (ShouldUpdate(asset))
            {
                Update();
                return;
            }
        }
    }

    private static bool ShouldUpdate(string str)
    {
        return str.StartsWith(spritesPath) && str.EndsWith(".png");
    }
    private static void Update()
    {
        updating = true;
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { spritesPath });
        List<Sprite> sprites = new List<Sprite>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite != null)
            {
                sprites.Add(sprite);
            }
        }

        GenerateScript(sprites.ToArray());

        SpriteHolder holder = AssetDatabase.LoadAssetAtPath<SpriteHolder>(spriteListPath);

        if (holder == null)
        {
            holder = ScriptableObject.CreateInstance<SpriteHolder>();
            AssetDatabase.CreateAsset(holder, spriteListPath);
        }

        SpriteFinder finder = Object.FindObjectOfType<SpriteFinder>();
        if (finder != null)
            finder.spriteHolder = holder;

        holder.sprites = sprites.ToArray();
        EditorUtility.SetDirty(holder);
        AssetDatabase.SaveAssets();
        updating = false;
    }

    private static void GenerateScript(Sprite[] sprites)
    {
        int count = 0;
        using(StreamWriter writer = new StreamWriter(spriteEnumPath))
        {
            writer.WriteLine("public enum SpriteEnum");
            writer.WriteLine("{");
            foreach(Sprite sprite in sprites)
            {
                writer.WriteLine("    " + sprite.name+(count>=sprites.Length-1 ? "" : ","));
                count++;
            }
            writer.WriteLine("}");
        }
        AssetDatabase.Refresh();
    }
}
