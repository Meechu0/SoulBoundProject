using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseScript : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private CanvasGroup _PausecanvasGroup;
    private CanvasGroup _ControlscanvasGroup;
    public Animator faderAnimator;
    private void Awake()
    {
        _PausecanvasGroup = GameObject.FindWithTag("Pause").GetComponent<CanvasGroup>();
        _ControlscanvasGroup = GameObject.FindWithTag("Controls").GetComponent<CanvasGroup>();
        Time.timeScale = 1;
    }

    public void Resume()
    {
        _PausecanvasGroup.alpha = 0;
        _PausecanvasGroup.interactable = false;
        _PausecanvasGroup.blocksRaycasts = false;
        _ControlscanvasGroup.alpha = 0;
        _ControlscanvasGroup.interactable = false;
        _ControlscanvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        _PausecanvasGroup.alpha = 1;
        _PausecanvasGroup.interactable = true;
        _PausecanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void ControlsSwitch()
    {
        _PausecanvasGroup.alpha = 0;
        _PausecanvasGroup.interactable = false;
        _PausecanvasGroup.blocksRaycasts = false;
        _ControlscanvasGroup.alpha = 1;
        _ControlscanvasGroup.interactable = true;
        _ControlscanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void PlayPause(InputAction.CallbackContext context)
    {
        _PausecanvasGroup = GameObject.FindWithTag("Pause").GetComponent<CanvasGroup>();
        _ControlscanvasGroup = GameObject.FindWithTag("Controls").GetComponent<CanvasGroup>();
        if (context.performed)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {

        Application.Quit();
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