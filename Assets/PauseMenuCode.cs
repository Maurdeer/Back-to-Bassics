using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public class PauseMenuCode : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;


    public void TogglePause()
    {
        if (GameManager.Instance.GSM.IsOnState<Pause>())
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    private void Pause ()
    {
        pauseMenuPanel.SetActive(true);
        GameManager.Instance.GSM.Transition<Pause>();
    }
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        GameManager.Instance.GSM.Transition<WorldTraversal>();
    }

    public void LoadSettings ()
    {
        pauseMenuPanel.SetActive(false);

        settingsMenuPanel.SetActive(true);

        Debug.Log("Loading settings");
    }

    public void BackToPauseMenu()
    {
        settingsMenuPanel.SetActive(false);

        pauseMenuPanel.SetActive(true);
    }

    public void ExitGame ()
    {
        Application.Quit();
        Time.timeScale = 1f;
    }
}
