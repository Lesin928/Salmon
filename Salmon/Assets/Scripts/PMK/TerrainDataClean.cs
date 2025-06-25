using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TerrainDataCleaner
{
    [MenuItem("Tools/Cleanup Unused TerrainData (All Scenes)")]
    public static void CleanUnusedTerrainData_AllScenes()
    {
        string[] allScenePaths = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        HashSet<TerrainData> usedTerrainData = new HashSet<TerrainData>();

        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        foreach (string scenePath in allScenePaths)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();

            foreach (var terrain in terrains)
            {
                if (terrain.terrainData != null)
                {
                    usedTerrainData.Add(terrain.terrainData);
                }
            }
        }

        // 다시 원래 씬으로 돌아가기
        EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);

        // 모든 TerrainData 에셋 경로
        string[] allTerrainDataPaths = AssetDatabase.FindAssets("t:TerrainData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        int deletedCount = 0;
        foreach (var path in allTerrainDataPaths)
        {
            var terrainData = AssetDatabase.LoadAssetAtPath<TerrainData>(path);
            if (terrainData != null && !usedTerrainData.Contains(terrainData))
            {
                bool success = AssetDatabase.DeleteAsset(path);
                if (success)
                {
                    Debug.Log($"Deleted unused TerrainData: {path}");
                    deletedCount++;
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Cleanup complete. Deleted {deletedCount} unused TerrainData assets.");
    }
}
