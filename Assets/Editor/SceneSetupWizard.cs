using UnityEngine;
using UnityEditor;

// Menu: Ireke Onibudo > Build Entire Universe
//
// Sibling of ogbojuode-3d's SceneSetupWizard, same architecture, different
// story: a riverside hub village instead of a forest hub, an undersea
// kingdom instead of a forest, three creatures drawn from Ireke Onibudo's
// actual trials instead of Ogboju Ode's, and Arogidigba (mermaid queen) as
// the riddle-boss instead of the Ostrich-King.
//
// Loads real FBX models from Assets/Models/ where they've been imported
// (see irekeonibudo-meshy_prompts.txt for the full asset list), falling
// back to primitives for anything not yet generated — the same
// load-real-asset-with-fallback pattern used in ogbojuode-3d.
public static class SceneSetupWizard
{
    [MenuItem("Ireke Onibudo/Build Entire Universe")]
    public static void BuildEverything()
    {
        EnsureTags();

        GameObject root = new GameObject("Ijoba_Omi_World");

        BuildUnderseaKingdom(root.transform);
        BuildRiverHub(root.transform);
        GameObject player = BuildPlayer(root.transform);
        BuildCreatures(root.transform);
        BuildArogidigba(root.transform);
        BuildMotherSpiritGuides(root.transform);
        BuildCameraAndLighting(player.transform);
        BuildManagers(root.transform, player.transform);

        Debug.Log("World built: riverside hub, undersea kingdom, three creatures " +
                  "(flying snake, wrestler-cat, warrior fish), Arogidigba, mother-spirit " +
                  "guides, player, camera, lighting, expedition/wisdom tracking.");
    }

    private static void EnsureTags()
    {
        AddTagIfMissing("Player");
        AddTagIfMissing("Enemy");
    }

