using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class story : MonoBehaviour
{
    public Animator faderAnimator;
    private void OnEnable()
    {
        StartCoroutine(FadeOut("TitleScreen"));
    }
    IEnumerator FadeOut(string sceneName)
    {
        faderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(faderAnimator.GetCurrentAnimatorClipInfo(0).Length);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut(int sceneIndex)
    {
        faderAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(faderAnimator.GetCurrentAnimatorClipInfo(0).Length);
        SceneManager.LoadScene(sceneIndex);
    }
}
