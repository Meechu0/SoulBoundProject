using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    Rigidbody2D rb;
    public int maxHealth;
    int currentHealth;
    public GameObject enemy;
    public bool playerControlled = false;

    public bool right;
    public bool left;

    public float speed;
    public float playerSpeed;

    public GameObject ghost;


    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage (int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Update()
    {

        if (!playerControlled)
        {
            if (right)
            {
                transform.localScale = new Vector2(1, 1);
                transform.Translate(1 * Time.deltaTime * speed, 0, 0);

            }
            if (left)
            {
                transform.localScale = new Vector2(-1, 1);
                transform.Translate(-1 * Time.deltaTime * speed, 0, 0);

            }
        }
        else if (playerControlled)
        {
            if (Input.GetKey("right"))
            {
                rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
                animator.Play("RightDragon");
            }
            else if (Input.GetKey("left"))
            {
                rb.velocity = new Vector2(-playerSpeed, rb.velocity.y);
                animator.Play("LeftDragon");
            }
            else if (Input.GetKey("up"))
            {
                rb.velocity = new Vector2(rb.velocity.x, playerSpeed);

            }
            else if (Input.GetKey("down"))
            {
                rb.velocity = new Vector2(rb.velocity.x, -playerSpeed);

            }
            else if (Input.GetKey(KeyCode.RightShift))
            {
              //unique action e.g projectile or bite attack
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Turn"))
        {
            if (right == true)
            {
                right = false;
                left = true;
            }
            else if (left == true)
            {
                right = true;
                left = false;
            }
        }
    }

    void Die()
    {
        ghost.transform.parent = null;
        ghost.SetActive(true);
        enemy.SetActive(false);
        //Destroy(enemy);
        
    }
    
}
