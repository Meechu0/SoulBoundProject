using System;
using UnityEngine;

public class Enemy_Projectile : MonoBehaviour
{
    public float projectile_Speed;
    public float projectile_Life;
    public float projectile_Arc;
    public float spinSpeed = 5f;
    public int damage = 1;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, projectile_Life);
        rb.velocity = -transform.right * projectile_Speed + Vector3.up * projectile_Arc;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * spinSpeed);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Enemy")) return;

        if (col.collider.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Health>().takeDamage(damage);
            Debug.Log("PLAYER_DAMAGED");
        }
        // if it hits environment the bullet will despawn
        Destroy(gameObject);
    }
}
