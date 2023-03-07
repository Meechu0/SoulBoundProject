using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExplodingEnemy : Enemy
{
    public float bounceForce = 2f;
    public GameObject Target;
    public GameObject explosionPrefab;
    public float enemySpeed;
    public float acceleration = 20f;
    public float speedLimit = 14f;
    public float distanceTillAttack;
    public float detonationTime;
    public float radius = 5.0F;
    public float explosionForce;
    public Sprite sprite2;
    public Sprite sprite3;
    private Vector2 movementInput;
    private int dir;
    CircleCollider2D Cc;
    Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;
    private MultiPlayerCamera _camera;

    private Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.FindWithTag("Player");
        Cc = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _camera = FindObjectOfType<MultiPlayerCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<enemy_Health>().isPossessed == false)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) < distanceTillAttack)
            {
                moveTowardsPlayer();
            }
            if (Vector2.Distance(transform.position, Target.transform.position) < 2)
            {
                Detonate();
            }

        }
    }

    private void FixedUpdate()
    {
        if (GetComponent<enemy_Health>().isPossessed)
        {
            spriteRenderer.flipX = dir > 0;
            
            rb.AddForce(movementInput * acceleration, ForceMode2D.Force);

            if (rb.velocity.magnitude > speedLimit)
            {
                rb.velocity = rb.velocity.normalized * speedLimit;
            }
        }
        if (rb.velocity.normalized != Vector2.zero)
            direction = rb.velocity;
    }

    void moveTowardsPlayer()
    {
        rb.velocity = Vector2.zero;
        float smoothSpeed = enemySpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, smoothSpeed);
    }
    
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        var xInput = context.ReadValue<Vector2>().x;
        if (xInput > 0) dir = 1;
        if (xInput < 0) dir = -1;
    }

    public void Detonate(InputAction.CallbackContext context)
    {
        if (context.performed && GetComponent<enemy_Health>().isPossessed)
            Detonate();
    }

    void Detonate()
    {
        Cc.isTrigger = false;
        rb.gravityScale = 2;
        enemySpeed = 0;
        rb.velocity = Vector2.zero;
        gameObject.tag = "Bomb";
        StartCoroutine(waitForDetonation());
    }

    IEnumerator waitForDetonation()
    {

        for (var timeLeft = detonationTime; timeLeft > 0; timeLeft -= Time.deltaTime)
        {
            if (timeLeft <= 1f)
                spriteRenderer.sprite = sprite3;
            else if (timeLeft <= 2f)
                spriteRenderer.sprite = sprite2;
            yield return null;
        }

        var ps =Instantiate(explosionPrefab, transform.position, quaternion.identity);
        Destroy(ps, 6f);
        Debug.Log("Exploded");
        Sound_Manager.playSound("puffer_Fish_Explosion");
        Vector3 explosionPos = transform.position;
        _camera.shake = 0.5;
        _camera.shakeStrength = 15f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in colliders)
        {
            if(hit.gameObject.tag == "Player")
            {
                hit.GetComponent<Health>().health -= 2;
                hit.GetComponent<Rigidbody2D>().velocity = Vector2.up * explosionForce;
                Debug.Log("BOMB_DAMAGED_PLAYER");
                break;
            }
        }

        GetComponent<enemy_Health>().enemyHealth = 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            var reflect = Vector2.Reflect(direction.normalized * (direction.magnitude * bounceForce), col.contacts[0].normal);
            rb.AddForce(reflect * bounceForce, ForceMode2D.Impulse);
        }
    }
}

    
