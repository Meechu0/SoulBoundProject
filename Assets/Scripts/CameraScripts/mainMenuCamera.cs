using UnityEngine;
using System.Collections;

public class mainMenuCamera : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}