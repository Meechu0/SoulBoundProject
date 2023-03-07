using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformCollisionChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy") && collision.gameObject.GetComponent<enemy_Health>().isPossessed == false)
        {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
