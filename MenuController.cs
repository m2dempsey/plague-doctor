using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    public GameObject controlsButton;

    public AudioClip kRestartAudioClip;
    public AudioClip kOutOfControlAudioClip;

    bool playPressed = false;

    AudioSource restartAudioSource;
    AudioSource outOfControlAudioSource;

    public void Start() {
        outOfControlAudioSource = AddAudio(kOutOfControlAudioClip);
        restartAudioSource = AddAudio(kRestartAudioClip);
    }

    public void PressPlay() {
        Debug.Log("Pressing play");
        if (!playPressed) {
            restartAudioSource.Play();
            Invoke("StartGame", 0.4f); // hardcoded;
            playPressed = true;
        }
    }

    void StartGame() {
        SceneManager.LoadScene("MainGame");
    }


    public void PressControls() {
        Debug.Log("Pressing controls");
        outOfControlAudioSource.Play();
        Invoke("RemoveControls", 1.35f); // hardcode
    }

    void RemoveControls() {
        Destroy(controlsButton);
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
