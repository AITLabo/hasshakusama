using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Hachisha_ProjectSetup : EditorWindow
{
    // ─────────────────────────────────────────────────────────────────
    //  Asset Path Constants
    //  ※ Asset Store からインポートした後、実際のフォルダ名に合わせて変更してください
    // ─────────────────────────────────────────────────────────────────

    // --- Shrine Pack ---
    // Asset Store に複数の Shrine Pack が存在します。
    // "Shrine Pack" や "Japanese Shrine" などを購入後、
    // Project ウィンドウでプレハブのパスを確認して以下を書き換えてください。
    const string SHRINE_TORII_PREFAB     = "Assets/ShrinePackage/Prefabs/Torii.prefab";
    const string SHRINE_LANTERN_PREFAB   = "Assets/ShrinePackage/Prefabs/StoneLantern.prefab";
    const string SHRINE_PAVING_PREFAB    = "Assets/ShrinePackage/Prefabs/PavingStone.prefab"; // "P" = Paving Stone

    // --- European Forest ---
    // "European Forest" アセット等の木プレハブパスに合わせてください。
    // 例: Nobiax の "Yughues Free Trees" など
    const string EURO_TREE_PREFAB_01     = "Assets/EuropeanForest/Prefabs/Trees/Tree_Oak_01.prefab";
    const string EURO_TREE_PREFAB_02     = "Assets/EuropeanForest/Prefabs/Trees/Tree_Birch_01.prefab";
    const string EURO_TREE_PREFAB_DEAD   = "Assets/EuropeanForest/Prefabs/Trees/Tree_Dead_01.prefab";
    const string EURO_BUSH_PREFAB        = "Assets/EuropeanForest/Prefabs/Plants/Bush_01.prefab";

    // ─────────────────────────────────────────────────────────────────

    [MenuItem("hasshakusama/Setup All")]
    public static void SetupAll()
    {
        Debug.Log("--- Hachishakusama: Starting full setup ---");

        SetupTagsAndLayers();
        SetupSceneObjects();
        SetupShrineZones();

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

        // 2. Setup GameManager
        Hachisha_GameManager gm = FindObjectOfType<Hachisha_GameManager>();
        if (gm == null)
        {
            Debug.Log("Creating Game Manager...");
            GameObject gmGO = new GameObject("Hachisha_GameManager");
            gm = gmGO.AddComponent<Hachisha_GameManager>();
        }

        // 3. Setup Player
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
            light.spotAngle = 35f;

            Hachisha_PlayerController pc = player.GetComponent<Hachisha_PlayerController>();
            pc.flashlight = light;
        }

        // 4. Setup NPC Friends
        List<Transform> npcTargets = new List<Transform>();
        npcTargets.Add(player.transform);

        string[] friendNames = { "Friend_A", "Friend_B" };
        foreach (var fname in friendNames)
        {
            GameObject friend = GameObject.Find(fname);
            if (friend == null)
            {
                Debug.Log($"Creating {fname}...");
                friend = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                friend.name = fname;
                friend.transform.position = player.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                friend.AddComponent<UnityEngine.AI.NavMeshAgent>();
                var n = friend.AddComponent<Hachisha_NPCFriend>();
                n.player = player.transform;
            }
            npcTargets.Add(friend.transform);
        }

        // 5. Setup AI
        Hachisha_StalkingAI ai = FindObjectOfType<Hachisha_StalkingAI>();
        if (ai == null)
        {
            Debug.Log("Creating Hachishakusama AI...");
            GameObject aiGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            aiGO.name = "Hachishakusama";
            aiGO.transform.localScale = new Vector3(0.5f, 2.5f, 0.5f);
            aiGO.transform.position = player.transform.position + new Vector3(40, 1.25f, 40);
            aiGO.AddComponent<UnityEngine.AI.NavMeshAgent>();
            ai = aiGO.AddComponent<Hachisha_StalkingAI>();
            ai.targets = npcTargets.ToArray();

            // Add Audio to AI
            GameObject audioGO = new GameObject("PoPoPo_Audio");
            audioGO.transform.parent = aiGO.transform;
            audioGO.transform.localPosition = Vector3.zero;
            AudioSource source = audioGO.AddComponent<AudioSource>();
            Hachisha_AudioManager audioMgr = aiGO.AddComponent<Hachisha_AudioManager>();
            audioMgr.poPoPoSource = source;
            audioMgr.player = player.transform;
        }

        // 6. Setup Shrine SafeZones
        SetupShrineZones();

        // 7. Setup Lighting & Atmosphere
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.01f, 0.01f, 0.05f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.08f;

        // Directional Light Setup
        Light dirLight = FindObjectOfType<Light>();
        if (dirLight != null && dirLight.type == LightType.Directional)
        {
            dirLight.intensity = 0.05f;
            dirLight.color = new Color(0.2f, 0.3f, 0.5f);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    //  Shrine SafeZone Setup
    //  Shrine Pack (Torii + StoneLantern) を使った安全地帯を3箇所生成する
    // ─────────────────────────────────────────────────────────────────
    [MenuItem("hasshakusama/Setup Shrine Zones")]
    public static void SetupShrineZones()
    {
        // マップ上の安全地帯配置座標（地形に合わせて調整してください）
        Vector3[] shrinePositions = new Vector3[]
        {
            new Vector3(  50f, 0f,  50f),   // 北東エリア
            new Vector3(-150f, 0f, -80f),   // 南西エリア（目的地付近）
            new Vector3(  20f, 0f, -200f),  // 南エリア（ゴール前）
        };

        Hachisha_StalkingAI ai = Object.FindObjectOfType<Hachisha_StalkingAI>();

        foreach (var pos in shrinePositions)
        {
            string zoneName = "Hachisha_ShrineZone_" + System.Array.IndexOf(shrinePositions, pos);
            if (GameObject.Find(zoneName) != null) continue; // 既に存在する場合はスキップ

            // --- 安全地帯ルートオブジェクト ---
            GameObject zone = new GameObject(zoneName);
            zone.transform.position = pos;

            // SafeZone トリガーコライダー（鳥居の内側をカバーする範囲）
            BoxCollider trigger = zone.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(8f, 4f, 8f);
            trigger.center = new Vector3(0f, 2f, 0f);

            var safeZone = zone.AddComponent<Hachisha_SafeZone>();
            if (ai != null) safeZone.hachishaAI = ai;

            // --- Torii ゲートをトリガー入口に配置 ---
            SpawnShrineAsset(SHRINE_TORII_PREFAB, zone.transform,
                offset: new Vector3(0f, 0f, -4f),
                yRot: 0f,
                fallbackScale: new Vector3(0.5f, 0.5f, 0.5f));

            // --- StoneLantern をトリガー両脇に配置（2基） ---
            SpawnShrineAsset(SHRINE_LANTERN_PREFAB, zone.transform,
                offset: new Vector3(-1.5f, 0f, -3.5f),
                yRot: 0f,
                fallbackScale: new Vector3(0.4f, 0.4f, 0.4f));
            SpawnShrineAsset(SHRINE_LANTERN_PREFAB, zone.transform,
                offset: new Vector3( 1.5f, 0f, -3.5f),
                yRot: 0f,
                fallbackScale: new Vector3(0.4f, 0.4f, 0.4f));

            // --- Paving Stone を足元に配置（参道） ---
            for (int i = 0; i < 4; i++)
            {
                SpawnShrineAsset(SHRINE_PAVING_PREFAB, zone.transform,
                    offset: new Vector3(0f, 0f, -3f + i * 1.5f),
                    yRot: 0f,
                    fallbackScale: Vector3.one);
            }

            // --- ポイントライト（灯籠の灯り演出） ---
            GameObject lightGO = new GameObject("LanternLight");
            lightGO.transform.parent = zone.transform;
            lightGO.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            Light l = lightGO.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1.0f, 0.7f, 0.3f); // 暖色のろうそく色
            l.intensity = 1.5f;
            l.range = 8f;

            Undo.RegisterCreatedObjectUndo(zone, "Create Shrine Zone");
            Debug.Log($"[ShrineZone] Created '{zoneName}' at {pos}");
        }
    }

    static void SpawnShrineAsset(string prefabPath, Transform parent, Vector3 offset, float yRot, Vector3 fallbackScale)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        GameObject obj;

        if (prefab != null)
        {
            obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        }
        else
        {
            // プレハブが見つからない場合はプレースホルダー Cube で代替
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.parent = parent;
            obj.transform.localScale = fallbackScale;
            Debug.LogWarning($"[ShrineZone] Prefab not found: {prefabPath}  →  プレースホルダーを配置しました。SHRINE_*_PREFAB パスを実際のパスに更新してください。");
        }

        obj.transform.localPosition = offset;
        obj.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
        Undo.RegisterCreatedObjectUndo(obj, "Spawn Shrine Asset");
    }

    // ─────────────────────────────────────────────────────────────────
    //  European Forest — Terrain Tree Prototype 登録
    //  Terrain の Tree Paint で使えるように Tree Prototype を自動登録する
    // ─────────────────────────────────────────────────────────────────
    [MenuItem("hasshakusama/Register European Forest Trees")]
    public static void RegisterEuropeanForestTrees()
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("[EuroForest] アクティブな Terrain が見つかりません。先に Terrain を作成してください。");
            return;
        }

        string[] treePrefabPaths = new string[]
        {
            EURO_TREE_PREFAB_01,
            EURO_TREE_PREFAB_02,
            EURO_TREE_PREFAB_DEAD,
            EURO_BUSH_PREFAB,
        };

        List<TreePrototype> prototypes = new List<TreePrototype>(terrain.terrainData.treePrototypes);
        int added = 0;

        foreach (var path in treePrefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"[EuroForest] プレハブが見つかりません: {path}\n  → EURO_TREE_PREFAB_* パスを実際のパスに更新してください。");
                continue;
            }
            // 重複登録チェック
            bool alreadyExists = false;
            foreach (var p in prototypes)
            {
                if (p.prefab == prefab) { alreadyExists = true; break; }
            }
            if (alreadyExists) continue;

            prototypes.Add(new TreePrototype { prefab = prefab, bendFactor = 0.5f });
            added++;
            Debug.Log($"[EuroForest] Tree Prototype 登録: {prefab.name}");
        }

        if (added > 0)
        {
            terrain.terrainData.treePrototypes = prototypes.ToArray();
            EditorUtility.SetDirty(terrain.terrainData);
            Debug.Log($"[EuroForest] {added} 本の Tree Prototype を Terrain に登録しました。Terrain の Paint Trees で使用できます。");
        }
        else
        {
            Debug.Log("[EuroForest] 追加する Tree Prototype はありませんでした（既に登録済みか、プレハブが見つかりません）。");
        }
    }
}
