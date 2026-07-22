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
    public float villageBoundaryZ = 12f; // matches the dock/reed-barrier line in SceneSetupWizard

    public bool InDangerZone { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (player == null) return;

        bool nowInDangerZone = player.position.z > villageBoundaryZ;
        if (nowInDangerZone != InDangerZone)
        {
            InDangerZone = nowInDangerZone;
            if (InDangerZone)
                Debug.Log("The current pulls him under. The riverside village falls behind.");
            else
                Debug.Log($"Returned to the riverside hub. Wisdom carried: {(WisdomTracker.Instance != null ? WisdomTracker.Instance.currentWisdom : 0)}");
        }
    }
}
