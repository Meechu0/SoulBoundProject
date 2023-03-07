using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_Pad : MonoBehaviour
{
    private Animator _animator;
    Vector3 direction;
    public float jumpForce;
    // Start is called before the first frame update

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = transform.TransformDirection(Vector3.up * jumpForce);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
            Sound_Manager.playSound("jump_Pad");
            _animator.SetTrigger("bounce");
        }
    }
}
