using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;

    // Indicates whether this enemy is dead.
    public bool isDead { get; protected set; } = false;

    [Tooltip("Set this to true if the enemy has a death animation")]
    public bool hasDeathAnimation = false;

    // Called when the object is first instantiated.
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Called each time the enemy is enabled (e.g., when the scene reloads).
    // This ensures the enemy starts in a reset state.
    protected virtual void OnEnable()
    {
        ResetEnemy();
    }

    // Called to handle enemy death.
    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        // Disable the collider so the enemy no longer interacts with the world.
        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        // disable the script until reset.
        this.enabled = false;

        if (hasDeathAnimation && animator != null)
        {
            animator.SetTrigger("Die");
            StartCoroutine(DeactivateAfterDelay(1f)); // Delay to allow death animation to complete.
        }
        else
        {
            // Immediately deactivate the enemy.
            gameObject.SetActive(false);
        }
    }

    // Coroutine to deactivate the enemy after a delay.
    protected virtual IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // Resets the enemy to its initial state. With a full scene reload,
    // this method is automatically called from OnEnable when the enemy is re-instantiated.
    public virtual void ResetEnemy()
    {
        isDead = false;

        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        this.enabled = true;
        gameObject.SetActive(true);

        if (hasDeathAnimation && animator != null)
        {
            animator.ResetTrigger("Die");
        }
    }
}
