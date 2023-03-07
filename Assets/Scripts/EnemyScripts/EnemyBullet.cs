using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float BulletSpeed;
    public float BulletLife;
    public Rigidbody2D rb;
    void Start()
    {
        Destroy(gameObject, BulletLife);
        // bullet speed adjusts velocity
        rb.velocity = transform.right * BulletSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
         col.gameObject.GetComponent<Health>().takeDamage(1);
            Debug.Log("PLAYER_DAMAGED");
        }
        // if it hits environment the bullet will despawn
        Destroy(gameObject);
    }


}
