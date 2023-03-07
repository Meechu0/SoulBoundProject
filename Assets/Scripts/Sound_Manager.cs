using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Manager : MonoBehaviour
{
    static AudioSource source;
    public static AudioClip frog_Jump, Hit, Swap, menu_Select, jump_Pad, Jump, puffer_Fish_Explosion, Possess, Lightning, depossess, hit_Miss, frog_Blip, charge_Enemy_Thud;

    void Start()
    {
        frog_Jump = Resources.Load<AudioClip>("SFX/Frog_Jump");
        Hit = Resources.Load<AudioClip>("SFX/Hit");
        jump_Pad = Resources.Load<AudioClip>("SFX/Jump_Pad");
        menu_Select = Resources.Load<AudioClip>("SFX/Menu_Select");
        Swap = Resources.Load <AudioClip>("SFX/Swap");
        puffer_Fish_Explosion = Resources.Load<AudioClip>("SFX/Puffer_Fish_Explosion_");
        Possess = Resources.Load<AudioClip>("SFX/Possess");
        Lightning = Resources.Load<AudioClip>("SFX/Lightning");
        Jump = Resources.Load<AudioClip>("SFX/Jump");
        depossess = Resources.Load<AudioClip>("SFX/de possess");
        hit_Miss = Resources.Load<AudioClip>("SFX/Sword_Miss");
        frog_Blip = Resources.Load<AudioClip>("SFX/Frog_Charge_Blip");
        charge_Enemy_Thud = Resources.Load<AudioClip>("SFX/Charging_Enemy_Hits_Wall");



        source = GetComponent<AudioSource>();
        source.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void playSound(string clip)
    {
        switch (clip)
        {
            case "frog_Jump":
                source.PlayOneShot(frog_Jump);
            break;
            case "jump_Pad":
                source.PlayOneShot(jump_Pad);
                break;
            case "Hit":
                source.PlayOneShot(Hit);
                break;
            case "Swap":
                source.PlayOneShot(Hit);
                break;
            case "menu_Select":
                source.PlayOneShot(menu_Select);
                break;
            case "puffer_Fish_Explosion":
                source.PlayOneShot(puffer_Fish_Explosion);
                break;
            case "Possess":
                source.PlayOneShot(Possess);
                break;
            case "Lightning":
                source.PlayOneShot(Lightning);
                break;
            case "Jump":
                source.PlayOneShot(Jump);
                break;
            case "de_Possess":
                source.PlayOneShot(depossess);
                break;
            case "sword_Miss":
                source.PlayOneShot(hit_Miss);
                break;
            case "frog_Blip":
                source.PlayOneShot(frog_Blip);
                break;
            case "charge_Enemy_Thud":
                source.PlayOneShot(charge_Enemy_Thud);
                break;

        }

    }
    
}
