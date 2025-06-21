using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// 네 개의 Terrain을 하나로 병합하는 Unity 에디터 윈도우
public class TerrainMergerEditor : EditorWindow
{
    // 병합할 네 개의 Terrain 참조
    Terrain topLeft;
    Terrain topRight;
    Terrain bottomLeft;
    Terrain bottomRight;

    // 메뉴에 "Tools/Terrain Merger" 항목 추가
    [MenuItem("Tools/Terrain Merger")]
    public static void ShowWindow()
    {
        // 에디터 윈도우 표시
        GetWindow<TerrainMergerEditor>("Terrain Merger");
    }

    // 에디터 윈도우 UI 그리기
    void OnGUI()
    {
        GUILayout.Label("Assign 4 Terrains to Merge", EditorStyles.boldLabel);

        // 네 개의 Terrain 오브젝트 필드
        topLeft = (Terrain)EditorGUILayout.ObjectField("Top Left", topLeft, typeof(Terrain), true);
        topRight = (Terrain)EditorGUILayout.ObjectField("Top Right", topRight, typeof(Terrain), true);
        bottomLeft = (Terrain)EditorGUILayout.ObjectField("Bottom Left", bottomLeft, typeof(Terrain), true);
        bottomRight = (Terrain)EditorGUILayout.ObjectField("Bottom Right", bottomRight, typeof(Terrain), true);

        // 병합 버튼
        if (GUILayout.Button("Merge Terrains"))
        {
            MergeTerrains();
        }
    }

