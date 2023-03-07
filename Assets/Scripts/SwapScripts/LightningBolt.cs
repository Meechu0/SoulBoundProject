using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public GameObject xLightningBolt;
    // Start is called before the first frame update
    private void DisableBolt()
    {
        xLightningBolt.GetComponent<SpriteRenderer>().enabled = false;

    }
    private void EnableBolt()
    {
        xLightningBolt.GetComponent<SpriteRenderer>().enabled = true;
    }

}
