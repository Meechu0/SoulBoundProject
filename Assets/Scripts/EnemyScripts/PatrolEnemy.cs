using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PatrolEnemy : Enemy
{
    //idea for script from lost relic games  https://www.youtube.com/watch?v=dHkbDn-KQ9E&t=30s&ab_channel=LostRelicGames
    const string LEFT = "left";
    const string RIGHT = "right";

    public Transform castPos;

    public int damage = 1;
    public float knockbackStrength = 20f;
    public float baseCastDist;

    public string facingDirection;

    Vector3 baseScale;

    private SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    public float enemySpeed = 5;

    private Vector2 movementInput;
    public Transform groundCheckPos;
    
    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;
    [SerializeField] private float hangTime = .2f;
    private  float hangCounter;
    [SerializeField] private float jumpBufferLength = .1f;
    private float jumpBufferCount;
    private bool knockback;
    [SerializeField] private LayerMask groundLayers;

    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;

        facingDirection = LEFT;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        if (knockback) return;
        
        if (GetComponent<enemy_Health>().isPossessed == false)
        {
            if (facingDirection == RIGHT) 
            { 
                rb.velocity = new Vector2(1 * enemySpeed, rb.velocity.y);
            }
            if (facingDirection == LEFT) 
            { 
                rb.velocity = new Vector2(-1 * enemySpeed, rb.velocity.y);
            }

            if (isHittingWall() || isNearEdge())
            {
                if (facingDirection == LEFT)
                {
                    ChangeFacingDirection(RIGHT);
                }
                else if (facingDirection == RIGHT)
                {
                    ChangeFacingDirection(LEFT);
                }
            }
        }
        if (!IsGrounded() && GetComponent<enemy_Health>().isPossessed == false)
        {
            if (rb.velocity.x < 0)
            {
                ChangeFacingDirection(LEFT);
            }

            if (rb.velocity.x > 0)
            {
                ChangeFacingDirection(RIGHT);
            }
        }


        if (GetComponent<enemy_Health>().isPossessed == true)
        {
            rb.velocity = new Vector2(movementInput.x * enemySpeed, rb.velocity.y);
            
            if(movementInput.x > 0)
                ChangeFacingDirection(RIGHT);
            else if(movementInput.x < 0)
                ChangeFacingDirection(LEFT);
        }
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        movementInput.y = context.ReadValue<float>();
    }
    
    // called by Player Input Component
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        movementInput.x = context.ReadValue<Vector2>().x;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, new Vector2(0.6f, 0.1f), 0f, groundLayers);
    }
    
    private void Jump()
    {
        //manage hangtime which is time after leaving the ground you can jump
        if(IsGrounded())
            hangCounter = hangTime;
        else
        {
            hangCounter -= Time.deltaTime;
        }
    
        //manage the jump buffer
        if(movementInput.y > 0 && GetComponent<enemy_Health>().isPossessed)
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (jumpBufferCount >= 0 && hangCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumpBufferCount = 0;
        }
        
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        // // Higher jump if key held
        // else if (rb.velocity.y > 0 && movementInput.y < 1)
        //     rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }

    bool isHittingWall()
    {
        bool val = false;

        float castDist = baseCastDist;

        if (facingDirection == LEFT)
        {
            castDist = -baseCastDist;
        }
        else
        {
            castDist = baseCastDist;
        }

        Vector3 targetPos = castPos.position;
        targetPos.x += castDist;

        Debug.DrawLine(castPos.position, targetPos, Color.red);

        if (Physics2D.Linecast(castPos.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            val = true;
        }
        else
        {
            val = false;
        }




        return val;
    }



    bool isNearEdge()
    {
        bool val = true;



        float castDist = baseCastDist;



        Vector3 targetPos = castPos.position;
        targetPos.y -= castDist;

        Debug.DrawLine(castPos.position, targetPos, Color.green);

        if (Physics2D.Linecast(castPos.position, targetPos, 1 << LayerMask.NameToLayer("Ground")))
        {
            val = false;
        }
        else
        {
            val = true;
        }




        return val;
    }


    void ChangeFacingDirection(string newDirection)
    {
        Vector3 newScale = baseScale;

        if (newDirection == LEFT)
        {
            newScale.x = 1;
        }
        else if (newDirection == RIGHT)
        {
            newScale.x = -1;
        }

        transform.localScale = newScale;

        facingDirection = newDirection;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Health>().takeDamage(damage);
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(-col.contacts[0].normal + Vector2.up * knockbackStrength, ForceMode2D.Impulse);
        }

        if (col.collider.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }

        if (col.collider.CompareTag("Jump Pad"))
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }

        if (col.collider.CompareTag("Platform") && GetComponent<enemy_Health>().isPossessed == false)
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundCheckPos.position, new Vector3(0.6f,0.1f, 1));
    }
}


