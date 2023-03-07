using System;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackDuration = .1f;
    [SerializeField] private float knockbackSpeed = 30f;
    [SerializeField] private float knockUpStrength = 1f;

    private float timeLeft;
    private bool performKnockback;

    private Rigidbody2D rb;
    private float otherEntityXPos;
    private Enemy enemyController;

    private bool isCharger;
    private chargingEnemy charger;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyController = GetComponent<Enemy>();

        if (GetComponent<chargingEnemy>())
        {
            isCharger = true;
            charger = GetComponent<chargingEnemy>();
        }
    }
    private void FixedUpdate()
    {
        if (!performKnockback) return;
        if (isCharger && charger.isChasing)
        {
            return;
        }
        
        if (timeLeft > 0)
        {
            enemyController.enabled = false;
            rb.velocity = new Vector2(otherEntityXPos * knockbackSpeed, knockUpStrength);
            timeLeft -= Time.deltaTime;
        }
        else if (timeLeft <= 0)
        {
            rb.velocity = Vector2.zero;
            enemyController.enabled = true;
            performKnockback = false;
        }
    }

    public void StartKnockback(Vector3 pos)
    {
        otherEntityXPos = (transform.position - pos).x >= 0 ? 1 : -1;
        timeLeft = knockbackDuration;
        performKnockback = true;
    }
}
