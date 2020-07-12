using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI gameOverText; 

    public AudioClip kRestartAudioClip;

    AudioSource restartAudioSource;

    private bool playPressed = false;

    public void Start() {
        restartAudioSource = AddAudio(kRestartAudioClip);
    }

    public void PressRestart() {
        if (!playPressed) {
            restartAudioSource.Play();
            Invoke("Restart", 0.4f); // hardcoded
            playPressed = true;
        }
    }

    void Restart() {
        Debug.Log("Restart!");
        HumanController.numHumans = 0;
        HumanController.numDeaths = 0;
        HumanController.numCritical = 0;
        HumanController.numInfected = 0;
        SceneManager.LoadScene("MainGame");
    }

    public void PressQuit() {
        Debug.Log("Pressing quit");
        Application.Quit();
    }

    private AudioSource AddAudio(AudioClip audioClip) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = audioClip;
        newAudio.loop = false;
        newAudio.playOnAwake = false;
        newAudio.volume = 1f;
        return newAudio;
    }
}
