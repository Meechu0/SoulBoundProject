using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class JumpingEnemy : Enemy
{
    private GameObject _target;
    private Rigidbody2D _rb;
    private Vector2 _movementInput;
    private enemy_Health _enemyHealth;
    private Animator _animator;
    private SpriteRenderer _sp;
    private bool _coroutinesDisabled;
    private bool _releaseJump;
    private bool _isChargedJumping;
    private bool _isChargingJump;
    private float _jumpCharge;
    private float _lastShootTime;
    private int _dir = -1;
    private float _attackCharge;
    private bool _isChargingAttack;
    private bool _hasPerformedFlashEffect;

    public Transform groundCheck;
    public Transform shootPoint;
    public GameObject projectile;
    public Material flashMat;
    public LayerMask groundLayers;

    public float minAIJumpForce;
    public float maxAIJumpForce;
    public float minAIJumpTime;
    public float maxAIJumpTime;
    public float minAIShootTime;
    public float maxAIShootTime;

    public float horizontalHoppingForce = 2f;
    public float verticalHoppingForce = 1f;
    public float horizontalJumpingForce = 1f;
    public float verticalJumpingForce = 1f;
    public float jumpChargeRate = 30f;
    public float maxJumpCharge = 13f;
    public float airDrift = 4f;
    public float maxAirSpeed = 10f;
    
    public float timeBetweenAttacks = 1f;
    public float attackChargeRate = 1f;
    public float maxAttackCharge;
    public float projectileSpeedMultiplier = 1;
    public float projectileArcMultiplier = 1;
    public float minProjectileSpeed = 1;
    public float maxprojecitleSpeed = 100;
    public float minProjecitleArc = 1;
    public float maxProjecileArc = 100;
    public float flashDuration = 0.15f;

    void Start()
    {
        _enemyHealth = GetComponent<enemy_Health>();
        _target = GameObject.FindWithTag("Player");
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sp = GetComponent<SpriteRenderer>();
        StartCoroutine(JumpSystem());
        StartCoroutine(ShootLogic());
        _coroutinesDisabled = false;
        _lastShootTime = Time.time;
    }
    
    void Update()
    {   
        if (IsGrounded()) _isChargedJumping = false;
        if (IsGrounded()) _rb.velocity = new Vector2(0, _rb.velocity.y);

        if (_enemyHealth.isPossessed == false)
        {
            if (transform.position.x > _target.transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        if (_isChargingAttack)
        {
            if(_attackCharge <= maxAttackCharge)
                _attackCharge += attackChargeRate * Time.deltaTime;
            if (_isChargingAttack && _attackCharge >= maxAttackCharge && !_hasPerformedFlashEffect)
            {
                _hasPerformedFlashEffect = true;
                StartCoroutine(nameof(AttackChargedFlash));
            }
        }
        
        _animator.SetBool("isChargingJump", _isChargingJump);
        _animator.SetBool("isGrounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        if(_enemyHealth.isPossessed == false)
        {
            if(_coroutinesDisabled)
            {
                StartCoroutine(JumpSystem());
                StartCoroutine(ShootLogic());
                _coroutinesDisabled = false;
            }
        }
        PossessedMovement();
    }

    private void PossessedMovement()
    {
        if(_enemyHealth.isPossessed)
        {
            transform.eulerAngles = new Vector3(0, _dir > 0 ? 0 : -180);

            if (!_coroutinesDisabled)
            {
                StopAllCoroutines();
                _coroutinesDisabled = true;
            }

            if (_movementInput.x != 0 && IsGrounded() && !_isChargingJump)
            {
                _rb.velocity = new Vector2(_dir * horizontalHoppingForce, verticalHoppingForce);
            }
            
            if (_movementInput.y > 0 && IsGrounded())
            {
                _isChargingJump = true;
                if(_jumpCharge < maxJumpCharge)
                    _jumpCharge += jumpChargeRate * Time.deltaTime;
            }
            
            if (_movementInput.y == 0 && _jumpCharge > 0)
            {
                _releaseJump = true;
                Sound_Manager.playSound("frog_Jump");
            }

            if (_releaseJump && _jumpCharge > 0)
            {
                _isChargedJumping = true;
                _isChargingJump = false;
                _rb.velocity = new Vector2(_dir * horizontalJumpingForce, verticalJumpingForce) * _jumpCharge;

                _jumpCharge = 0;
                _releaseJump = false;
            }
            
            if(_isChargedJumping && !IsGrounded())
            {
                _rb.AddForce(new Vector2(_movementInput.x * airDrift, 0), ForceMode2D.Force);

                _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -maxAirSpeed, maxAirSpeed), _rb.velocity.y);
            }
        }
    }
    
    private bool IsGrounded()
    {
        if (_rb.velocity.y > 0.1f) return false;
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.6f, 0.15f), 0f, groundLayers);
    }
    
    // called by Player Input Component
    public void GetJumpInput(InputAction.CallbackContext context)
    {
        _movementInput.y = context.ReadValue<float>();
    }
    
    // called by Player Input Component
    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _movementInput.x = context.ReadValue<Vector2>().x;
        var xInput = context.ReadValue<Vector2>().x;
        if (xInput > 0) _dir = 1;
        if (xInput < 0) _dir = -1;
    }

    public void PlayerShoot(InputAction.CallbackContext context)
    {
        if (!_enemyHealth.isPossessed) return;
        
            if (context.performed)
            {
                _isChargingAttack = true;
            }

            if (context.canceled)
            {
                _isChargingAttack = false;
                _hasPerformedFlashEffect = false;

                if (Time.time - _lastShootTime > timeBetweenAttacks)
                {
                    var newProjectile = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                    newProjectile.GetComponent<Enemy_Projectile>().projectile_Arc =
                        Mathf.Clamp(3 * _attackCharge * projectileArcMultiplier, minProjectileSpeed, maxprojecitleSpeed);
                    newProjectile.GetComponent<Enemy_Projectile>().projectile_Speed = 
                        Mathf.Clamp(7 * _attackCharge * projectileSpeedMultiplier, minProjecitleArc, maxProjecileArc);
                    _lastShootTime = Time.time;

                    if (_attackCharge >= maxAttackCharge)
                    {
                        newProjectile.GetComponent<Enemy_Projectile>().damage = 2;
                    }
                }
                _attackCharge = 0f;
            }
    }

    void EnemyShoot()
    {
        Instantiate(projectile, shootPoint.position, shootPoint.rotation);
    }
    
    void AIJump()
    {
        _rb.velocity = Vector2.up * Random.Range(minAIJumpForce, maxAIJumpForce);
    }

    IEnumerator JumpSystem()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAIJumpTime, maxAIJumpTime));
            if (IsGrounded())
            {
                AIJump();
            }
        }
    }

    IEnumerator ShootLogic()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAIShootTime, maxAIShootTime));
            EnemyShoot();
        }
    }

    IEnumerator AttackChargedFlash()
    {
        var currentMat = _sp.material;
        _sp.material = flashMat;
        Sound_Manager.playSound("frog_Blip");
        yield return new WaitForSeconds(flashDuration);
        _sp.material = currentMat;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(0.6f, 0.15f, 0));
    }
}

