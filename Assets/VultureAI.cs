using UnityEngine;

public class VultureAI : EnemyAI
{
    [Header("Vulture Settings")]
    public float idleMoveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float detectionRange = 10f;
    [SerializeField] private int damageToPlayer = 1;

    private Transform player;
    private Vector2 idleDirection;
    private float idleTimer;
    private float idleChangeTime = 2f;
    private bool isChasing = false;

    // Using Awake here to initialize references early.
    protected override void Awake()
    {
        base.Awake();
        // Find the player by tag.
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        // Initialize the idle movement
        PickNewIdleDirection();
    }

    void Update()
    {
        if (isDead || player == null)
            return;

        // Determine whether the player is within detection range.
        float distance = Vector2.Distance(transform.position, player.position);
        isChasing = distance <= detectionRange;

        // Choose movement direction based on whether we're chasing or idling.
        Vector2 moveDir = isChasing 
            ? (player.position - transform.position).normalized 
            : idleDirection;

        // Apply velocity based on the appropriate speed.
        rb.velocity = moveDir * (isChasing ? chaseSpeed : idleMoveSpeed);

        // If not chasing, update idle movement.
        if (!isChasing)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                PickNewIdleDirection();
            }
        }
    }

    // Chooses a random idle direction and resets the idle timer.
    void PickNewIdleDirection()
    {
        idleDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f)).normalized;
        idleTimer = idleChangeTime;
    }

    // When colliding with the player, inflict damage.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead && other.CompareTag("Player"))
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null)
                hp.TakeDamage(damageToPlayer);
        }
    }

    // This method resets the enemy state. It is called automatically via OnEnable in EnemyAI.
    public override void ResetEnemy()
    {
        base.ResetEnemy();
        // Reset vulture-specific behavior.
        PickNewIdleDirection();
        isChasing = false;
    }
}
