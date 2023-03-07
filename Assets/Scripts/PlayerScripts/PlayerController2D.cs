using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController2D : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private int dir = 1;
    private Vector2 movementInput;
    private PlayerInput playerInput;
    private InputUser inputUser;

    [HideInInspector] public bool stopInput;

    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private int baseDamage;
    [SerializeField] private Transform attackPointR;
    [SerializeField] private Transform attackPointL;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Transform check;
    [SerializeField] private LayerMask groundLayers;

    [SerializeField] private  float fallMultiplier = 5f;
    [SerializeField] private  float lowJumpMultiplier = 2.5f;

    [SerializeField] private  float hangTime = .2f;
    private  float hangCounter;

    [SerializeField] private  float jumpBufferLength = .1f;
    private float jumpBufferCount;

    GameObject gameManager;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputUser = playerInput.user;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("gameManager");
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        UpdateAnimParams();
    }

    private void UpdateAnimParams()
    {
        animator.SetBool("run", rb.velocity.x != 0);
        animator.SetFloat("direction", dir);
        animator.SetBool("jump", !IsGroundedButForJumpAnimationCheck());
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (stopInput) return;
        movementInput.y = context.ReadValue<float>();
    }
    
    // called by Player Input Component
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        if (stopInput) return;
        movementInput.x = context.ReadValue<Vector2>().x;
        var xInput = context.ReadValue<Vector2>().x;
        if (xInput > 0) dir = 1;
        if (xInput < 0) dir = -1;
    }
    
    // called by Player Input Component
    public void Attack(InputAction.CallbackContext context)
    {
        if (stopInput) return;
        if (context.performed)
        {
            Transform attackPoint = dir == 1 ? attackPointR : attackPointL;
            animator.SetTrigger("attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            
            
            foreach (Collider2D enemy in hitEnemies)
            {
                var health = enemy.GetComponent<enemy_Health>();
                
                if(health)
                    health.TakeDamage(baseDamage, transform.position);
            }
        }
    }
    
    private bool IsGrounded()
    {
        if (rb.velocity.y > 1) return false;
        return Physics2D.OverlapBox(check.position, new Vector2(0.6f, 0.1f), 0f, groundLayers);
    }
    
    private bool IsGroundedButForJumpAnimationCheck()
    {
        return Physics2D.OverlapBox(check.position, new Vector2(0.6f, 0.1f), 0f, groundLayers);
    }
    
    private void Move()
    {
        rb.velocity = new Vector2(movementInput.x * runSpeed, rb.velocity.y);
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
        if(movementInput.y > 0)
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (jumpBufferCount >= 0 && hangCounter > 0)
        {
            animator.SetBool("jump", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumpBufferCount = 0;
        }
        
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        // Higher jump if key held
        else if (rb.velocity.y > 0 && movementInput.y < 1)
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }

    // Animation Event (in attack and jump animations)
     private void ResetAttack()
     {
         animator.ResetTrigger("attack");
     }
    
     private void OnDrawGizmosSelected()
     {
         Gizmos.DrawWireSphere(attackPointR.position, attackRange);
         Gizmos.DrawWireSphere(attackPointL.position, attackRange);
     }
}
