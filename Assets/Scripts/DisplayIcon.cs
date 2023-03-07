using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DisplayIcon : MonoBehaviour

{
    public Image UI_ICON;
    public GameObject ParticleEff;
    public GameObject Player1Particles;
    public GameObject Player2Particles;
    public Sprite ChargerIcon;
    public Sprite patrolIcon;
    public Sprite oniIcon;
    public Sprite frogIcon;
    public Sprite pufferIcon;
    public Sprite redGhostIcon;
    public Sprite blueGhostIcon;





    private ScoreTracker _ScoreTracker;



    // Start is called before the first frame update
    void Start()
    {
        _ScoreTracker = FindObjectOfType<ScoreTracker>(); 
    }

    // Update is called once per frame

    void Update()
    {      

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (var enemy in enemies)
        {

            if (enemy.GetComponent<enemy_Health>().isPossessed)
            {
                if (enemy.GetComponent<PatrolEnemy>())
                {
                    UI_ICON.GetComponent<Image>().sprite = patrolIcon;
                }
                else if (enemy.GetComponent<chargingEnemy>())
                {
                    UI_ICON.GetComponent<Image>().sprite = ChargerIcon;
                }
                else if (enemy.GetComponent<EnemyShooter>())
                {
                    UI_ICON.GetComponent<Image>().sprite = oniIcon;
                }
                else if (enemy.GetComponent<JumpingEnemy>())
                {
                    UI_ICON.GetComponent<Image>().sprite = frogIcon;
                }
                else if (enemy.GetComponent<ExplodingEnemy>())
                {
                    UI_ICON.GetComponent<Image>().sprite = pufferIcon;
                }


                Player2Particles.transform.position = enemy.transform.position;
                Player1Particles.transform.position = enemy.transform.position;

                if (_ScoreTracker.Player1Turn == true)
                {
                    Player1Particles.SetActive(false);
                    Player2Particles.SetActive(true);
                }

                else if (_ScoreTracker.Player1Turn == false)
                {
                    Player1Particles.SetActive(true);
                    Player2Particles.SetActive(false);
                }


            }

            else if (GameObject.FindGameObjectWithTag("Ghost"))
            {
                if (_ScoreTracker.Player1Turn == true)
                {
                    UI_ICON.GetComponent<Image>().sprite = blueGhostIcon;
                }
                if (_ScoreTracker.Player1Turn == false)
                {
                    UI_ICON.GetComponent<Image>().sprite = redGhostIcon;
                }

                Player1Particles.SetActive(false);
                Player2Particles.SetActive(false);
            }
        }
    }

}



