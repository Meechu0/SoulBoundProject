using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Image[] lives;
    public Sprite fullLife;
    public Sprite emptyLife;
    
    public float rumbleLowFrequency = 0.5f;
    public float rumbleHighFrequency = 0.5f;
    public float rumbleTime = 0.2f;

    private SwapAnimation swapAnimation;
    private PlayerManager _playerManager;
    private UIController uicontroller;


    private void Awake()
    {
        uicontroller = FindObjectOfType<UIController>();
        swapAnimation = FindObjectOfType<SwapAnimation>();
        _playerManager = FindObjectOfType<PlayerManager>();

    }

    void Update()
    {
        uicontroller.health = health;
    }
    public void takeDamage(int damage)
    {
        if (swapAnimation.Swap == false)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            StartCoroutine(whitecolor());
            uicontroller.health -= damage;
            health -= damage;
            Sound_Manager.playSound("Hit");
        }    
        _playerManager.PlayerRumble(rumbleLowFrequency, rumbleHighFrequency, rumbleTime);
    }

    IEnumerator whitecolor()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
        

