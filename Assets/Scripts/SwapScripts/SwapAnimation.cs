using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;




public class SwapAnimation : MonoBehaviour
{
    public GameObject Background;
    public GameObject player1Particles;
    public GameObject player2Particles;
    public GameObject lightningBolt;
    public RuntimeAnimatorController redPlayerAnimator;
    public RuntimeAnimatorController bluePlayerAnimator;
    public RuntimeAnimatorController redGhostAnimator;
    public RuntimeAnimatorController blueGhostAnimator;
    public Material LineRendererRed;
    public Material LineRendererBlue;
    public Sprite xd;

    public Transform Player;
    public Transform Ghost;
    public Transform _lightningBolt;

    public Vector3 StartPos;
    public Vector3 EndPos;

    public float lerpTime;
    public float currentLerpTime;
    public bool Swap;
    private ScoreTracker _ScoreTracker;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Stop();
        GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Clear();
        GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Play();
        Background.SetActive(false);
        lightningBolt.GetComponent<Animator>().enabled = false;
        _ScoreTracker = FindObjectOfType<ScoreTracker>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = redPlayerAnimator;
        GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = blueGhostAnimator;
    }

    private void Update()
    {
        if (Swap == true)
            {
            float perc = currentLerpTime / lerpTime;
            Player = GameObject.Find("Player(Clone)").transform;
            // lightning strike
            lightningBolt.SetActive(true);
            lightningBolt.GetComponent<Animator>().enabled = true;
            //players controllers off
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().stopInput = true;
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<ghostScripts>().enabled = false;
            Background.SetActive(true);
            //swap particles on
            player1Particles.SetActive(true);
            player2Particles.SetActive(true);
            // start timer
            currentLerpTime += Time.deltaTime;
            // swap particles
            player1Particles.transform.position = Vector3.Lerp(StartPos, EndPos, perc);
            player2Particles.transform.position = Vector3.Lerp(EndPos, StartPos, perc);
                     
            //------------------------------------------------------------------------
            if (GameObject.FindGameObjectWithTag("Ghost"))
            {
                lightningBolt.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y+11, Player.transform.position.z);
                Ghost = GameObject.FindGameObjectWithTag("Ghost").transform;

                if (_ScoreTracker.Player1Turn == true)
                {
                    GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererBlue;
                    StartPos = Ghost.transform.position;
                    EndPos = Player.transform.position;
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = redPlayerAnimator;
                    GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = blueGhostAnimator;
                    GameObject.FindGameObjectWithTag("Ghost").GetComponent<Light2D>().color = new Color(0.0462798364f, 0.743228793f, 0.754716992f, 15);
                    GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Stop();
                    GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Clear();
                    GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Play();
                }
                else if (_ScoreTracker.Player1Turn == false)
                {
                    GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererRed;
                    
                    StartPos = Player.transform.position;
                    EndPos = Ghost.transform.position;                    
                    GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = bluePlayerAnimator;
                    GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = redGhostAnimator;
                    GameObject.FindGameObjectWithTag("Ghost").GetComponent<Light2D>().color = new Color (0.783018887f, 0.180980787f, 0.219327003f, 15);
                    GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Play();
                    GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Stop();
                    GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Clear();
                }

                if (currentLerpTime >= 0.3)
                {
                    Background.SetActive(false);
                }

                    if (currentLerpTime >= lerpTime)
                    {                   
                    lightningBolt.SetActive(false);
                    lightningBolt.GetComponent<Animator>().enabled = false;
                    currentLerpTime = 0;
                    Swap = false;
                    player1Particles.SetActive(false);
                    player2Particles.SetActive(false);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>().stopInput = false;
                    GameObject.FindGameObjectWithTag("Ghost").GetComponent<ghostScripts>().enabled = true;
                }              
            }            
        }

       
    }
}
    

    // Update is called once per frame




