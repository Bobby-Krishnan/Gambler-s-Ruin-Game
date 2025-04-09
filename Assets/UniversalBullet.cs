using UnityEngine;

public class UniversalBullet : MonoBehaviour
{
    public float speed = 6f;
    public float lifetime = 3f;
    public int damage = 1; //  Set damage dealt by bullet
    public string targetTag;

    private Vector2 direction;

    public void SetDirection(Vector2 dir, string target)
    {
        direction = dir.normalized;
        targetTag = target;
        Destroy(gameObject, lifetime); // Auto destroy after time
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it hit a valid target
        if (other.CompareTag(targetTag))
        {
            //  Deal damage if Health component exists
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        //  Destroy on walls/barriers
        else if (other.CompareTag("Wall") || other.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
    }
}
