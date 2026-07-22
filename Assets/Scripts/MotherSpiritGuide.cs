using UnityEngine;

// Ìyá Ìrèké: the hero's dead mother, who visits him in dreams and returns
// to help him survive Arogidigba's trials — per the actual source material,
// this is his real guide through the undersea kingdom, not a minor NPC.
// Same wandering-spirit pattern as GhommidSpirit.cs in ogbojuode-3d, but
// framed as guidance rather than mischief.
[RequireComponent(typeof(RiddleGiver))]
public class MotherSpiritGuide : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderSpeed = 1.0f;

    private Vector3 homePosition;
    private Vector3 wanderTarget;

    void Start()
    {
        homePosition = transform.position;
        PickNewWanderTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, wanderSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, wanderTarget) < 0.2f)
            PickNewWanderTarget();
    }

    private void PickNewWanderTarget()
    {
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        wanderTarget = homePosition + new Vector3(offset.x, 0f, offset.y);
    }
}
