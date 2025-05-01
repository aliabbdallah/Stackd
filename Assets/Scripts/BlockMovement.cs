using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    [Header("Physics Settings")]
    public float settleThreshold = 0.1f; // How still the block needs to be to count as settled
    public float maxSettleTime = 6.0f; // Maximum time to wait for settling

    [Header("Season Physics")]
    public SeasonPhysics seasonPhysics; // Add this field

    private Rigidbody2D rb;
    private bool isDropping = false;
    private bool hasSettled = false;
    private Vector3 startPosition;
    private GameManager gameManager;
    private float settleTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;

        // Find the GameManager
        gameManager = FindObjectOfType<GameManager>();

        // Apply season-specific physics if available
        if (seasonPhysics != null)
        {
            ApplySeasonPhysics();
        }
    }

    void ApplySeasonPhysics()
    {
        // Apply settings from the scriptable object
        rb.gravityScale = seasonPhysics.gravity;
        
        // Create and apply physics material
        PhysicsMaterial2D blockMaterial = new PhysicsMaterial2D();
        blockMaterial.friction = seasonPhysics.blockFriction;
        blockMaterial.bounciness = seasonPhysics.blockBounciness;
        
        // Apply the physics material to the collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.sharedMaterial = blockMaterial;
        }
    }

    void ApplySeasonEffects()
    {
        if (seasonPhysics != null && isDropping && !hasSettled)
        {
            if (seasonPhysics.windStrength > 0)
            {
                float windForce = seasonPhysics.windStrength;
                rb.AddForce(Vector2.right * windForce, ForceMode2D.Force);
            }
            
            if (seasonPhysics.hasRandomEvents && Random.value < 0.02f)
            {
                float gustStrength = Random.Range(1f, 3f); // Minimum 1 to ensure noticeable push
                rb.AddForce(Vector2.right * gustStrength, ForceMode2D.Impulse);
            }
        }
    }

    void Update()
    {
        if (!isDropping)
        {
            float currentMoveSpeed = seasonPhysics != null ? seasonPhysics.moveSpeed : 2.0f;
            float moveDirection = Mathf.Sin(Time.time * currentMoveSpeed);
            Vector3 newPosition = startPosition;
            newPosition.x = startPosition.x + (moveDirection * 3.0f);
            transform.position = newPosition;

            // Check for player tap/click
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                DropBlock();
            }
        }
        else if (isDropping && !hasSettled)
        {
            // Apply season-specific effects during fall
            ApplySeasonEffects();
            
            // Check if the block has settled
            CheckIfSettled();
            
            // Add safety timer to force settle after max time
            settleTimer += Time.deltaTime;
            if (settleTimer > maxSettleTime)
            {
                ForceSettle();
            }
        }
    }

    void DropBlock()
    {
        isDropping = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private float stableTime = 0f;
    private float requiredStableTime = 0.5f;
    
    void CheckIfSettled()
    {
        // Check if the block has very low velocity
        if (rb.velocity.magnitude < settleThreshold && Mathf.Abs(rb.angularVelocity) < settleThreshold)
        {
            // Track how long it's been stable
            stableTime += Time.deltaTime;
            
            // If stable for the required duration, consider it settled
            if (stableTime >= requiredStableTime)
            {
                hasSettled = true;
                NotifySettled();
            }
        }
        else
        {
            stableTime = 0f;
        }
    }
    
    void ForceSettle()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        hasSettled = true;
        NotifySettled();
    }

    void NotifySettled()
    {
        // Double-check it's still settled or force it to be
        if (hasSettled)
        {
            // Stop all movement
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; // Make it static once settled
            
            // Disable this script
            this.enabled = false;

            // Tell the game manager with a small delay to ensure physics has settled
            if (gameManager != null)
            {
                StartCoroutine(NotifyGameManagerWithDelay());
            }
        }
    }
    
    IEnumerator NotifyGameManagerWithDelay()
    {
        // Wait for physics to fully settle
        yield return new WaitForSeconds(1.0f);
        
        // Now notify the game manager
        gameManager.BlockSettled();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // We could add additional collision handling here if needed
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit the death zone
        if (other.CompareTag("DeathZone"))
        {
            // We already have a reference to gameManager in this script
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}