    private static void AddTagIfMissing(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        for (int i = 0; i < tagsProp.arraySize; i++)
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag) return;

        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    // Returns the first imported model (FBX/etc.) found under folderPath, or
    // null if that folder doesn't exist or has no model yet.
    private static GameObject LoadModelInFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath)) return null;
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folderPath });
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    // Instantiates the real model at folderPath if one has been imported;
    // otherwise builds a colored primitive placeholder of fallbackType. Real
    // models are left with their own materials untouched; only the
    // primitive fallback gets fallbackColor applied.
    private static GameObject InstantiateModelOrFallback(string folderPath, PrimitiveType fallbackType, Color fallbackColor)
    {
        GameObject modelAsset = LoadModelInFolder(folderPath);
        if (modelAsset != null)
            return (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);

        GameObject placeholder = GameObject.CreatePrimitive(fallbackType);
        SetColor(placeholder, fallbackColor);
        return placeholder;
    }

    // Props (Kenney kit pieces) live directly under Assets/Models/Props/ as
    // single files, unlike characters which get their own subfolder — so
    // this loads by exact filename rather than searching a folder.
    private static GameObject LoadPropModel(string fileName)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Props/" + fileName);
    }

    private static GameObject InstantiatePropModel(string fileName)
    {
        GameObject asset = LoadPropModel(fileName);
        return asset != null ? (GameObject)PrefabUtility.InstantiatePrefab(asset) : null;
    }

    private const string SpearPrefabPath = "Assets/Prefabs/ThrownSpear.prefab";

    // Raw imported FBX models have no physics components, so
    // IrekeOnibudoController.ThrowSpear()'s `rb.linearVelocity = ...` would
    // silently no-op against the bare model. This builds a proper thrown
    // prefab once (Rigidbody, gravity off so it flies straight; a small
    // trigger Collider so it doesn't get stopped by scenery) and reuses the
    // saved prefab asset on every subsequent run instead of rebuilding it.
    private static GameObject EnsureSpearPrefab()
    {
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(SpearPrefabPath);
        if (existing != null) return existing;

        GameObject spearModel = LoadModelInFolder("Assets/Models/Props/Spear_Oko_Eja");
        if (spearModel == null) return null;

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(spearModel);
        instance.name = "ThrownSpear";

        Rigidbody rb = instance.AddComponent<Rigidbody>();
        rb.useGravity = false;

        if (instance.GetComponentInChildren<Collider>() == null)
        {
            CapsuleCollider col = instance.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.radius = 0.1f;
            col.height = 1.2f;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, SpearPrefabPath);
        Object.DestroyImmediate(instance);
        return prefab;
    }

    // --- Undersea kingdom (Ijoba Omi): the dangerous expanse, standing in
    // for the forest — reef instead of trees, glowing plankton instead of
    // spirit fungus. Stays primitive-only: the imported Kenney props are
    // all land assets (fantasy-town/nature/survival kits), so there's
    // nothing appropriate here yet to swap the reef/plankton for. ---
    private static void BuildUnderseaKingdom(Transform parent)
    {
        GameObject kingdom = new GameObject("Ijoba_Omi_Undersea_Kingdom");
        kingdom.transform.parent = parent;

        GameObject seabed = GameObject.CreatePrimitive(PrimitiveType.Plane);
        seabed.name = "Seabed_Floor";
        seabed.transform.parent = kingdom.transform;
        seabed.transform.position = new Vector3(0f, -1f, 40f);
        seabed.transform.localScale = new Vector3(12f, 1f, 8f);
        SetColor(seabed, new Color(0.1f, 0.15f, 0.2f));

        for (int i = 0; i < 20; i++)
        {
            GameObject reef = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            reef.name = "Coral_Reef_" + i;
            reef.transform.parent = kingdom.transform;
            reef.transform.position = new Vector3(Random.Range(-45f, 45f), -0.5f, Random.Range(10f, 75f));
            reef.transform.localScale = new Vector3(Random.Range(1f, 2f), Random.Range(1.5f, 3f), Random.Range(1f, 2f));
            SetColor(reef, new Color(Random.Range(0.4f, 0.8f), 0.2f, 0.3f));
        }

        for (int i = 0; i < 15; i++)
        {
            GameObject plankton = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            plankton.name = "Glowing_Plankton_" + i;
            plankton.transform.parent = kingdom.transform;
            plankton.transform.position = new Vector3(Random.Range(-40f, 40f), Random.Range(0f, 3f), Random.Range(10f, 75f));
            plankton.transform.localScale = Vector3.one * 0.25f;
            SetColor(plankton, new Color(0.4f, 0.8f, 1f));
        }
    }

    // --- Riverside hub (Ilu Eti-Odo): safe village Ireke Onibudo sets out
    // from — huts, a dock, a canoe, a shrine to Olokun (sea god) as the
    // gathering point in place of a bonfire, reed barrier at the water's
    // edge in place of defensive spikes. ---
    private static void BuildRiverHub(Transform parent)
    {
        GameObject hub = new GameObject("Ilu_Eti_Odo_Riverside_Hub");
        hub.transform.parent = parent;

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Village_Ground";
        ground.transform.parent = hub.transform;
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(4f, 1f, 4f);
        SetColor(ground, new Color(0.6f, 0.5f, 0.3f));

        Vector3[] hutPositions =
        {
            new Vector3(-10f, 0f, -5f),
            new Vector3(10f, 0f, -5f),
            new Vector3(-10f, 0f, 8f),
            new Vector3(10f, 0f, 8f),
        };
        foreach (Vector3 pos in hutPositions)
        {
            BuildHut(hub.transform, pos);
        }

        GameObject dock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dock.name = "Riverside_Dock";
        dock.transform.parent = hub.transform;
        dock.transform.position = new Vector3(0f, 0.3f, 14f);
        dock.transform.localScale = new Vector3(4f, 0.3f, 6f);
        SetColor(dock, new Color(0.35f, 0.25f, 0.15f));

        // No plank/piling asset was imported for the dock deck itself, so it
        // stays primitive; pillar-wood posts underneath it are real, though.
        GameObject dockPillarAsset = LoadPropModel("pillar-wood.fbx");
        if (dockPillarAsset != null)
        {
            Vector3[] postOffsets =
            {
                new Vector3(-1.5f, -0.6f, 12f), new Vector3(1.5f, -0.6f, 12f),
                new Vector3(-1.5f, -0.6f, 16f), new Vector3(1.5f, -0.6f, 16f),
            };
            foreach (Vector3 offset in postOffsets)
            {
                GameObject post = (GameObject)PrefabUtility.InstantiatePrefab(dockPillarAsset);
                post.name = "Dock_Support_Post";
                post.transform.parent = hub.transform;
                post.transform.position = offset;
            }
        }

        GameObject canoe = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        canoe.name = "Canoe";
        canoe.transform.parent = hub.transform;
        canoe.transform.position = new Vector3(2f, 0.4f, 16f);
        canoe.transform.localScale = new Vector3(0.6f, 1.8f, 0.6f);
        canoe.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        SetColor(canoe, new Color(0.4f, 0.25f, 0.1f));

        GameObject shrine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shrine.name = "Shrine_To_Olokun";
        shrine.transform.parent = hub.transform;
        shrine.transform.position = new Vector3(0f, 0.6f, 0f);
        shrine.transform.localScale = new Vector3(1.2f, 0.6f, 1.2f);
        SetColor(shrine, new Color(0.2f, 0.4f, 0.6f));

        Light shrineLight = shrine.AddComponent<Light>();
        shrineLight.type = LightType.Point;
        shrineLight.color = new Color(0.3f, 0.6f, 0.9f);
        shrineLight.range = 15f;
        shrineLight.intensity = 1.5f;

        // Real campfire beside the shrine (extra gathering-point detail;
        // doesn't replace the shrine, just dresses the area around it).
        GameObject campfire = InstantiatePropModel("campfire-pit.fbx");
        if (campfire != null)
        {
            campfire.name = "Campfire_Near_Shrine";
            campfire.transform.parent = hub.transform;
            campfire.transform.position = new Vector3(2.5f, 0f, 0f);
        }

        // Reed barrier at the water's edge — real fortified-fence pieces
        // where available, falling back to the thin cylinder posts.
        GameObject fenceAsset = LoadPropModel("fence-fortified.fbx");
        for (int i = 0; i < 10; i++)
        {
            float x = Mathf.Lerp(-20f, 20f, i / 9f);

            if (fenceAsset != null)
            {
                GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(fenceAsset);
                fence.name = "Reed_Barrier_" + i;
                fence.transform.parent = hub.transform;
                fence.transform.position = new Vector3(x, 0f, 12f);
            }
            else
            {
                GameObject reed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                reed.name = "Reed_Barrier_" + i;
                reed.transform.parent = hub.transform;
                reed.transform.position = new Vector3(x, 0.6f, 12f);
                reed.transform.localScale = new Vector3(0.1f, 0.6f, 0.1f);
                SetColor(reed, new Color(0.5f, 0.55f, 0.2f));
            }
        }

        BuildVillageFringe(hub.transform);
    }

    // Real tree_oak_dark / mushroom_redGroup props scattered around the
    // village's edge for atmosphere; skipped entirely if the assets aren't
    // imported (no primitive fallback — this decoration is purely additive).
    private static void BuildVillageFringe(Transform hubTransform)
    {
        GameObject treeAsset = LoadPropModel("tree_oak_dark.fbx");
        if (treeAsset != null)
        {
            Vector3[] treePositions =
            {
                new Vector3(-16f, 0f, -8f), new Vector3(16f, 0f, -8f),
                new Vector3(-18f, 0f, 5f), new Vector3(18f, 0f, 5f),
            };
            foreach (Vector3 pos in treePositions)
            {
                GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(treeAsset);
                tree.name = "Village_Fringe_Tree";
                tree.transform.parent = hubTransform;
                tree.transform.position = pos;
            }
        }

        GameObject mushroomAsset = LoadPropModel("mushroom_redGroup.fbx");
        if (mushroomAsset != null)
        {
            Vector3[] mushroomPositions = { new Vector3(-14f, 0f, -6f), new Vector3(14f, 0f, -6f) };
            foreach (Vector3 pos in mushroomPositions)
            {
                GameObject mushroom = (GameObject)PrefabUtility.InstantiatePrefab(mushroomAsset);
                mushroom.name = "Village_Fringe_Mushroom";
                mushroom.transform.parent = hubTransform;
                mushroom.transform.position = pos;
            }
        }
    }

    // Builds one hut from real Kenney pieces (four wall-wood walls, a
    // roof-high-point roof, pillar-wood corner posts) when they're
    // available; falls back to the original cylinder-body/cube-roof
    // primitive hut otherwise. Either way the hut gets a BoxCollider so it
    // still blocks movement/raycasts the way the old primitive body did.
    private static void BuildHut(Transform hubTransform, Vector3 position)
    {
        GameObject hut = new GameObject("Riverside_Hut");
        hut.transform.parent = hubTransform;
        hut.transform.position = position;

        GameObject wallAsset = LoadPropModel("wall-wood.fbx");
        GameObject roofAsset = LoadPropModel("roof-high-point.fbx");
        GameObject pillarAsset = LoadPropModel("pillar-wood.fbx");

        if (wallAsset != null && roofAsset != null)
        {
            const float half = 1.25f;
            Vector3[] wallOffsets = { new Vector3(0f, 0f, half), new Vector3(0f, 0f, -half), new Vector3(half, 0f, 0f), new Vector3(-half, 0f, 0f) };
            float[] wallYRotations = { 0f, 180f, 90f, 270f };
            for (int i = 0; i < 4; i++)
            {
                GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallAsset);
                wall.name = "Hut_Wall";
                wall.transform.parent = hut.transform;
                wall.transform.localPosition = wallOffsets[i];
                wall.transform.localRotation = Quaternion.Euler(0f, wallYRotations[i], 0f);
            }

            GameObject roof = (GameObject)PrefabUtility.InstantiatePrefab(roofAsset);
            roof.name = "Hut_Roof";
            roof.transform.parent = hut.transform;
            roof.transform.localPosition = new Vector3(0f, 2f, 0f);

            if (pillarAsset != null)
            {
                Vector3[] cornerOffsets =
                {
                    new Vector3(half, 0f, half), new Vector3(-half, 0f, half),
                    new Vector3(half, 0f, -half), new Vector3(-half, 0f, -half),
                };
                foreach (Vector3 corner in cornerOffsets)
                {
                    GameObject post = (GameObject)PrefabUtility.InstantiatePrefab(pillarAsset);
                    post.name = "Hut_Corner_Post";
                    post.transform.parent = hut.transform;
                    post.transform.localPosition = corner;
                }
            }

            BoxCollider col = hut.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 1f, 0f);
            col.size = new Vector3(half * 2f, 2f, half * 2f);
        }
        else
        {
            GameObject hutBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hutBody.name = "Riverside_Hut_Body";
            hutBody.transform.parent = hut.transform;
            hutBody.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            hutBody.transform.localScale = new Vector3(2.5f, 1.5f, 2.5f);
            SetColor(hutBody, new Color(0.6f, 0.4f, 0.2f));

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Thatched_Roof";
            roof.transform.parent = hutBody.transform;
            roof.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            roof.transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
            roof.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            SetColor(roof, new Color(0.4f, 0.3f, 0.1f));
        }
    }

    private static GameObject BuildPlayer(Transform parent)
    {
        // Root carries the tag/controller/collider regardless of whether a
        // real model or a primitive ends up as the visual, so
        // IrekeOnibudoController and CreatureAI raycasts keep working
        // either way.
        GameObject player = new GameObject("Player_Ireke_Onibudo");
        player.tag = "Player";
        player.transform.parent = parent;
        player.transform.position = new Vector3(0f, 1f, -10f);
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerVitals>();
        IrekeOnibudoController controller = player.AddComponent<IrekeOnibudoController>();

        GameObject visual = InstantiateModelOrFallback(
            "Assets/Models/Characters/Player_Ireke_Onibudo",
            PrimitiveType.Capsule, new Color(0.5f, 0.3f, 0.15f));
        visual.name = "Player_Visual";
        visual.transform.parent = player.transform;
        visual.transform.localPosition = Vector3.zero;
        // Movement collision is handled by CharacterController on the root;
        // strip any collider that came with the visual (primitive fallback
        // has one, imported FBX models normally don't).
        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null) Object.DestroyImmediate(visualCollider);

        GameObject launchPoint = new GameObject("Spear_Launch_Point");
        launchPoint.transform.parent = player.transform;
        launchPoint.transform.localPosition = new Vector3(0f, 0.5f, 0.8f);
        controller.spearLaunchPoint = launchPoint.transform;

        // Thrown-spear prefab: built once with Rigidbody + Collider by
        // EnsureSpearPrefab(), then reused on every subsequent wizard run.
        GameObject spearPrefab = EnsureSpearPrefab();
        if (spearPrefab != null) controller.spearPrefab = spearPrefab;

        // Visual-only Idà (blade) held at hip height. SwingBlade() in
        // IrekeOnibudoController does a raycast, not a physical hit off this
        // object, so this is purely cosmetic — skipped if not imported.
        GameObject bladeAsset = InstantiatePropModel("blade.fbx");
        if (bladeAsset != null)
        {
            bladeAsset.name = "Ida_Blade_Visual";
            bladeAsset.transform.parent = player.transform;
            bladeAsset.transform.localPosition = new Vector3(0.4f, 0.9f, 0.2f);
        }

        return player;
    }

    // Three creatures drawn from the actual trials in Ìrèké Oníbùdó:
    // the flying snake that seizes a princess, the violent Wrestler-Cat
    // from the story-within-a-story, and a warrior-fish guard of
    // Arogidigba's undersea kingdom.
    private static void BuildCreatures(Transform parent)
    {
        SpawnCreature(parent, CreatureAI.CreatureType.Swift, "Flying_Snake_Ejo_Fifo",
            "Assets/Models/Characters/Flying_Snake_Ejo_Fifo",
            new Vector3(-8f, 3f, 30f), new Color(0.3f, 0.7f, 0.3f), new Vector3(2.5f, 1f, 1f));
        SpawnCreature(parent, CreatureAI.CreatureType.Brute, "Wrestler_Cat_Ologbo_Ijakadi",
            "Assets/Models/Characters/Wrestler_Cat_Ologbo_Ijakadi",
            new Vector3(10f, 0f, 50f), new Color(0.5f, 0.4f, 0.1f), new Vector3(2.5f, 2.5f, 2.5f));
        SpawnCreature(parent, CreatureAI.CreatureType.Titan, "Warrior_Fish_Eja_Jagunjagun",
            "Assets/Models/Characters/Warrior_Fish_Eja_Jagunjagun",
            new Vector3(0f, 0f, 68f), new Color(0.1f, 0.3f, 0.5f), new Vector3(2.5f, 3.5f, 4f));
    }

    private static void SpawnCreature(Transform parent, CreatureAI.CreatureType statPreset, string displayName,
        string modelFolderPath, Vector3 position, Color color, Vector3 scale)
    {
        GameObject go = InstantiateModelOrFallback(modelFolderPath, PrimitiveType.Cube, color);
        go.name = displayName;
        go.tag = "Enemy";
        go.transform.parent = parent;
        go.transform.position = position;
        go.transform.localScale = scale; // eyeball-check against real model proportions in Editor

        // CreatureAI's contact-damage check relies on the player hitting a
        // collider; the primitive fallback has one built in, but imported
        // FBX models normally don't, so add one when it's missing.
        if (go.GetComponent<Collider>() == null) go.AddComponent<BoxCollider>();

        CreatureAI ai = go.AddComponent<CreatureAI>();
        CreatureAI.ApplyPreset(ai, statPreset); // reuses ogbojuode's stat presets by role (fast/slow/tank)
    }

    // Arogidigba: deepest in the undersea kingdom, past all three creatures.
    private static void BuildArogidigba(Transform parent)
    {
        GameObject queen = new GameObject("Arogidigba_Mermaid_Queen");
        queen.transform.parent = parent;
        queen.transform.position = new Vector3(0f, 0f, 85f);

        GameObject model = LoadModelInFolder("Assets/Models/Characters/Arogidigba_Mermaid_Queen");
        if (model != null)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            instance.name = "Arogidigba_Model";
            instance.transform.parent = queen.transform;
            instance.transform.localPosition = Vector3.zero;
        }
        else
        {
            // Two-part primitive placeholder, kept as a fallback until/unless
            // the mermaid queen model is unavailable.
            GameObject upperBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            upperBody.name = "Arogidigba_UpperBody_Human";
            upperBody.transform.parent = queen.transform;
            upperBody.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            upperBody.transform.localScale = new Vector3(1.2f, 1.3f, 1.2f);
            SetColor(upperBody, new Color(0.7f, 0.5f, 0.6f));

            GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tail.name = "Arogidigba_LowerBody_FishTail";
            tail.transform.parent = queen.transform;
            tail.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            tail.transform.localScale = new Vector3(0.8f, 1.5f, 0.8f);
            SetColor(tail, new Color(0.2f, 0.5f, 0.6f));
        }

        queen.AddComponent<ArogidigbaBoss>();
        RiddleGiver riddle = queen.AddComponent<RiddleGiver>();
        riddle.riddleText = "I rule the water yet was never wet. What manner of thing am I?";
        riddle.correctAnswerHint = "a queen of two natures / Arogidigba herself";
        riddle.wisdomReward = 50;
    }

    // Three wandering visitations of Ireke Onibudo's mother's spirit,
    // scattered through the undersea kingdom rather than guarding the path.
    private static void BuildMotherSpiritGuides(Transform parent)
    {
        Vector3[] positions =
        {
            new Vector3(-15f, 1f, 22f),
            new Vector3(18f, 1f, 40f),
            new Vector3(-5f, 1f, 58f),
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject guide = InstantiateModelOrFallback(
                "Assets/Models/Characters/Mother_Spirit_Guide_Iya_Ireke",
                PrimitiveType.Sphere, new Color(0.85f, 0.85f, 1f));
            guide.name = "Mother_Spirit_Guide_" + i;
            guide.transform.parent = parent;
            guide.transform.position = positions[i];
            guide.transform.localScale = Vector3.one * 0.8f;

            guide.AddComponent<MotherSpiritGuide>();
            RiddleGiver riddle = guide.AddComponent<RiddleGiver>();
            riddle.riddleText = "What follows you even into the deepest water, unseen?";
            riddle.correctAnswerHint = "a mother's love / memory";
            riddle.wisdomReward = 10;
        }
    }

    private static void BuildManagers(Transform parent, Transform playerTransform)
    {
        GameObject managers = new GameObject("Managers");
        managers.transform.parent = parent;

        managers.AddComponent<WisdomTracker>();

        ExpeditionManager expedition = managers.AddComponent<ExpeditionManager>();
        expedition.player = playerTransform;
        expedition.villageBoundaryZ = 12f; // matches the dock/reed-barrier line above
    }

    private static void BuildCameraAndLighting(Transform playerTransform)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.SetParent(playerTransform);
            cam.transform.localPosition = new Vector3(0f, 6f, -8f);
            cam.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
        }

        GameObject deepLight = new GameObject("Deep_Water_Light");
        Light light = deepLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.3f, 0.5f, 0.7f);
        light.intensity = 0.5f;
        deepLight.transform.rotation = Quaternion.Euler(70f, -20f, 0f);
    }

    private static void SetColor(GameObject go, Color color)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r == null) return;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = color;
        r.sharedMaterial = mat;
    }
}
