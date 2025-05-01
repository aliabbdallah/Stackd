using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    [Header("Physics Settings")]
    public float settleThreshold = 0.1f; 
    public float maxSettleTime = 6.0f; 

    [Header("Season Physics")]
    public SeasonPhysics seasonPhysics; 

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

        
        gameManager = FindObjectOfType<GameManager>();

        
        if (seasonPhysics != null)
        {
            ApplySeasonPhysics();
        }
    }

    void ApplySeasonPhysics()
    {
        
        rb.gravityScale = seasonPhysics.gravity;
        
        
        PhysicsMaterial2D blockMaterial = new PhysicsMaterial2D();
        blockMaterial.friction = seasonPhysics.blockFriction;
        blockMaterial.bounciness = seasonPhysics.blockBounciness;
        
        
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
                float gustStrength = Random.Range(1f, 3f); 
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

            
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                DropBlock();
            }
        }
        else if (isDropping && !hasSettled)
        {
            
            ApplySeasonEffects();
            
            
            CheckIfSettled();
            
            
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
        
        if (rb.velocity.magnitude < settleThreshold && Mathf.Abs(rb.angularVelocity) < settleThreshold)
        {
            
            stableTime += Time.deltaTime;
            
            
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
        
        if (hasSettled)
        {
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static; 
            
            
            this.enabled = false;

            
            if (gameManager != null)
            {
                StartCoroutine(NotifyGameManagerWithDelay());
            }
        }
    }
    
    IEnumerator NotifyGameManagerWithDelay()
    {
        
        yield return new WaitForSeconds(1.0f);
        
        
        gameManager.BlockSettled();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("DeathZone"))
        {
            
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}