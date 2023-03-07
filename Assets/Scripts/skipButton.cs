using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class skipButton : MonoBehaviour
{
    public PlayableDirector _playableDirector;
    public Animator faderAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void skipPrologue()
    {
        StartCoroutine(FadeOut("TitleScreen"));
    }
    
    IEnumerator FadeOut(string sceneName)
    {
        faderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(faderAnimator.GetCurrentAnimatorClipInfo(0).Length);
        SceneManager.LoadScene(sceneName);
    }
}
