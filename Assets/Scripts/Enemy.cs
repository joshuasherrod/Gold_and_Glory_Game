using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Set HP
    public int health = 50;
    // Set Damage inflicted to player upon contact
    public int contactDamage = 25;
    // Init animator
    private Animator animator;

    // Patrol points & speed
    public Transform patrolLeft;
    public Transform patrolRight;
    public float patrolSpeed = 2f;

    // Movement & appearance components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool movingRight = true;

    // Blink red when taking damage
    private Coroutine flashCoroutine;
    private Color originalColor;

    // On death, drop coins
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoinsDropped = 1;
    [SerializeField] private int maxCoinsDropped = 3;

    // Death sound effect
    [Header("Enemy Sounds")]
    [SerializeField] private AudioClip deathSound;

    // Runs when game starts
    private void Awake()
    {
        // Get enemy's Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Get enemy's animator component
        animator = GetComponent<Animator>();

        // Get enemy's spriteRenderer component
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Save enemy's original sprite color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Remove health when player attacks enemy
    public void TakeDamage(int damage)
    {
        // Subtract damage from enemy's health
        health -= damage;

        // Flash red when taking damage
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRed());


        // Die when health reaches 0
        if (health <= 0)
        {
            Die();
        }
    }

    
    private void SetAnimation()
    {
        animator.Play("Enemy1_Idle");
    }

    private void FixedUpdate()
    {
        // Stop if no patrol points
        if (patrolLeft == null || patrolRight == null)
            return;

        // Find left patrol point
        float leftX = Mathf.Min(
            patrolLeft.position.x,
            patrolRight.position.x
        );

        // Find right patrol point
        float rightX = Mathf.Max(
            patrolLeft.position.x,
            patrolRight.position.x
        );

        // Turn around when it reaches patrol points
        if (movingRight && transform.position.x >= rightX)
        {
            movingRight = false;
            spriteRenderer.flipX = true;
        }
        else if (!movingRight && transform.position.x <= leftX)
        {
            movingRight = true;
            spriteRenderer.flipX = false;
        }

        // Set horizontal movement direction
        float direction = movingRight ? 1f : -1f;

        // Horizontal movement & patrol speed
        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );
    }

    // Flash red when taking damage
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    // Enemy death
    private void Die()
    {
        // Drop coind if coin prefab is assigned
        if (coinPrefab != null)
        {
            // Choose random amount of coins from 1-3 to drop
            int coinsToDrop = Random.Range(
                minCoinsDropped,
                maxCoinsDropped + 1);

            // Create dropped coins
            for (int i = 0; i < coinsToDrop; i++)
            {
                GameObject droppedCoin = Instantiate(
                    coinPrefab,
                    transform.position,
                    Quaternion.identity);

                Rigidbody2D coinRb = 
                    droppedCoin.GetComponent<Rigidbody2D>();

                // Boing boing
                if (coinRb != null)
                {
                    float horizontalVelocity = Random.Range(-2f, 2f);
                    float upwardVelocity = Random.Range(3f, 6f);
                    coinRb.linearVelocity = new Vector2(
                        horizontalVelocity,
                        upwardVelocity);
                    
                }
            }
            // Death soi=und effect
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    deathSound,
                    Camera.main.transform.position,
                    20f);
            }
        }
        // Delete enemy after death
        Destroy(gameObject);
    }
}
