using System.Collections;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    // Ground layer
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool onGround;

    // Movement
    public float moveSpeed = 5f;
    public float jump = 10f;
    private Rigidbody2D rb;
    private float moveInput;

    // Animations
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Health & UI
    public int health = 100;
    // Gain a heart when player collects 50 coins
    [SerializeField] private HealthUI healthUI;
    [SerializeField] private GameManager gameManager;

    // Attack
    public Transform attack;
    public float attackRange = 0.5f;
    public int attackDamage = 25;
    public LayerMask enemyLayer;
    private bool isAttacking;

    // Coins
    public int coinCount = 0;
    public TMP_Text coinCountText;
    
    // Player sound effects
    [Header("Player Sounds")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip jumpSound;
    // [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip receiveDamageSound;

    // private float walkSoundTimer;
    // [SerializeField] private float walkSoundInterval = 0.35f;

    // DoubleJump!
    public int maxJumps = 2;
    private int jumpsRemaining;


    void Start()
    {
        // Get player's Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Preventplayer from rotating upon collision
        rb.freezeRotation = true;

        // Give player max jumps (2)
        jumpsRemaining = maxJumps;

        // Get player's Animator component
        animator = GetComponent<Animator>();

        // Get player's spriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Display player's health using heart UI
        healthUI.SetHealth(health);

        // Display player's starting coin count
        coinCountText.text = coinCount.ToString();
    }

    void Update()
    {
        // Movement
        moveInput = Input.GetAxisRaw("Horizontal");

        // Jump when pressing spacebar on ground
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
            PlaySound(jumpSound);
            jumpsRemaining--;
        }  

        // Call animations
        SetAnimation(moveInput);

        // Flip animations if running/jumping/falling to the left
        if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;

            // Flip attack transform if moving left
            attack.localPosition = new Vector3(
                -Mathf.Abs(attack.localPosition.x),
                attack.localPosition.y,
                attack.localPosition.z);
        }
        if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;

            // Attack transform if moving right
            attack.localPosition = new Vector3(
                Mathf.Abs(attack.localPosition.x),
                attack.localPosition.y,
                attack.localPosition.z);
        }

        // Attack when pressing J
        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }

        // Timer for walking sound effect
        //if (Mathf.Abs(moveInput) > 0.1f && onGround)
        //{
        //    walkSoundTimer -= Time.deltaTime;

        //    if (walkSoundTimer <= 0f)
        //    {
        //        PlaySound(walkSound);
        //        walkSoundTimer = walkSoundInterval;
        //    }
        //}
        //else
        //{
        //    walkSoundTimer = 0f;
        //}
        
    }

    private void FixedUpdate()
    {
        // Check if groundcheck circle is touching the ground
        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        // Set onGround to true if player is touching the ground
        onGround = groundCollider != null;

        // check onGround to determine if player should have double jump
        onGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer);

        // Reset player's jumpsRemaining 
        if (onGround)
        {
            jumpsRemaining = maxJumps;
        }

        // Move horizontally while jumping
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y);
    }

    // Movement aimations
    private void SetAnimation(float moveInput)
    {
        // Attack animation
        if (isAttacking)
        {
            return;
        }

        if (onGround)
        {
            // layer idle & run animations
            if(moveInput == 0)
            {
                animator.Play("Player_idle");
            }
            else
            {
                animator.Play("Player_Run");
            }

        }
        else
        {
            // Player jump & fall animations
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }

    // Called when player collides with object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If touching "damage" tagged objects, -25 HP & play sound
        if(collision.gameObject.tag == "Damage")
        {
            TakeDamage(25);
            PlaySound(receiveDamageSound);
            return;
        }

        // Look for enemy component on the object or its parents
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();

        // If player collides with enemy, take enemy's damage
        if (enemy != null)
        {
            // Take damage, play sound
            TakeDamage(enemy.contactDamage);
            PlaySound(receiveDamageSound);
            return;
        }
    }

    // Blink red when taking damage
    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    // End Game
    private void Die()
    {
        gameManager.GameOver();;
    }

    // Attack code
    private void Attack()
    {
        // Prevent player from starting another attack while
        // mid-attack
        if (isAttacking)
        {
            return;
        }
        // Player is attacking, play sound & animation
        isAttacking = true;
        animator.Play("Player_Attack");
        PlaySound(attackSound);

        // Find enemies in enemy layer that are in attack range
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attack.position,
            attackRange,
            enemyLayer);

        // Hit all enemies in range
        foreach (Collider2D enemyCollider in enemiesHit)
        {
            // Find enemy script on collider or parent object
            Enemy enemy = enemyCollider.GetComponentInParent<Enemy>();

            // Deal damage to enemy if component was found
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }

        StartCoroutine(EndAttack());
    }

    // Attack duration
    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    // Enemies blink red when being hit
    private void OnDrawGizmosSelected()
    {
        if (attack == null)
        return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack.position, attackRange);
    }

    // Take damage when hit by enemies or spikes
    public void TakeDamage (int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);

        // Update health
        healthUI.SetHealth(health);

        // Knockback & make character blink red
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
        StartCoroutine(BlinkRed());

        // Die if health goes to 0
        if (health <= 0)
        {
            Die();
        }
    }

    // Track amount of coins collected
    // If player collects more than 50 coins, add one heart
    public void AddCoin(int amount)
    {
        int oldCoins = coinCount;
        coinCount += amount;

        if (coinCount / 50 > oldCoins / 50)
        {
            health = Mathf.Min(health + 25, 100);
            healthUI.SetHealth(health);
        }
        coinCountText.text = coinCount.ToString();
    }

    // Helper function for calling sound effects
    private void PlaySound(AudioClip clip)
    {
        if(clip != null)
        {
            AudioSource.PlayClipAtPoint(
                clip, 
                Camera.main.transform.position,
                10f);
        }
    }
}
