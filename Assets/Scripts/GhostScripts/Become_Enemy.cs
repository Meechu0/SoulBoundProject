using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Become_Enemy : MonoBehaviour
{
    private bool isEnemy = false;
    public GameObject guiObj;
    GameObject ghost;
    enemyScript enemyControl;

    // Start is called before the first frame update
    void Start()
    {
        ghost = GameObject.FindWithTag("Ghost");
        guiObj.SetActive(false);
        enemyControl = GetComponent<enemyScript>();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ghost" && isEnemy == false)
        {
            guiObj.SetActive(true);
            if (Input.GetKey("e"))
            {
                guiObj.SetActive(false);
                ghost.transform.parent = gameObject.transform;
                ghost.SetActive(false);
                isEnemy = true;
                enemyControl.playerControlled = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ghost")
        {
            guiObj.SetActive(false);
        }
    
    }


    // Update is called once per frame
    void Update()
    {
        if(isEnemy == true && Input.GetKey((KeyCode.F)))
        {
            isEnemy = false;
            ghost.SetActive(true);
            ghost.transform.parent = null;
            enemyControl.playerControlled = false;
        }
    }
}
