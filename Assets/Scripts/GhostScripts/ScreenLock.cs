using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLock : MonoBehaviour
{

    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.12f, 0.88f);
        pos.y = Mathf.Clamp(pos.y, 0.15f, 0.8f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}