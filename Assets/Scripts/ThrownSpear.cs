using UnityEngine;

// Was missing entirely: IrekeOnibudoController.ThrowSpear() and
// SceneSetupWizard.EnsureSpearPrefab() built a spear with a Rigidbody and a
// trigger Collider, but nothing in the project ever read that trigger, so
// the ranged weapon flew through enemies without dealing any damage.
//
// Attach to the ThrownSpear prefab (EnsureSpearPrefab does this
// automatically). On trigger-enter with anything IDamageable other than the
// thrower, deals damage once and destroys the spear so it can't hit the same
// or another target again.
public class ThrownSpear : MonoBehaviour
{
    public int damage = 20;

    // Set by IrekeOnibudoController.ThrowSpear() right after Instantiate so
    // the spear doesn't damage the player who just threw it.
    [HideInInspector] public GameObject thrower;

    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (thrower != null && other.transform.IsChildOf(thrower.transform)) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            hasHit = true;
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
