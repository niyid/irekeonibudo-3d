using System.Collections;
using UnityEngine;

// Same movement/combat/teleport skeleton as YorubaHunterController in
// ogbojuode-3d, reflavored: Ireke Onibudo is a traveler, not a hunter, so
// the ranged weapon is a thrown fishing spear rather than a musket, and the
// melee weapon is a plain blade rather than a hunting machete. Egbe carries
// over unchanged — it's a general Yoruba metaphysical concept, not specific
// to either story.
[RequireComponent(typeof(CharacterController))]
public class IrekeOnibudoController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6.0f;
    public float rotationSpeed = 720.0f;
    public float gravity = -9.81f;

    [Header("Thrown Spear (Ọ̀kọ̀ Ẹja)")]
    public Transform spearLaunchPoint;
    public GameObject spearPrefab;
    public float spearThrowRate = 1.2f;
    public float spearSpeed = 25f;
    private float nextThrowTime = 0f;

    [Header("Egbe Spell (Teleport)")]
    public float egbeDistance = 8.0f;
    public float egbeCooldown = 3.0f;
    public ParticleSystem egbeSmokeFX;
    private float nextEgbeTime = 0f;

    [Header("Blade (Idà)")]
    public float meleeRange = 2.0f;
    public int meleeDamage = 25;
    private bool isAttacking = false;

    // Mobile input support - set by touch UI, left at zero for keyboard/editor testing
    public Vector2 MobileMoveInput = Vector2.zero;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleCombatInput();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal") + MobileMoveInput.x;
        float v = Input.GetAxis("Vertical") + MobileMoveInput.y;
        Vector3 inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        if (controller.isGrounded)
        {
            velocity = inputDir * moveSpeed;
            if (inputDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCombatInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
            StartCoroutine(SwingBlade());

        if (Input.GetMouseButtonDown(1) && Time.time >= nextThrowTime)
            ThrowSpear();

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextEgbeTime)
            CastEgbe();
    }

    private IEnumerator SwingBlade()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.25f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, meleeRange))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null) target.TakeDamage(meleeDamage);
        }

        yield return new WaitForSeconds(0.15f);
        isAttacking = false;
    }

    public void ThrowSpear()
    {
        if (Time.time < nextThrowTime) return;
        nextThrowTime = Time.time + spearThrowRate;

        if (spearLaunchPoint != null && spearPrefab != null)
        {
            GameObject spear = Instantiate(spearPrefab, spearLaunchPoint.position, spearLaunchPoint.rotation);
            Rigidbody rb = spear.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = spearLaunchPoint.forward * spearSpeed;
            Destroy(spear, 3f);
        }
    }

    public void CastEgbe()
    {
        if (Time.time < nextEgbeTime) return;
        nextEgbeTime = Time.time + egbeCooldown;

        if (egbeSmokeFX != null) Instantiate(egbeSmokeFX, transform.position, Quaternion.identity);

        Vector3 destination = transform.position + transform.forward * egbeDistance;
        controller.enabled = false;
        transform.position = destination;
        controller.enabled = true;

        if (egbeSmokeFX != null) Instantiate(egbeSmokeFX, transform.position, Quaternion.identity);
    }

    // Called by on-screen mobile buttons
    public void TriggerBladeAttackButton()
    {
        if (!isAttacking) StartCoroutine(SwingBlade());
    }
}
