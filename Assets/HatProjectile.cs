using UnityEngine;

public class HatProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 1; // Set hat damage (adjust in Inspector)
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        Destroy(gameObject, 3f); // Auto clean-up
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Deal damage to enemies with Health component
        if (other.CompareTag("Enemy"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
    }
}
