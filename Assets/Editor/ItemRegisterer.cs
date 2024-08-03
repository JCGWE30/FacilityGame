using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using PlasticGui.WorkspaceWindow.Diff;

public class ItemRegisterer : AssetPostprocessor
{
    private static readonly string itemPath = "Assets/Items";
    private static readonly string itemListPath = "Assets/Items/ItemHolder.asset";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool shouldUpdate = false;

        foreach(string asset in importedAssets)
        {
            if(asset.StartsWith(itemPath) && asset.EndsWith(".prefab"))
            {
                shouldUpdate = true;
                break;
            }
        }

        foreach (string asset in deletedAssets)
        {
            if (asset.StartsWith(itemPath) && asset.EndsWith(".prefab"))
            {
                shouldUpdate = true;
                break;
            }
        }

        foreach (string asset in movedAssets)
        {
            if (asset.StartsWith(itemPath) && asset.EndsWith(".prefab"))
            {
                shouldUpdate = true;
                break;
            }
        }

        if (shouldUpdate)
            UpdateHeld();
    }

    private static void UpdateHeld()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { itemPath });
        List<GameObject> prefabs = new List<GameObject>();

        foreach(string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab.GetComponent<ItemDesc>() == null)
                continue;
            if(prefab != null)
            {
                prefabs.Add(prefab);
            }
        }

        ItemList itemList = AssetDatabase.LoadAssetAtPath<ItemList>(itemListPath);

        ItemFinder finder = Object.FindObjectOfType<ItemFinder>();
        if (finder != null)
            finder.itemList = itemList;

        if (itemList == null)
        {
            itemList = ScriptableObject.CreateInstance<ItemList>();
            AssetDatabase.CreateAsset(itemList, itemListPath);
        }

        itemList.items = prefabs.ToArray();
        EditorUtility.SetDirty(itemList);
        AssetDatabase.SaveAssets();
    }
}
