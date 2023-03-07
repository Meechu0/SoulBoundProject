using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class random_Enemy_Spawning : MonoBehaviour
{

    public GameObject[] enemySelection;
    int random_Number;
    GameObject Selector;
    void Start()
    {

        pickRandom();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void pickRandom()
    {
        int customSeed = System.DateTime.Now.Millisecond;
        Random.InitState(customSeed);
        random_Number = Random.Range(0, enemySelection.Length);

        Instantiate(enemySelection[random_Number], transform.position, Quaternion.identity);
        Debug.Log("Spawned " + enemySelection[random_Number].name.ToString());
        Destroy(gameObject);
    }
}
