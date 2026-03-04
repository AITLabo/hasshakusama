using UnityEngine;
using UnityEditor;

public class TerrainGenerators : EditorWindow
{
    float perlinScale = 0.1f;
    float heightScale = 10f;

    [MenuItem("Tools/Hachisha/Generate Mountainous Terrain")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGenerators>("Terrain Gen");
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        perlinScale = EditorGUILayout.Slider("Perlin Scale", perlinScale, 0.01f, 0.5f);
        heightScale = EditorGUILayout.Slider("Height Scale", heightScale, 1f, 50f);

        if (GUILayout.Button("Generate Noise Terrain"))
        {
            Generate();
        }
    }

    void Generate()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain found in scene!");
            return;
        }

        TerrainData data = terrain.terrainData;
        int res = data.heightmapResolution;
        float[,] heights = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                heights[x, y] = Mathf.PerlinNoise(x * perlinScale, y * perlinScale) * (heightScale / data.size.y);
            }
        }

        data.SetHeights(0, 0, heights);
        Debug.Log("Generated procedural mountains for Hachishakusama.");
    }
}
