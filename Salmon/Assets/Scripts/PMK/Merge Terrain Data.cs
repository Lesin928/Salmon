using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TerrainMerger : EditorWindow
{
    [MenuItem("Tools/Terrain Merger (Auto Position)")]
    public static void ShowWindow()
    {
        GetWindow<TerrainMerger>("Terrain Merger");
    }

    public Terrain[] sourceTerrains;

    private void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        SerializedProperty terrainsProp = so.FindProperty("sourceTerrains");

        EditorGUILayout.PropertyField(terrainsProp, true);

        if (GUILayout.Button("Merge Terrains"))
        {
            MergeTerrains();
        }

        so.ApplyModifiedProperties();
    }

    void MergeTerrains()
    {
        if (sourceTerrains == null || sourceTerrains.Length == 0)
        {
            Debug.LogError("No terrains assigned.");
            return;
        }

        // 모두 같은 크기, 해상도인지 확인
        float terrainWidth = sourceTerrains[0].terrainData.size.x;
        float terrainLength = sourceTerrains[0].terrainData.size.z;
        int heightRes = sourceTerrains[0].terrainData.heightmapResolution;
        float terrainHeight = sourceTerrains[0].terrainData.size.y;

        foreach (var t in sourceTerrains)
        {
            if (t.terrainData.size.x != terrainWidth || t.terrainData.size.z != terrainLength)
            {
                Debug.LogError("All terrains must have the same size!");
                return;
            }
            if (t.terrainData.heightmapResolution != heightRes)
            {
                Debug.LogError("All terrains must have the same heightmap resolution!");
                return;
            }
        }

        // 월드 좌표 기준으로 정렬 (Z 기준 행, X 기준 열)
        var sortedTerrains = sourceTerrains.OrderBy(t => t.transform.position.z).ThenBy(t => t.transform.position.x).ToArray();

        // 그리드 크기 계산
        int colCount = Mathf.RoundToInt((sortedTerrains.Max(t => t.transform.position.x) - sortedTerrains.Min(t => t.transform.position.x)) / terrainWidth) + 1;
        int rowCount = Mathf.RoundToInt((sortedTerrains.Max(t => t.transform.position.z) - sortedTerrains.Min(t => t.transform.position.z)) / terrainLength) + 1;

        // 새 TerrainData 크기 계산
        int heightWidth = (heightRes - 1) * colCount + 1;
        int heightHeight = (heightRes - 1) * rowCount + 1;

        TerrainData newData = new TerrainData();
        newData.heightmapResolution = heightWidth;
        newData.size = new Vector3(terrainWidth * colCount, terrainHeight, terrainLength * rowCount);

        // 알파맵, 디테일 해상도도 비율에 맞춰 설정 (간단히 첫 Terrain 기준 × colCount/rowCount)
        newData.alphamapResolution = sourceTerrains[0].terrainData.alphamapResolution * colCount;
        newData.baseMapResolution = sourceTerrains[0].terrainData.baseMapResolution * colCount;
        newData.SetDetailResolution(sourceTerrains[0].terrainData.detailResolution * colCount, 8);

        // 높이 데이터 병합
        float[,] newHeights = new float[heightHeight, heightWidth];

        // 나무 및 디테일 모으기
        List<TreeInstance> allTrees = new List<TreeInstance>();
        List<DetailPrototype> detailPrototypes = new List<DetailPrototype>();

        Vector3 minPos = new Vector3(sortedTerrains.Min(t => t.transform.position.x), 0, sortedTerrains.Min(t => t.transform.position.z));

        foreach (Terrain terrain in sortedTerrains)
        {
            Vector3 pos = terrain.transform.position;

            int xIndex = Mathf.RoundToInt((pos.x - minPos.x) / terrainWidth);
            int zIndex = Mathf.RoundToInt((pos.z - minPos.z) / terrainLength);

            float[,] heights = terrain.terrainData.GetHeights(0, 0, heightRes, heightRes);

            for (int i = 0; i < heightRes; i++)
            {
                for (int j = 0; j < heightRes; j++)
                {
                    int ni = Mathf.Clamp(zIndex * (heightRes - 1) + i, 0, heightHeight - 1);
                    int nj = Mathf.Clamp(xIndex * (heightRes - 1) + j, 0, heightWidth - 1);

                    newHeights[ni, nj] = heights[i, j];
                }
            }

            // 나무 위치 변환 및 추가
            foreach (TreeInstance tree in terrain.terrainData.treeInstances)
            {
                TreeInstance newTree = tree;

                Vector3 worldPos = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;

                Vector3 normPos = new Vector3(
                    (worldPos.x - minPos.x) / newData.size.x,
                    tree.position.y,
                    (worldPos.z - minPos.z) / newData.size.z
                );

                newTree.position = normPos;
                allTrees.Add(newTree);
            }

            // 디테일 프로토타입 병합
            foreach (DetailPrototype detail in terrain.terrainData.detailPrototypes)
            {
                if (!detailPrototypes.Contains(detail))
                    detailPrototypes.Add(detail);
            }
        }

        newData.SetHeights(0, 0, newHeights);
        newData.treeInstances = allTrees.ToArray();
        newData.treePrototypes = sourceTerrains[0].terrainData.treePrototypes;
        newData.detailPrototypes = detailPrototypes.ToArray();

        GameObject newTerrainObj = Terrain.CreateTerrainGameObject(newData);
        newTerrainObj.name = "MergedTerrain";
        newTerrainObj.transform.position = minPos;

        Debug.Log("Terrain merge complete.");
    }
}
