using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseUI;

    public static bool paused = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && HumanController.numInfected > 0) {
            if (!paused) {
                Debug.Log("Hey, I'm pausin here!");
                pauseUI.SetActive(true);
                Time.timeScale = 0f;
                paused = true;
            } else {
                PressResume(); // unpause
            } 
        }
    }

    public void PressResume() {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void PressRestart() {
        Debug.Log("Restart!");
        HumanController.numHumans = 0;
        HumanController.numDeaths = 0;
        HumanController.numCritical = 0;
        HumanController.numInfected = 0;
        PressResume(); // unpause
        SceneManager.LoadScene("MainGame");
    }

    public void PressQuit() {
        Debug.Log("Pressing quit");
        Application.Quit();
    }
}
