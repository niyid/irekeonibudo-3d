using UnityEngine;

// Mirrors the book's structure: Ireke Onibudo is swept from safety into
// Arogidigba's undersea kingdom and must find his way back. This doesn't
// gate anything by itself (no lockouts) — it just tracks state and logs
// transitions, so you can hook UI, music changes, or an "expedition
// complete" summary onto it later.
public class ExpeditionManager : MonoBehaviour
{
    public static ExpeditionManager Instance { get; private set; }

    public Transform player;

    // Replaces the old standalone villageBoundaryZ float — SceneSetupWizard
    // now assigns this once from WorldBounds.IrekeOnibudoDefaults, and the
    // same struct is what BuildCreatures/BuildMotherSpiritGuides use for
    // spawn placement, so the boundary and the spawn band can't silently
    // drift apart.
    public WorldBounds bounds = WorldBounds.IrekeOnibudoDefaults;

    public bool InDangerZone { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (player == null) return;

        bool nowInDangerZone = player.position.z > bounds.villageBoundaryZ;
        if (nowInDangerZone != InDangerZone)
        {
            InDangerZone = nowInDangerZone;
            if (InDangerZone)
                Debug.Log("The current pulls him under. The riverside village falls behind.");
            else
            {
                Debug.Log($"Returned to the riverside hub. Wisdom carried: {(WisdomTracker.Instance != null ? WisdomTracker.Instance.currentWisdom : 0)}");
                // Natural checkpoint: save whenever the player makes it back
                // to safety, rather than requiring an explicit save menu yet.
                WisdomTracker.Instance?.Save();
            }
        }
    }
}
