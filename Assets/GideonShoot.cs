using UnityEngine;

public class GideonShoot : MonoBehaviour
{
    public GameObject bulletPrefabRight;  // Right-facing bullet prefab.
    public GameObject bulletPrefabLeft;   // Left-facing bullet prefab.
    public Transform firePoint;           // Where the bullet spawns from.
    public float shootCooldown = 0.5f;    // Delay between shots.

    // Sound effect for shooting.
    public AudioClip gunShotSound;

    private float shootTimer = 0f;
    private GideonMovement gideonMovement;

    public bool hasGun = false;

    void Start()
    {
        // Get reference to GideonMovement script on the same GameObject.
        gideonMovement = GetComponent<GideonMovement>();
    }

    void Update()
    {
        if (!hasGun) return;
        
        shootTimer -= Time.deltaTime;

        // Press X to shoot.
        if (Input.GetKeyDown(KeyCode.X) && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    void Shoot()
    {
        // Determine direction Gideon is facing (left or right).
        bool isFacingRight = gideonMovement.isFacingRight;

        // Choose bullet prefab and direction.
        GameObject selectedPrefab = isFacingRight ? bulletPrefabRight : bulletPrefabLeft;
        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Spawn bullet at fire point.
        GameObject bullet = Instantiate(selectedPrefab, firePoint.position, Quaternion.identity);

        // Send direction info to the bullet.
        UniversalBullet bulletScript = bullet.GetComponent<UniversalBullet>();
        if (bulletScript != null)
            bulletScript.SetDirection(shootDirection, "Enemy");

        // Play the gun shot sound effect.
        if (gunShotSound != null)
            AudioSource.PlayClipAtPoint(gunShotSound, firePoint.position);
    }
}