    // Terrain 병합 로직
    void MergeTerrains()
    {
        // Terrain 유효성 검사
        if (!ValidateTerrains()) return;

        int res = topLeft.terrainData.heightmapResolution;

        // 각 Terrain의 높이맵 데이터 읽기
        float[,] tlHeights = topLeft.terrainData.GetHeights(0, 0, res, res);
        float[,] trHeights = topRight.terrainData.GetHeights(0, 0, res, res);
        float[,] blHeights = bottomLeft.terrainData.GetHeights(0, 0, res, res);
        float[,] brHeights = bottomRight.terrainData.GetHeights(0, 0, res, res);

        // 병합될 Terrain의 해상도 계산
        int mergedRes = (res - 1) * 2 + 1;
        float[,] mergedHeights = new float[mergedRes, mergedRes];

        // 높이맵 병합 (상하는 그대로, 좌우만 바꿈)
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                mergedHeights[y, x] = trHeights[y, x];                   // Top Right → 좌상단
                mergedHeights[y, x + res - 1] = tlHeights[y, x];         // Top Left → 우상단
                mergedHeights[y + res - 1, x] = brHeights[y, x];         // Bottom Right → 좌하단
                mergedHeights[y + res - 1, x + res - 1] = blHeights[y, x]; // Bottom Left → 우하단
            }
        }

        // TerrainLayer 병합 및 매핑 정보 생성
        Dictionary<Terrain, int[]> layerMappings;
        TerrainLayer[] mergedLayers = MergeTerrainLayers(out layerMappings);

        // 알파맵(스플랫맵) 병합
        int alphamapRes = topLeft.terrainData.alphamapResolution;
        int mergedAlphaRes = (alphamapRes - 1) * 2 + 1;
        int totalLayers = mergedLayers.Length;
        float[,,] mergedAlphamaps = new float[mergedAlphaRes, mergedAlphaRes, totalLayers];

        // 알파맵 데이터 병합
        MergeAlphamaps(mergedAlphamaps, alphamapRes, layerMappings);

        // 새 TerrainData 생성 및 설정
        TerrainData newTerrainData = new TerrainData();
        newTerrainData.heightmapResolution = mergedRes;
        newTerrainData.size = new Vector3(
            topLeft.terrainData.size.x * 2,   // 가로 2배
            topLeft.terrainData.size.y,       // 높이 동일
            topLeft.terrainData.size.z * 2);  // 세로 2배
        newTerrainData.terrainLayers = mergedLayers;

        // 병합된 높이맵과 알파맵 적용
        newTerrainData.SetHeights(0, 0, mergedHeights);
        newTerrainData.SetAlphamaps(0, 0, mergedAlphamaps);

        // 나무(Detail) 데이터 병합은 필요시 추가 가능

        // 새 Terrain 오브젝트 생성
        GameObject terrainGO = Terrain.CreateTerrainGameObject(newTerrainData);
        terrainGO.name = "MergedTerrain";

        Debug.Log("Terrains merged successfully!");
    }

    // 네 Terrain의 TerrainLayer를 병합하고, 각 Terrain별로 매핑 정보 반환
    TerrainLayer[] MergeTerrainLayers(out Dictionary<Terrain, int[]> layerMappings)
    {
        layerMappings = new Dictionary<Terrain, int[]>();
        List<TerrainLayer> mergedLayerList = new List<TerrainLayer>();

        Terrain[] terrains = { topLeft, topRight, bottomLeft, bottomRight };

        foreach (var terrain in terrains)
        {
            var layers = terrain.terrainData.terrainLayers;
            int[] mapping = new int[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                int index = mergedLayerList.IndexOf(layer);
                if (index < 0)
                {
                    mergedLayerList.Add(layer);
                    index = mergedLayerList.Count - 1;
                }
                mapping[i] = index; // 기존 레이어가 병합 레이어에서 몇 번째인지 기록
            }
            layerMappings[terrain] = mapping;
        }
        return mergedLayerList.ToArray();
    }

    // 알파맵(스플랫맵) 병합
    // merged: 병합될 알파맵 배열
    // res: 원본 알파맵 해상도
    // layerMappings: Terrain별 레이어 매핑 정보
    void MergeAlphamaps(float[,,] merged, int res, Dictionary<Terrain, int[]> layerMappings)
    {
        // 각 Terrain의 알파맵 데이터 읽기
        float[,,] tlAlpha = topLeft.terrainData.GetAlphamaps(0, 0, res, res);
        float[,,] trAlpha = topRight.terrainData.GetAlphamaps(0, 0, res, res);
        float[,,] blAlpha = bottomLeft.terrainData.GetAlphamaps(0, 0, res, res);
        float[,,] brAlpha = bottomRight.terrainData.GetAlphamaps(0, 0, res, res);

        // 병합 알파맵 초기화
        for (int y = 0; y < merged.GetLength(0); y++)
            for (int x = 0; x < merged.GetLength(1); x++)
                for (int layer = 0; layer < merged.GetLength(2); layer++)
                    merged[y, x, layer] = 0f;

        // 각 Terrain의 알파맵을 병합 알파맵에 복사
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                // Top Right → 좌상단
                CopyAlpha(trAlpha, merged, layerMappings[topRight], x, y, x, y);
                // Top Left → 우상단
                CopyAlpha(tlAlpha, merged, layerMappings[topLeft], x, y, x + res - 1, y);
                // Bottom Right → 좌하단
                CopyAlpha(brAlpha, merged, layerMappings[bottomRight], x, y, x, y + res - 1);
                // Bottom Left → 우하단
                CopyAlpha(blAlpha, merged, layerMappings[bottomLeft], x, y, x + res - 1, y + res - 1);
            }
        }
    }

    // 알파맵 한 픽셀의 모든 레이어 값을 병합 알파맵에 복사
    // source: 원본 알파맵
    // target: 병합 알파맵
    // mapping: 레이어 매핑 정보
    // sx, sy: 원본 좌표
    // tx, ty: 타겟 좌표
    void CopyAlpha(float[,,] source, float[,,] target, int[] mapping, int sx, int sy, int tx, int ty)
    {
        for (int layer = 0; layer < mapping.Length; layer++)
        {
            int targetLayer = mapping[layer];
            target[ty, tx, targetLayer] = source[sy, sx, layer];
        }
    }

    // Terrain 유효성 검사 (null 체크, 해상도 일치 여부 등)
    bool ValidateTerrains()
    {
        // 네 Terrain이 모두 할당되었는지 확인
        if (!topLeft || !topRight || !bottomLeft || !bottomRight)
        {
            EditorUtility.DisplayDialog("Error", "Please assign all 4 terrains.", "OK");
            return false;
        }

        int res = topLeft.terrainData.heightmapResolution;
        int alphaRes = topLeft.terrainData.alphamapResolution;

        // 높이맵 해상도 일치 여부 확인
        if (topRight.terrainData.heightmapResolution != res ||
            bottomLeft.terrainData.heightmapResolution != res ||
            bottomRight.terrainData.heightmapResolution != res)
        {
            EditorUtility.DisplayDialog("Error", "All terrains must have the same heightmap resolution.", "OK");
            return false;
        }

        // 알파맵 해상도 일치 여부 확인
        if (topRight.terrainData.alphamapResolution != alphaRes ||
            bottomLeft.terrainData.alphamapResolution != alphaRes ||
            bottomRight.terrainData.alphamapResolution != alphaRes)
        {
            EditorUtility.DisplayDialog("Error", "All terrains must have the same alphamap resolution.", "OK");
            return false;
        }

        return true;
    }
}
