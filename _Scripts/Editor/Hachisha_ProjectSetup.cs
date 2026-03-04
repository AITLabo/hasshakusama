using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Hachisha_ProjectSetup : EditorWindow
{
    [MenuItem("hasshakusama/Setup All")]
    public static void SetupAll()
    {
        Debug.Log("--- Hachishakusama: Starting full setup ---");

        SetupTagsAndLayers();
        SetupSceneObjects();

        Debug.Log("--- Hachishakusama: Setup complete ---");
    }

    static void SetupTagsAndLayers()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        
        // Add Player tag if missing
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        bool hasPlayerTag = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == "Player") hasPlayerTag = true;
        }
        if (!hasPlayerTag)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            tagsProp.GetArrayElementAtIndex(0).stringValue = "Player";
            Debug.Log("Added 'Player' tag.");
        }

        tagManager.ApplyModifiedProperties();
    }

    static void SetupSceneObjects()
    {
        // 1. Setup Terrain
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.Log("Creating initial Terrain...");
            GameObject terrainGO = Terrain.CreateTerrainGameObject(new TerrainData());
            terrainGO.name = "Hachisha_Terrain";
            terrain = terrainGO.GetComponent<Terrain>();
        }

        // 2. Setup Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Creating Player...");
            player = new GameObject("Hachisha_Player");
            player.tag = "Player";
            player.AddComponent<CharacterController>();
            player.AddComponent<Hachisha_PlayerController>();
            
            GameObject camGO = new GameObject("FirstPersonCamera");
            camGO.transform.parent = player.transform;
            camGO.transform.localPosition = new Vector3(0, 1.7f, 0);
            camGO.AddComponent<Camera>();

            GameObject flashlightGO = new GameObject("Flashlight");
            flashlightGO.transform.parent = camGO.transform;
            flashlightGO.transform.localPosition = Vector3.zero;
            Light light = flashlightGO.AddComponent<Light>();
            light.type = LightType.Spot;
            light.intensity = 2f;
            light.range = 50f;
            light.spotAngle = 30f;

            Hachisha_PlayerController pc = player.GetComponent<Hachisha_PlayerController>();
            pc.flashlight = flashlightGO.transform;
        }

        // 3. Setup AI
        Hachisha_StalkingAI ai = FindObjectOfType<Hachisha_StalkingAI>();
        if (ai == null)
        {
            Debug.Log("Creating Hachishakusama AI...");
            GameObject aiGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            aiGO.name = "Hachishakusama";
            aiGO.transform.localScale = new Vector3(0.5f, 2.5f, 0.5f);
            aiGO.transform.position = new Vector3(20, 1.25f, 20);
            aiGO.AddComponent<UnityEngine.AI.NavMeshAgent>();
            ai = aiGO.AddComponent<Hachisha_StalkingAI>();
            ai.player = player.transform;

            // Add Audio to AI
            GameObject audioGO = new GameObject("PoPoPo_Audio");
            audioGO.transform.parent = aiGO.transform;
            audioGO.transform.localPosition = Vector3.zero;
            AudioSource source = audioGO.AddComponent<AudioSource>();
            Hachisha_AudioManager audioMgr = aiGO.AddComponent<Hachisha_AudioManager>();
            audioMgr.poPoPoSource = source;
            audioMgr.player = player.transform;
        }

        // 4. Setup Lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.01f, 0.01f, 0.02f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.05f;
    }
}
