using UnityEngine;
using System.Collections;

public class GideonMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public GameObject hatPrefab;
    public Transform throwPoint;

    // Sound effect for hat throw.
    public AudioClip hatThrowSound;

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;
    public Vector2 lastMoveDirection = Vector2.right;
    public bool isFacingRight { get; private set; } = true;

    private float throwCooldown = 0.75f;
    private float lastThrowTime = -999f;
    private bool isThrowing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Ensure you have an AudioSource on this GameObject.
    }

    void Update()
    {
        if (!isThrowing)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            movement = new Vector2(moveX, moveY).normalized;

            // Update facing direction
            if (moveX > 0)
                isFacingRight = true;
            else if (moveX < 0)
                isFacingRight = false;

            if (movement != Vector2.zero)
                lastMoveDirection = movement;

            // Update animator parameters.
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetBool("IsMoving", movement != Vector2.zero);
            animator.SetBool("IsFacingRight", isFacingRight);
        }

        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= lastThrowTime + throwCooldown)
        {
            lastThrowTime = Time.time;
            StartCoroutine(PlayThrowAnimation());
        }
    }

    void FixedUpdate()
    {
        if (!isThrowing)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void ThrowHat()
    {
        GameObject hat = Instantiate(hatPrefab, throwPoint.position, Quaternion.identity);
        float direction = isFacingRight ? 1f : -1f;

        HatProjectile hatScript = hat.GetComponent<HatProjectile>();
        if (hatScript != null)
            hatScript.SetDirection(new Vector2(direction, 0));

        // Play hat throw sound effect.
        if (audioSource != null && hatThrowSound != null)
            audioSource.PlayOneShot(hatThrowSound);
    }

    IEnumerator PlayThrowAnimation()
    {
        isThrowing = true;
        string animName = isFacingRight ? "Gideon_Throw_Right" : "Gideon_throw_hat_left";
        animator.Play(animName);

        yield return new WaitForSeconds(0.15f);
        ThrowHat();

        yield return new WaitForSeconds(0.25f);
        isThrowing = false;

        if (movement != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.Play("Gideon_Idle");
        }
    }
}
