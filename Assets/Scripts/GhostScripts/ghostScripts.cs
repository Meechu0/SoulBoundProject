using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ghostScripts : MonoBehaviour
{
    Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public float Speed;
    private Vector2 moveDir;
    public bool hasPossessed = false;
    public float nearestEnemy;
    public GameObject[] enemies;
    public float possessDistance;
    public GameObject possessGUI;
    private PlayerInput playerInput;

    private Vector2 movementInput;
    private PlayerManager _playerManager;
    
    void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }
    
    void FixedUpdate()
    {
        rb.velocity = movementInput * Speed;
        spriteRenderer.flipX = movementInput.x > 1;
        
        Die();
    }
    
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    void Die()
    {
        if (hasPossessed)
        {
            Destroy(gameObject);
        }
        else
            Camera.main.GetComponent<MultiPlayerCamera>().targets[1] = GameObject.FindWithTag("Ghost").transform;
    }
    
    public void CheckEnemyDistance(InputAction.CallbackContext context)
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closestEnemy = null;
        float closestEnemyDist = 100f;

        for (int i = 0; i < enemyList.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, enemyList[i].transform.position);
            
            if (distance < closestEnemyDist)
            {
                closestEnemy = enemyList[i];
                closestEnemyDist = distance;
            }
        }

        if (closestEnemyDist <= possessDistance)
        {
            if (context.performed)
            {
                closestEnemy.GetComponent<enemy_Health>().isPossessed = true;
                _playerManager.InstantiateEnemyWithInput(closestEnemy, closestEnemy.transform.position);
                Sound_Manager.playSound("Possess");
                Destroy(closestEnemy);
                hasPossessed = true;
            }
        }
    }
}

