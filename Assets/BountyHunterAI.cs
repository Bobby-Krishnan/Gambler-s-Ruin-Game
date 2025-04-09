using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class BountyHunterAI : EnemyAI
{
    [Header("Movement Settings")]
    public float idleMoveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 8f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;

    private float shootTimer = 0f;
    private Transform player;
    private Vector2 idleDirection;
    private float idleTimer;
    private float idleChangeTime = 2f;
    private bool isChasing = false;

    // Use Awake to initialize references as soon as the object is created.
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Find the player early.
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
        
        // Initialize idle movement.
        PickNewIdleDirection();
    }

    // OnEnable is called every time the enemy becomes active,
    // such as after a full scene reload.
    protected override void OnEnable()
    {
        base.OnEnable();
        shootTimer = 0f;
        PickNewIdleDirection();
        // Ensure player reference is still valid.
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

        // Check distance to player to determine if chasing or idle.
        float dist = Vector2.Distance(transform.position, player.position);
        isChasing = dist <= detectionRange;

        Vector2 moveDir = isChasing ? (player.position - transform.position).normalized : idleDirection;
        float speed = isChasing ? chaseSpeed : idleMoveSpeed;

        rb.velocity = moveDir * speed;
        UpdateAnimation(moveDir);

        // Handle shooting when chasing.
        if (isChasing)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                ShootAtPlayer(moveDir);
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

    // Sets a new idle direction and resets the idle timer.
    void PickNewIdleDirection()
    {
        idleDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        idleTimer = idleChangeTime;
    }

    // Updates animation parameters based on movement direction.
    void UpdateAnimation(Vector2 dir)
    {
        animator.SetBool("IsMoving", dir.magnitude > 0.1f);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            animator.SetTrigger(dir.x > 0 ? "WalkRight" : "WalkLeft");
        else
            animator.SetTrigger(dir.y > 0 ? "WalkUp" : "WalkDown");
    }

    // Spawns and directs a bullet toward the player.
    void ShootAtPlayer(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        UniversalBullet bulletScript = bullet.GetComponent<UniversalBullet>();
        if (bulletScript != null)
            bulletScript.SetDirection(direction, "Player");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetTrigger(direction.x > 0 ? "ShootRight" : "ShootLeft");
        else
            animator.SetTrigger(direction.y > 0 ? "ShootUp" : "ShootDown");
    }

    // ResetEnemy is called from OnEnable (inherited from EnemyAI)
    // to restore default behavior on scene reload.
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        shootTimer = 0f;
        PickNewIdleDirection();
    }

    // Override Die() to play the death animation and then keep the body on the map.
    public override void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (rb != null)
            rb.velocity = Vector2.zero;
        if (TryGetComponent<Collider2D>(out var col))
            col.enabled = false;

        if (hasDeathAnimation && animator != null)
        {
            animator.SetTrigger("Die");
            // Wait for the animation to complete, then disable AI processing.
            StartCoroutine(WaitAndKeepDead());
        }
        else
        {
            StartCoroutine(WaitAndKeepDead());
        }
    }

    // Waits a short amount of time, then disables this script so the bounty hunter remains dead.
    IEnumerator WaitAndKeepDead()
    {
        yield return new WaitForSeconds(1f); 
        this.enabled = false; // Disables further AI logic, leaving the GameObject visible.
    }
}
