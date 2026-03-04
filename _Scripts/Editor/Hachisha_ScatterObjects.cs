using UnityEngine;
using UnityEditor;

public class Hachisha_ScatterObjects : EditorWindow
{
    GameObject prefabToScatter;
    int count = 10;
    float range = 100f;

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
            Scatter();
        }
    }

    void Scatter()
    {
        if (prefabToScatter == null)
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

        for (int i = 0; i < count; i++)
        {
            float rx = Random.Range(-range, range);
            float rz = Random.Range(-range, range);
            Vector3 pos = new Vector3(rx, 100f, rz);

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 200f))
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToScatter);
                obj.transform.position = hit.point;
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Undo.RegisterCreatedObjectUndo(obj, "Scatter Object");
            }
        }
        Debug.Log($"Scattered {count} instances of {prefabToScatter.name}.");
    }
}
