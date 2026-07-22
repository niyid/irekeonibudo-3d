using System.Collections.Generic;
using UnityEngine;

// Minimal "Ase" charm system: charms are data, not hardcoded behavior, so
// either sibling project's charm enum/set (ogbojuode-3d's or
// irekeonibudo-3d's) can plug straight into this without a rewrite.
[System.Serializable]
public class Charm
{
    public string charmName;
    public string description;
    public bool isActive;
}

public class PlayerVitals : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    public int currentHealth;

    public List<Charm> equippedCharms = new List<Charm>();

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private bool isDefeated = false;

    public void TakeDamage(int amount)
    {
        if (isDefeated) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0)
        {
            isDefeated = true;
            HandleDefeat();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public bool HasCharm(string name)
    {
        return equippedCharms.Exists(c => c.charmName == name && c.isActive);
    }

    private void HandleDefeat()
    {
        Debug.Log("The traveler has fallen. Returning to the hub village.");
        // Hook your respawn-at-hub / game-over flow here.
    }

    // Call this once your respawn-at-hub flow repositions the player, so
    // TakeDamage stops ignoring hits and the traveler can be hurt again.
    public void Respawn()
    {
        currentHealth = maxHealth;
        isDefeated = false;
    }
}
