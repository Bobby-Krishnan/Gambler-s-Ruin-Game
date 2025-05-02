using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class LucienBossAI : EnemyAI
{
    [Header("Movement Settings")]
    public float idleMoveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float detectionRange = 10f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;

    private float shootTimer = 0f;
    private Transform player;
    private Vector2 idleDirection;
    private float idleTimer;
    private float idleChangeTime = 2f;
    private bool isChasing = false;
    private bool isShooting = false;

    // Initialize references as early as possible.
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Find the player by tag.
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Set an initial idle direction.
        PickNewIdleDirection();
    }

    // OnEnable is called each time the boss becomes active (such as after a full scene reload).
    protected override void OnEnable()
    {
        base.OnEnable();
        shootTimer = 0f;
        isShooting = false;
        PickNewIdleDirection();

        // Ensure the player reference is valid.
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        // Determine if the boss should be chasing the player.
        float dist = Vector2.Distance(transform.position, player.position);
        isChasing = dist <= detectionRange;

        if (!isShooting)
        {
            Vector2 moveDir = isChasing ? (player.position - transform.position).normalized : idleDirection;
            float speed = isChasing ? chaseSpeed : idleMoveSpeed;
            rb.velocity = moveDir * speed;

            UpdateAnimation(moveDir);

            if (isChasing)
            {
                shootTimer -= Time.deltaTime;
                if (shootTimer <= 0f)
                {
                    StartCoroutine(ShootWithAnimation(moveDir));
                    shootTimer = shootCooldown;
                }
            }
            else
            {
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f)
                    PickNewIdleDirection();
            }
        }
    }

    // Sets a new idle direction and resets the idle timer.
    void PickNewIdleDirection()
    {
        idleDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        idleTimer = idleChangeTime;
    }

    // Updates the animation based on the movement direction.
    void UpdateAnimation(Vector2 dir)
    {
        animator.SetBool("IsMoving", dir.magnitude > 0.1f);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            animator.SetTrigger(dir.x > 0 ? "WalkRight" : "WalkLeft");
        else
            animator.SetTrigger(dir.y > 0 ? "WalkUp" : "WalkDown");
    }

    // Shoots at the player with an animation sequence.
    IEnumerator ShootWithAnimation(Vector2 dir)
    {
        isShooting = true;
        rb.velocity = Vector2.zero;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            animator.SetTrigger(dir.x > 0 ? "ShootRight" : "ShootLeft");
        else
            animator.SetTrigger(dir.y > 0 ? "ShootUp" : "ShootDown");

        yield return new WaitForSeconds(0.25f);

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            UniversalBullet bulletScript = bullet.GetComponent<UniversalBullet>();
            if (bulletScript != null)
                bulletScript.SetDirection(dir, "Player");
        }

        yield return new WaitForSeconds(0.2f);
        isShooting = false;
    }

    // Override Die() to ensure death animation plays and the boss remains visible after dying.
    public override void Die()
    {
        if (isDead) return;

        isDead = true;
        if (rb != null)
            rb.velocity = Vector2.zero;
        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        if (hasDeathAnimation && animator != null)
        {
            animator.SetTrigger("Die");
            // Wait for the death animation to complete, then disable this script to freeze his state.
            StartCoroutine(WaitAndKeepDead());
        }
        else
        {
            StartCoroutine(WaitAndKeepDead());
        }
    }

    // Waits a few seconds to allow the death animation to play, then disables further AI processing
    // without deactivating the GameObject.
    IEnumerator WaitAndKeepDead()
    {
        yield return new WaitForSeconds(3f); 
        this.enabled = false; 
    }

    // Resets the boss to its default state. This is automatically called when the object is enabled.
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        shootTimer = 0f;
        isShooting = false;
        PickNewIdleDirection();
    }
}
