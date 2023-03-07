using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform targetPoint1;
    [SerializeField] private Transform targetPoint2;
    
    [SerializeField] private float speed = 0.1f;

    private float timer;


    private void Start()
    {
        platform.position = targetPoint1.position;
    }

    private void Update()
    {
        timer = Mathf.PingPong(Time.time * speed, 1);
        platform.position = Vector2.Lerp(targetPoint1.position, targetPoint2.position,timer);
    }
}
