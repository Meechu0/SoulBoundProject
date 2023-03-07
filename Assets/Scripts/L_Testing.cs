using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Testing : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform[] points;
    [SerializeField] private l_Controller line;
    private LineRenderer lr;
    Renderer rend;
    public GameObject test;
    void Start()
    {
        rend = GetComponent<Renderer>();
        
        lr = GetComponent<LineRenderer>();
        points[0] = GameObject.FindGameObjectWithTag("Ghost").transform;
       
        
    }

    // Update is called once per frame
    void Update()
    {

            NearestEnemy();
            line.SetUpLine(points);
      

       

    }


    public void NearestEnemy()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closestEnemy = null;
        float closestEnemyDist = 100f;

        for (int i = 0; i < enemyList.Length; i++)
        {
            var distance = Vector3.Distance(gameObject.GetComponentInParent<Transform>().position, enemyList[i].transform.position);

            if (distance < closestEnemyDist)
            {
                closestEnemy = enemyList[i];
                closestEnemyDist = distance;
                points[1] = enemyList[i].transform;

                if(distance < gameObject.GetComponentInParent<ghostScripts>().possessDistance)
                {
                    Debug.Log("enable here");
                    rend.enabled = true;
                }
                if (distance > gameObject.GetComponentInParent<ghostScripts>().possessDistance)
                {
                    Debug.Log("Disable here");
                    rend.enabled = false;
                }


            }
           
                


        }



        


        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }

    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }
}
