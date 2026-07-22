using UnityEngine;

// Arògìdígbà: the mermaid queen of the undersea kingdom — half-human,
// half-fish, ruler of the fish-folk who captured Ireke Onibudo. In the
// source material she sets a string of trials designed for failure. Same
// pattern as OstrichKingBoss.cs in ogbojuode-3d: she holds ground and
// poses a riddle rather than chasing; only becomes hostile if the riddle
// (trial) is failed outright.
[RequireComponent(typeof(RiddleGiver))]
public class ArogidigbaBoss : MonoBehaviour, IDamageable
{
    public int maxHealth = 250;
    public int contactDamage = 25;
    public float aggroRange = 6f;

    private int currentHealth;
    private Transform player;
    private bool trialFailed = false;
    private float nextContactHitTime;
    private const float contactHitCooldown = 1.0f;

    void Start()
    {
        currentHealth = maxHealth;
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null || !trialFailed) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= 2.5f && Time.time >= nextContactHitTime)
        {
            nextContactHitTime = Time.time + contactHitCooldown;
            IDamageable target = player.GetComponent<IDamageable>();
            if (target != null) target.TakeDamage(contactDamage);
        }
    }

    // Called by RiddleGiver when the player fails the trial outright.
    public void OnRiddleFailed()
    {
        trialFailed = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
