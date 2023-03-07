using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Image[] lives;
    public Sprite fullLife;
    public Sprite emptyLife;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < lives.Length; i++)
        {
            if (i < health)
            {
                lives[i].sprite = fullLife;
            }
            else
            {
                lives[i].sprite = emptyLife;
            }
            if (i < maxHealth)
            {
                lives[i].enabled = true;
            }
            else
            {
                lives[i].enabled = true;
            }

        }
    }
}
