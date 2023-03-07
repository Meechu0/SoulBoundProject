using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class chargingEnemy : Enemy
{
    const string LEFT = "left";
    const string RIGHT = "right";

    public Transform castPos;
    public Transform castPosPlayerDetection;
    private GameObject Target;
    private Animator _animator;
    private MultiPlayerCamera shakeCamera;

    public int damage = 1;
    public float knockbackStrength = 20f;
    public float baseCastDist;
    public float playerCastDist;
    public float stunDuration = 1f;
    public bool isChasing = false;
    public float cameraShakeDuration = 0.1f;
    public float cameraShakeStrength = 10f;
    private bool CR_Running = false;
    private bool isStunned;

    public string facingDirection;

    Vector3 baseScale;

    private SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    public float enemySpeed;
    public float normalSpeed;
    public float chasingSpeed;
    public float agroRange;
    public float distToPlayer;

    private Vector2 movementInput;
    public Transform groundCheckPos;

    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;
    [SerializeField] private float hangTime = .2f;
    private float hangCounter;
    [SerializeField] private float jumpBufferLength = .1f;
    private float jumpBufferCount;
    private bool knockback;
    [SerializeField] private LayerMask groundLayers;


    // Start is called before the first frame update
    void Start()
    {
        shakeCamera = FindObjectOfType<MultiPlayerCamera>();
        _animator = GetComponent<Animator>();
        baseScale = transform.localScale;

        facingDirection = LEFT;
        enemySpeed = normalSpeed;
        Target = GameObject.Find("Player(Clone)");

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        isChasing = false;
    }

    private void Update()
    {
        SetupAnimations();
        Jump();

        if (GetComponent<enemy_Health>().atEdge && !CR_Running)
        {
            StartCoroutine(stunTime());
        }
        
        if (GetComponent<enemy_Health>().isPossessed == false)
        {
            if (isHittingWall() && enemySpeed == 10 && !CR_Running)
            {
                StartCoroutine(stunTime());
            }


            if (canSeePlayer(agroRange) && CR_Running == false)
            {
                StartCoroutine(StartCharge());
            }
        }
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

            if (isHittingWall() && !isChasing)
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
            if (isChasing)
            {
                rb.velocity = new Vector2((facingDirection == LEFT ? -1 : 1) * enemySpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(movementInput.x * enemySpeed, rb.velocity.y);
            }
            
            if (movementInput.x > 0 && !CR_Running)
                ChangeFacingDirection(RIGHT);
            else if (movementInput.x < 0&& !CR_Running)
                ChangeFacingDirection(LEFT);

            if (isHittingWall() && enemySpeed == chasingSpeed && !CR_Running) 
            {
                setEnemySpeed();
                StartCoroutine(stunTime());
            }
        }
    }

    private void SetupAnimations()
    {
        if (GetComponent<enemy_Health>().isPossessed)
        {
            _animator.SetBool("isCharging", isChasing);
            _animator.SetBool("IsGrounded", IsGrounded());
            _animator.SetBool("isMoving", movementInput.x != 0);
        }
        else
        {
            _animator.SetBool("isCharging", isChasing);
            _animator.SetBool("IsGrounded", IsGrounded());
            _animator.SetBool("isMoving", enemySpeed != 0);
        }
    }

    public void Charge(InputAction.CallbackContext context)
    {
        if (context.performed && !isChasing && IsGrounded() && GetComponent<enemy_Health>().isPossessed)
        {
            StartCoroutine(StartCharge());
        }
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (isStunned) return;
        movementInput.y = context.ReadValue<float>();
    }

    // called by Player Input Component
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        if (isChasing) return;
        movementInput.x = context.ReadValue<Vector2>().x;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, new Vector2(0.6f, 0.1f), 0f, groundLayers);
    }

    bool canSeePlayer(float distance)
    {
        bool val = false;

        float castDist = playerCastDist;

        if (facingDirection == LEFT)
        {
            castDist = -playerCastDist;
        }
        else
        {
            castDist = playerCastDist;
        }

        Vector3 targetPos = castPosPlayerDetection.position;
        targetPos.x += castDist;

        // Debug.DrawLine(castPos.position, targetPos, Color.yellow);

        RaycastHit2D hit = Physics2D.Linecast(castPosPlayerDetection.position, targetPos, 1 << LayerMask.NameToLayer("Default"));
        

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    val = true;
                }
                else
                {
                    val = false;
                }
            
        }
        return val;
    }

    private void Jump()
    {
        //manage hangtime which is time after leaving the ground you can jump
        if (IsGrounded())
            hangCounter = hangTime;
        else
        {
            hangCounter -= Time.deltaTime;
        }

        //manage the jump buffer
        if (movementInput.y > 0 && GetComponent<enemy_Health>().isPossessed && IsGrounded())
            jumpBufferCount = jumpBufferLength;
        else
            jumpBufferCount -= Time.deltaTime;

        if (jumpBufferCount >= 0 && hangCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            jumpBufferCount = 0;
        }

        if (rb.velocity.y < 1)
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
            col.gameObject.GetComponent<Rigidbody2D>().velocity = -col.contacts[0].normal + Vector2.up * knockbackStrength;
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

    void setEnemySpeed()
    {
        isChasing = false;
        isStunned = false;
        enemySpeed = normalSpeed;
    }

    IEnumerator stunTime()
    {
        Sound_Manager.playSound("charge_Enemy_Thud");
        shakeCamera.shake = cameraShakeDuration;
        shakeCamera.shakeStrength = cameraShakeStrength;
        isStunned = true;
        movementInput.y = 0;
        _animator.SetTrigger("HitWall");
        CR_Running = true;
        enemySpeed = 0;
        yield return new WaitForSeconds(stunDuration);
        enemySpeed = normalSpeed;
        CR_Running = false;
        setEnemySpeed();
    }

    IEnumerator StartCharge()
    {
        _animator.SetTrigger("Charge");
        enemySpeed = 0;
        isChasing = true;
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        enemySpeed = chasingSpeed;
        
    }

    private void ResetTrigger()
    {
        _animator.ResetTrigger("Charge");
        _animator.ResetTrigger("HitWall");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundCheckPos.position, new Vector3(0.6f, 0.1f, 1));
    }

   










































    /*bool canSeePlayer(float distance)
    {
        bool val = false;

        if (Target.transform.position.x > transform.position.x)
        {
            
            RaycastHit2D hit = Physics2D.Raycast(castPoint2.transform.position, Vector2.right, distance);
            Debug.DrawRay(castPoint2.transform.position, Vector2.right.normalized * distance, Color.red);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    val = true;
                    Debug.Log("Charging_Enemy_Found_Player");

                }
                else
                {
                    val = false;
                }

            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(castPoint.transform.position, Vector2.left, distance);
            Debug.DrawRay(castPoint.transform.position, transform.right * distance, Color.red);
            if (hit.collider != null)
            {
                
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    val = true;
                    Debug.Log("Charging_Enemy_Found_Player");

                }
                else
                {
                    val = false;
                }

            }
        }


       
        return val;

    }
    */

    /*IEnumerator startCharge()
    {
        if (canCharge == true)
        {
            if (Target.transform.position.x < transform.position.x)
            {
                rb.AddForce(new Vector2(-20, 0));
            }

            if (Target.transform.position.x > transform.position.x)
            {
                rb.AddForce(new Vector2(20, 0));
            }

            canCharge = false;

        }

        yield return new WaitForSeconds(2f);
        canCharge = true;
        
    }
    */

}
