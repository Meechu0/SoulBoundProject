using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class MultiPlayerCamera : MonoBehaviour
{

    public List<Transform> targets;

    public Vector2 offset;
    public float smoothTime = 0.3f;

    public float minZoom = 5f;
    public float maxZoom = 50f;
    public float zoomLimiter = 50f;

    private Vector2 velocity;
    private Camera cam;
    public double shake = 0;
    public float shakeStrength = 10f;
    public float TimeMultiplayer = 1;




    private void Update()
    {
        if (shake > 0)
        {
            cam.transform.position = cam.transform.position + Random.insideUnitSphere / shakeStrength;
            shake -= Time.deltaTime * TimeMultiplayer;
        }
        else
        {
            shake = 0;
            shakeStrength = 10f;
        }
        

    }

    private void Start()
    {
        targets[0] = GameObject.FindWithTag("Player").transform;
        targets[1] = GameObject.FindWithTag("Ghost").transform;
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {

       


        if (!targets[1])
        {
            if(GameObject.FindWithTag("Ghost"))
                targets[1] = GameObject.FindWithTag("Ghost").transform;
            else
            {
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemies)
                {
                    if (enemy.GetComponent<enemy_Health>().isPossessed)
                        targets[1] = enemy.transform;
                }
            }
        }
            
        
        if (targets.Count == 0)
            return;

        Move();
        Zoom();

    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter) ;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    void Move()
    {
        Vector2 centerPoint = GetCenterPoint();
        Vector2 newPosition = centerPoint + offset;

        transform.position = Vector2.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector2.zero );

        for (int i =0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        
        if(targets.Count ==1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector2.zero );
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }
}
