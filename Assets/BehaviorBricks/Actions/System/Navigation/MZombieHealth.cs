using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages zombie health, damage reactions, state transitions, and death/ragdoll behavior.
/// Follows strict priority order: Dead > Crawling > Falling > normal states.
/// </summary>
public class ZombieHealth : MonoBehaviour, IDamageable
{
    // ─── Public State Flags (read by Behavior Tree actions) ───
    [Header("Zombie State (Read Only at Runtime)")]
    public bool IsDead = false;
    public bool IsCrawling = false;
    public bool FirstShotTriggered = false;
    private AudioSource zombieAudio;

    // ─── Configuration ───
    [Header("Settings")]
    [SerializeField] private int hitsToTriggerFall;
    [SerializeField] private int hitsToDie;

    // ─── Private State ───
    private int hitCount = 0;

    // ─── Cached Components (Rule 1: cache in Start, null-check before use) ───
    private Animator animator;
    private NavMeshAgent navAgent;
    private BehaviorExecutor behaviorExecutor;

    // ═══════════════════════════════════════════════════════════
    //  INITIALIZATION
    // ═══════════════════════════════════════════════════════════

    private void Start()
    {
        CacheComponents();
        ValidateComponents();

        if (zombieAudio != null)
        {
            zombieAudio.loop = true;
            zombieAudio.Play();
        }
    }

    private void CacheComponents()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        behaviorExecutor = GetComponent<BehaviorExecutor>();
        zombieAudio = GetComponent<AudioSource>();
    }

    private void ValidateComponents()
    {
        if (animator == null)
            Debug.LogWarning($"[ZombieHealth] Animator missing on {gameObject.name}");
        if (navAgent == null)
            Debug.LogWarning($"[ZombieHealth] NavMeshAgent missing on {gameObject.name}");
        if (behaviorExecutor == null)
            Debug.LogWarning($"[ZombieHealth] BehaviorExecutor missing on {gameObject.name}");
    }

    // ═══════════════════════════════════════════════════════════
    //  DAMAGE HANDLING (Rule 7: Damage triggers state changes)
    // ═══════════════════════════════════════════════════════════


    /// <summary>
    /// Called by Bullet.cs when a bullet hits this zombie.
    /// First hit → Fall. Second hit → Death.
    /// </summary>

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        for (int i = 0; i < amount; i++)
        {
            TakeHit();
        }
    }

    public void TakeHit()
    {
        if (IsDead) return;

        hitCount++;
        Debug.Log($"[ZombieHealth] {gameObject.name} took hit #{hitCount}");

        // While crawling, only die after enough hits
        if (IsCrawling)
        {
            if (hitCount >= hitsToDie)
            {
                Die();
            }
            return;
        }

        if (hitCount >= hitsToDie)
        {
            Die();
        }
        else if (hitCount >= hitsToTriggerFall)
        {
            TriggerFall();
        }
    }


    // ═══════════════════════════════════════════════════════════
    //  STATE TRANSITIONS
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Triggers the falling state. Resets all animation booleans and stops navigation.
    /// The CheckAndFall behavior tree action handles the actual fall animation.
    /// </summary>
    private void TriggerFall()
    {
        FirstShotTriggered = true;

        ResetAllAnimationStates();
        StopNavigation();

        // Trigger the fall animation immediately
        if (animator != null)
        {
            animator.SetTrigger("fallTrigger");
        }

        Debug.Log($"[ZombieHealth] {gameObject.name} is falling! fallTrigger set.");
    }

    /// <summary>
    /// Called by CheckAndFall action after the fall animation completes.
    /// Transitions the zombie from falling into crawling state.
    /// </summary>
    public void StartCrawling()
    {
        FirstShotTriggered = false;
        IsCrawling = true;
        hitCount = 0;

        if (animator != null)
            animator.SetBool("isCrawling", true);

        Debug.Log($"[ZombieHealth] {gameObject.name} is now crawling!");
    }

    // ═══════════════════════════════════════════════════════════
    //  DEATH & RAGDOLL (Rule 10: disable AI, enable ragdoll)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Kills the zombie: stops all AI systems and activates ragdoll physics.
    /// </summary>
    private void Die()
    {
        IsDead = true;
        IsCrawling = false;
        FirstShotTriggered = false;

        if (zombieAudio != null)
            zombieAudio.Stop();

        // Stop AI first
        DisableBehaviorTree();
        DisableNavigation();

        // Disable animator safely
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.enabled = false;
        }

        ActivateRagdoll();

        Debug.Log($"[ZombieHealth] {gameObject.name} is dead! Ragdoll activated.");
    }

    // ═══════════════════════════════════════════════════════════
    //  HELPER METHODS (Rule 12: modular, no duplicated logic)
    // ═══════════════════════════════════════════════════════════

    private void ResetAllAnimationStates()
    {
        if (animator == null) return;

        animator.SetBool("isChasing", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isCrawling", false);
    }

    private void StopNavigation()
    {
        if (navAgent != null && navAgent.isOnNavMesh)
            navAgent.isStopped = true;
    }

    private void DisableBehaviorTree()
    {
        if (behaviorExecutor != null)
            behaviorExecutor.enabled = false;
    }

    private void DisableAnimator()
    {
        if (animator != null)
            animator.enabled = false;
    }

    private void DisableNavigation()
    {
        if (navAgent != null)
        {
            if (navAgent.isOnNavMesh)
            {
                navAgent.isStopped = true;
                navAgent.ResetPath();
            }

            navAgent.enabled = false;
        }
    }

    private void ActivateRagdoll()
    {
        // Disable the root collider
        Collider rootCollider = GetComponent<Collider>();
        if (rootCollider != null)
            rootCollider.enabled = false;

        // Get ragdoll rigidbodies
        Rigidbody[] ragdollBodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in ragdollBodies)
        {
            // Reset all physics velocity to prevent flying
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Enable ragdoll colliders
        Collider[] ragdollColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != gameObject)
                col.enabled = true;
        }

        // Prevent ragdoll parts from colliding with each other
        for (int i = 0; i < ragdollColliders.Length; i++)
        {
            for (int j = i + 1; j < ragdollColliders.Length; j++)
            {
                Physics.IgnoreCollision(ragdollColliders[i], ragdollColliders[j]);
            }
        }
    }
}