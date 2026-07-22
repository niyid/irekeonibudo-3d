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
// Builds entirely from primitives for now — no models imported yet. See
// irekeonibudo-meshy_prompts.txt for the asset list to generate before
// swapping these placeholders out, following the same
// load-real-asset-with-fallback pattern used in ogbojuode-3d once assets
// exist.
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

    // --- Undersea kingdom (Ijoba Omi): the dangerous expanse, standing in
    // for the forest — reef instead of trees, glowing plankton instead of
    // spirit fungus. ---
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
            new Vector3(-10f, 1.5f, -5f),
            new Vector3(10f, 1.5f, -5f),
            new Vector3(-10f, 1.5f, 8f),
            new Vector3(10f, 1.5f, 8f),
        };
        foreach (Vector3 pos in hutPositions)
        {
            GameObject hut = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hut.name = "Riverside_Hut";
            hut.transform.parent = hub.transform;
            hut.transform.position = pos;
            hut.transform.localScale = new Vector3(2.5f, 1.5f, 2.5f);
            SetColor(hut, new Color(0.6f, 0.4f, 0.2f));

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Thatched_Roof";
            roof.transform.parent = hut.transform;
            roof.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            roof.transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
            roof.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            SetColor(roof, new Color(0.4f, 0.3f, 0.1f));
        }

        GameObject dock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dock.name = "Riverside_Dock";
        dock.transform.parent = hub.transform;
        dock.transform.position = new Vector3(0f, 0.3f, 14f);
        dock.transform.localScale = new Vector3(4f, 0.3f, 6f);
        SetColor(dock, new Color(0.35f, 0.25f, 0.15f));

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

        for (int i = 0; i < 10; i++)
        {
            GameObject reed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            reed.name = "Reed_Barrier_" + i;
            reed.transform.parent = hub.transform;
            float x = Mathf.Lerp(-20f, 20f, i / 9f);
            reed.transform.position = new Vector3(x, 0.6f, 12f);
            reed.transform.localScale = new Vector3(0.1f, 0.6f, 0.1f);
            SetColor(reed, new Color(0.5f, 0.55f, 0.2f));
        }
    }

    private static GameObject BuildPlayer(Transform parent)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player_Ireke_Onibudo";
        player.tag = "Player";
        player.transform.parent = parent;
        player.transform.position = new Vector3(0f, 1f, -10f);
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerVitals>();
        IrekeOnibudoController controller = player.AddComponent<IrekeOnibudoController>();
        SetColor(player, new Color(0.5f, 0.3f, 0.15f));

        GameObject launchPoint = new GameObject("Spear_Launch_Point");
        launchPoint.transform.parent = player.transform;
        launchPoint.transform.localPosition = new Vector3(0f, 0.5f, 0.8f);
        controller.spearLaunchPoint = launchPoint.transform;

        return player;
    }

    // Three creatures drawn from the actual trials in Ìrèké Oníbùdó:
    // the flying snake that seizes a princess, the violent Wrestler-Cat
    // from the story-within-a-story, and a warrior-fish guard of
    // Arogidigba's undersea kingdom.
    private static void BuildCreatures(Transform parent)
    {
        SpawnCreature(parent, CreatureAI.CreatureType.Swift, "Flying_Snake_Ejo_Fifo",
            new Vector3(-8f, 3f, 30f), new Color(0.3f, 0.7f, 0.3f), new Vector3(2.5f, 1f, 1f));
        SpawnCreature(parent, CreatureAI.CreatureType.Brute, "Wrestler_Cat_Ologbo_Ijakadi",
            new Vector3(10f, 0f, 50f), new Color(0.5f, 0.4f, 0.1f), new Vector3(2.5f, 2.5f, 2.5f));
        SpawnCreature(parent, CreatureAI.CreatureType.Titan, "Warrior_Fish_Eja_Jagunjagun",
            new Vector3(0f, 0f, 68f), new Color(0.1f, 0.3f, 0.5f), new Vector3(2.5f, 3.5f, 4f));
    }

    private static void SpawnCreature(Transform parent, CreatureAI.CreatureType statPreset, string displayName,
        Vector3 position, Color color, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = displayName;
        go.tag = "Enemy";
        go.transform.parent = parent;
        go.transform.position = position;
        go.transform.localScale = scale;
        CreatureAI ai = go.AddComponent<CreatureAI>();
        CreatureAI.ApplyPreset(ai, statPreset); // reuses ogbojuode's stat presets by role (fast/slow/tank)
        SetColor(go, color);
    }

    // Arogidigba: deepest in the undersea kingdom, past all three creatures.
    private static void BuildArogidigba(Transform parent)
    {
        GameObject queen = new GameObject("Arogidigba_Mermaid_Queen");
        queen.transform.parent = parent;
        queen.transform.position = new Vector3(0f, 0f, 85f);

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
            GameObject guide = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            guide.name = "Mother_Spirit_Guide_" + i;
            guide.transform.parent = parent;
            guide.transform.position = positions[i];
            guide.transform.localScale = Vector3.one * 0.8f;
            SetColor(guide, new Color(0.85f, 0.85f, 1f));

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
