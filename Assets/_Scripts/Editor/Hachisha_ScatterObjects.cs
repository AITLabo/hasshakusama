using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Hachisha_ScatterObjects : EditorWindow
{
    GameObject prefabToScatter;
    int count = 10;
    float range = 100f;

    // ─── European Forest プリセット設定 ───────────────────────────────
    // プレハブパスは Hachisha_ProjectSetup.cs の EURO_* 定数と合わせてください
    static readonly (string path, int count, float range)[] EURO_FOREST_PRESETS = new[]
    {
        ("Assets/EuropeanForest/Prefabs/Trees/Tree_Oak_01.prefab",   80, 220f),
        ("Assets/EuropeanForest/Prefabs/Trees/Tree_Birch_01.prefab", 60, 200f),
        ("Assets/EuropeanForest/Prefabs/Trees/Tree_Dead_01.prefab",  30, 180f), // 枯れ木：より密林奥
        ("Assets/EuropeanForest/Prefabs/Plants/Bush_01.prefab",      50, 160f),
    };

    [MenuItem("hasshakusama/Scatter Objects")]
    public static void ShowWindow()
    {
        GetWindow<Hachisha_ScatterObjects>("Scatter Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Scatter Tool", EditorStyles.boldLabel);
        prefabToScatter = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToScatter, typeof(GameObject), false);
        count = EditorGUILayout.IntSlider("Count", count, 1, 100);
        range = EditorGUILayout.Slider("Range", range, 10f, 500f);

        if (GUILayout.Button("Scatter Objects"))
        {
            Scatter(prefabToScatter, count, range);
        }

        EditorGUILayout.Space(10);
        GUILayout.Label("─── European Forest Preset ───", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "欧州森林アセットの木々・茂みを一括配置します。\n" +
            "EURO_FOREST_PRESETS のパスが実際のアセットパスと合っていることを確認してください。",
            MessageType.Info);
        if (GUILayout.Button("🌲  Scatter European Forest (全プリセット一括)"))
        {
            ScatterEuropeanForest();
        }
    }

    void Scatter(GameObject prefab, int spawnCount, float spawnRange)
    {
        if (prefab == null)
        {
            Debug.LogError("Assign a prefab first!");
            return;
        }

        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain!");
            return;
        }

        int placed = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            float rx = Random.Range(-spawnRange, spawnRange);
            float rz = Random.Range(-spawnRange, spawnRange);
            Vector3 pos = new Vector3(rx, 500f, rz);

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 1000f))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > 45f) continue;

                // Shrine Zone 内には木を置かない（安全地帯を守る）
                if (IsInsideAnyShrineZone(hit.point)) continue;

                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                obj.transform.position = hit.point;
                float scale = Random.Range(0.8f, 1.3f);
                obj.transform.localScale = Vector3.one * scale;
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Undo.RegisterCreatedObjectUndo(obj, "Scatter Object");
                placed++;
            }
        }
        Debug.Log($"[Scatter] {prefab.name}: {placed}/{spawnCount} 個配置しました。");
    }

    // ─── European Forest 一括スキャッター ─────────────────────────────
    [MenuItem("hasshakusama/Scatter European Forest")]
    public static void ScatterEuropeanForest()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("[EuroForest Scatter] アクティブな Terrain が見つかりません。");
            return;
        }

        int totalPlaced = 0;
        foreach (var (path, cnt, rng) in EURO_FOREST_PRESETS)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"[EuroForest Scatter] プレハブが見つかりません: {path}");
                continue;
            }

            int placed = 0;
            for (int i = 0; i < cnt; i++)
            {
                float rx = Random.Range(-rng, rng);
                float rz = Random.Range(-rng, rng);
                Vector3 pos = new Vector3(rx, 500f, rz);

                RaycastHit hit;
                if (Physics.Raycast(pos, Vector3.down, out hit, 1000f))
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) > 40f) continue;
                    if (IsInsideAnyShrineZone(hit.point)) continue;

                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    obj.transform.position = hit.point;
                    float scale = Random.Range(0.85f, 1.25f);
                    obj.transform.localScale = Vector3.one * scale;
                    obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    Undo.RegisterCreatedObjectUndo(obj, "Scatter EuroForest");
                    placed++;
                }
            }
            Debug.Log($"[EuroForest Scatter] {prefab.name}: {placed}/{cnt} 個配置");
            totalPlaced += placed;
        }
        Debug.Log($"[EuroForest Scatter] 完了 — 合計 {totalPlaced} 個のオブジェクトを配置しました。");
    }

    // Shrine Zone のトリガー内かどうかチェック（木が安全地帯を塞がないように）
    static bool IsInsideAnyShrineZone(Vector3 point)
    {
        foreach (var zone in Object.FindObjectsOfType<Hachisha_SafeZone>())
        {
            Collider col = zone.GetComponent<Collider>();
            if (col != null && col.bounds.Contains(point)) return true;
        }
        return false;
    }
}
