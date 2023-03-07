using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class enemy_Health : MonoBehaviour
{
    public float enemyHealth;
    public bool isPossessed = false;

    public float rumbleLowFrequency = 0.5f;
    public float rumbleHighFrequency = 0.5f;
    public float rumbleTime = 0.2f;
    private PlayerManager _playerManager;
    private Knockback _knockback;
    public bool atEdge;

    void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        _knockback = GetComponent<Knockback>();
    }

    void Update()
    {
        //--  Clamps enemy when possessed ( cant run outside of camera borders)
        if (isPossessed)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            pos.x = Mathf.Clamp(pos.x, 0.08f, 0.92f);
            pos.y = Mathf.Clamp(pos.y, 0.15f, 0.8f);
            transform.position = Camera.main.ViewportToWorldPoint(pos);
            if (pos.x >= 0.91f || pos.x <= 0.09f)
            {
                atEdge = true;
            }
            else
            {
                atEdge = false;
            }
        }
        
        Die();
    }

    private void Die()
    {
        
        if (isPossessed && enemyHealth <= 0)
        {
            _playerManager.RespawnGhostAndRemoveEnemy();
            Destroy(gameObject);
        }
        else if (enemyHealth <= 0 && !isPossessed)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage, Vector3 pos)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(whitecolor());
        enemyHealth -= damage;
        Sound_Manager.playSound("Hit");
        _knockback.StartKnockback(pos);
        _playerManager.GhostRumble(rumbleLowFrequency, rumbleHighFrequency, rumbleTime);
    }
    IEnumerator whitecolor()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    public void ejectGhost(InputAction.CallbackContext context)
    {
        if(context.performed && isPossessed)
        {
            _playerManager.RespawnGhost(transform.position);
            Sound_Manager.playSound("de_Possess");
            isPossessed = false;
        }
    }


}
