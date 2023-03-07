using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyShooter : Enemy
{

    public float enemySpeed;
    // target player here
    public GameObject Target;
    //how fast enemy adjusts aim
    public float rotationSpeed;
    //where the bullets fire from
    public Transform shootPoint;
    //spawn bullet prefab
    public GameObject bullet;
    // bool used for reloading
    public bool fire = true;

    //rate of fire
    public float shootTime;
    public float distanceToFire;

    public SpriteRenderer sp;

    private Animator anim;
    
    private Vector2 movementInput;
    private int dir;
    private bool ghostControlledFirstShot = false;
    void Start()
    {
        //when enemy spawns will search for the tag and this is how he will identify the player
        Target = GameObject.FindWithTag("Player");
        anim = gameObject.GetComponent<Animator>();
        sp = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GetComponent<enemy_Health>().isPossessed == false)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) < distanceToFire)
            {
                EnemyShoot();
            }
            
           if(transform.position.x > Target.transform.position.x )
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                // sp.flipX = true;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                //sp.flipX = false;
            }

        }

        if (GetComponent<enemy_Health>().isPossessed == true)
        {
            transform.Translate(new Vector2((dir * -movementInput.x) * enemySpeed * Time.deltaTime, (movementInput.y) * enemySpeed * Time.deltaTime));
            transform.eulerAngles = dir == 1 ? new Vector3(0, -180,0) : Vector3.zero;
           
        }
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        var xInput = context.ReadValue<Vector2>().x;
        if (xInput > 0) dir = 1;
        if (xInput < 0) dir = -1;
    }

    void LookAtPlayer()
    {
        // targets player nd updates according to rotation speed variable
        Vector2 direction = Target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    public void PlayerControlledShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (!fire && !ghostControlledFirstShot)
                fire = true;
            EnemyShoot();
            ghostControlledFirstShot = true;
        }
    }

    void EnemyShoot()
    {
        //basic shooting logic
        if (fire == true)
        {
            fire = false;
            Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            StartCoroutine(ReloadFire());
            anim.Play("Oni Fire");
        }

    }
    
    IEnumerator ReloadFire()
    {
        //stops enemy spam firing and allows control of rate of fire
        yield return new WaitForSeconds(shootTime);
        fire = true;
    }
}

