using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public Animator faderAnimator;
    private musicScript _musicScript;

    private void Start()
    {
        _musicScript = FindObjectOfType<musicScript>();
        Debug.Assert(faderAnimator != null, "SCENE SWITCHER NEEDS A REFERENCE TO AN ANIMATOR WITH SCREEN FADE ANIMATIONS");
    }

    public void StartGameSwitch()
    {
        StartCoroutine(FadeOut("Game"));
        _musicScript.StopMenuMusic();


    }
    
    public void MainMenuSwitch()
    {
        StartCoroutine(FadeOut("Menu"));
    }
    
    public void OptionsSwitch()
    {
        StartCoroutine(FadeOut("Options"));
    }
    
    public void ControlsSwitch()
    {
        StartCoroutine(FadeOut("Controls"));
    }
    
    public void BackSwitch()
    {
        StartCoroutine(FadeOut(SceneManager.GetActiveScene().buildIndex - 1));
    }
    
    public void QuitSwitch()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeOut(SceneManager.GetActiveScene().name));
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
