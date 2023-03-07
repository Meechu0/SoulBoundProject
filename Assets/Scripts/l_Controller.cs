using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class l_Controller : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;
    private void Awake()
    
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


       
        
    }

    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }
}
