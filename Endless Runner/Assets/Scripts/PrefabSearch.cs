using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PrefabSearch : MonoBehaviour
{
    /// <summary>
    /// Finds all prefabs in a specific folder that have names starting with the specified string
    /// </summary>
    /// <param name="folderPath">The folder path to search in (relative to Assets folder)</param>
    /// <param name="nameStartsWith">The starting string for prefab names</param>
    /// <param name="searchSubfolders">Whether to search in subfolders recursively</param>
    /// <returns>List of found prefabs</returns>
    public static List<GameObject> FindPrefabsInFolder(string folderPath, string nameStartsWith, bool searchSubfolders = true)
    {
        List<GameObject> foundPrefabs = new List<GameObject>();
        
        // Validate inputs
        if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(nameStartsWith))
        {
            Debug.LogWarning("Folder path and name filter cannot be null or empty");
            return foundPrefabs;
        }
        
        // Ensure folder path is properly formatted
        if (!folderPath.StartsWith("Assets/"))
        {
            folderPath = "Assets/" + folderPath.TrimStart('/');
        }
        
        // Use the appropriate method based on runtime vs editor
        #if UNITY_EDITOR
        foundPrefabs = FindPrefabsInFolderEditor(folderPath, nameStartsWith, searchSubfolders);
        #else
        foundPrefabs = FindPrefabsInFolderRuntime(folderPath, nameStartsWith, searchSubfolders);
        #endif
        
        return foundPrefabs;
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// Editor-only implementation using AssetDatabase (more efficient)
    /// </summary>
    private static List<GameObject> FindPrefabsInFolderEditor(string folderPath, string nameStartsWith, bool searchSubfolders)
    {
        List<GameObject> foundPrefabs = new List<GameObject>();
        
        try
        {
            // Use AssetDatabase for efficient searching in editor
            string searchFilter = $"t:Prefab {nameStartsWith}*";
            string[] guids = UnityEditor.AssetDatabase.FindAssets(searchFilter, new[] { folderPath });
            
            foreach (string guid in guids)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                
                if (prefab != null && prefab.name.StartsWith(nameStartsWith))
                {
                    foundPrefabs.Add(prefab);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error searching for prefabs: {e.Message}");
        }
        
        Debug.Log($"Found {foundPrefabs.Count} prefabs in '{folderPath}' starting with '{nameStartsWith}'");
        return foundPrefabs;
    }
    #endif
    
    /// <summary>
    /// Runtime implementation using Resources folders
    /// </summary>
    private static List<GameObject> FindPrefabsInFolderRuntime(string folderPath, string nameStartsWith, bool searchSubfolders)
    {
        List<GameObject> foundPrefabs = new List<GameObject>();
        
        try
        {
            // Convert the folder path to a Resources-relative path
            string resourcesPath = ConvertToResourcesPath(folderPath);
            
            if (string.IsNullOrEmpty(resourcesPath))
            {
                Debug.LogWarning($"Folder path '{folderPath}' is not a valid Resources folder");
                return foundPrefabs;
            }
            
            // Load all prefabs from the Resources folder
            GameObject[] allPrefabs = Resources.LoadAll<GameObject>(resourcesPath);
            
            foreach (GameObject prefab in allPrefabs)
            {
                if (prefab.name.StartsWith(nameStartsWith))
                {
                    foundPrefabs.Add(prefab);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error searching for prefabs at runtime: {e.Message}");
        }
        
        Debug.Log($"Found {foundPrefabs.Count} prefabs in '{folderPath}' starting with '{nameStartsWith}' at runtime");
        return foundPrefabs;
    }
    
    /// <summary>
    /// Converts a full asset path to a Resources-relative path
    /// </summary>
    private static string ConvertToResourcesPath(string fullPath)
    {
        // Look for "Resources" in the path
        int resourcesIndex = fullPath.IndexOf("Resources/");
        if (resourcesIndex == -1)
        {
            return null;
        }
        
        // Extract the path after "Resources/"
        string resourcesRelativePath = fullPath.Substring(resourcesIndex + 10); // 10 = "Resources/".Length
        
        // Remove any trailing slashes
        resourcesRelativePath = resourcesRelativePath.TrimEnd('/');
        
        return resourcesRelativePath;
    }
    
    /// <summary>
    /// Finds prefabs and returns them as a dictionary keyed by their names
    /// </summary>
    public static Dictionary<string, GameObject> FindPrefabsInFolderAsDictionary(string folderPath, string nameStartsWith, bool searchSubfolders = true)
    {
        List<GameObject> prefabs = FindPrefabsInFolder(folderPath, nameStartsWith, searchSubfolders);
        return prefabs.ToDictionary(prefab => prefab.name, prefab => prefab);
    }
    
    /// <summary>
    /// Finds prefabs and instantiates them as children of a parent transform
    /// </summary>
    public static List<GameObject> FindAndInstantiatePrefabs(string folderPath, string nameStartsWith, Transform parent = null, bool searchSubfolders = true)
    {
        List<GameObject> prefabs = FindPrefabsInFolder(folderPath, nameStartsWith, searchSubfolders);
        List<GameObject> instances = new List<GameObject>();
        
        foreach (GameObject prefab in prefabs)
        {
            GameObject instance = Object.Instantiate(prefab, parent);
            instances.Add(instance);
        }
        
        return instances;
    }
}