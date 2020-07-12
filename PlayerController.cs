using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public GameObject kWallPrefab;
    public GameObject kGameOverPrefab;
    public GameObject kStayAtHomePrefab;
    public GameObject kMaskApplicationPrefab;
    public GameObject kTestApplicationPrefab;
    public GameObject kSocialDistanceApplicationPrefab;
    
    public GameObject activeItemSprite;
    public TextMeshProUGUI cashText;

    public int cash = 1000;

    public AudioClip kGameOverClip;
    public AudioClip kItemPlaceClip;
    public AudioClip kItemSelectClip;
    public AudioClip kInsufficientFundsClip;

    private AudioSource gameOverAudioSource;
    private AudioSource itemPlaceAudioSource;
    private AudioSource itemSelectAudioSource;
    private AudioSource insufficientFundsAudioSource;

    private Camera mainCamera;

    private bool gameOver = false;

    enum SelectedItem {
        VerticalWall,
        HorizontalWall,
        Masks,
        SocialDistance,
        Test,
        StayAtHome
    }
    private SelectedItem selectedItem = SelectedItem.VerticalWall;
    public int kWallCost = 500;
    public int kMastCost = 200;
    public int kSocialDistancingCost = 300;
    public int kTestCost = 100;
    public int kStayAtHomeCost = 400;

    void Start() {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        gameOverAudioSource = AddAudio(kGameOverClip);    
        itemPlaceAudioSource = AddAudio(kItemPlaceClip);
        itemSelectAudioSource = AddAudio(kItemSelectClip);
        insufficientFundsAudioSource = AddAudio(kInsufficientFundsClip);
        cashText.text = "$" + cash;
    }

    private AudioSource AddAudio(AudioClip audioClip) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = audioClip;
        newAudio.loop = false;
        newAudio.playOnAwake = false;
        newAudio.volume = 1f;
        return newAudio;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && !gameOver && !PauseController.paused) {
            Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            if (PosIsInWorldBounds(pos) && BuyItem()) {
                itemPlaceAudioSource.Play();
                GameObject go = null;
                pos.z = 1;
                switch (selectedItem) {
                    case SelectedItem.VerticalWall:
                        go = Instantiate(kWallPrefab) as GameObject;
                        go.transform.localScale = new Vector3(1, 64, 1);
                        break;
                    case SelectedItem.HorizontalWall:
                        go = Instantiate(kWallPrefab) as GameObject;
                        go.transform.localScale = new Vector3(128, 1, 1);
                        break;
                    case SelectedItem.Masks:
                        go = Instantiate(kMaskApplicationPrefab) as GameObject;
                        break;
                    case SelectedItem.SocialDistance:
                        go = Instantiate(kSocialDistanceApplicationPrefab) as GameObject;
                        break;
                    case SelectedItem.Test:
                        go = Instantiate(kTestApplicationPrefab) as GameObject;
                        break;
                    case SelectedItem.StayAtHome:
                        go = Instantiate(kStayAtHomePrefab) as GameObject;
                        break;
                }
                if (go != null) {
                    go.transform.position = pos;   
                }
            }                                      
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            PressVerticalWalls();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            PressHorizontalWalls();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            PressMasks();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            PressTest();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            PressStayAtHome();
        } 
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            PressSocialDistance();
        } 

        if (HumanController.numInfected == 0 && !gameOver) {
            gameOver = true;
            gameOverAudioSource.Play();
            GameObject gameOverObj = Instantiate(kGameOverPrefab) as GameObject;
            TextMeshProUGUI gameOverText = gameOverObj.GetComponent<GameOverController>().gameOverText;
            gameOverText.text = "Infection Eradicated!\nSurvivors: " + HumanController.numHumans + "\nFatalities: " + HumanController.numDeaths;
        }
    }

    private bool BuyItem() {
        int cost = 0;
        switch (selectedItem) {
            case SelectedItem.VerticalWall:
            case SelectedItem.HorizontalWall:
                cost = kWallCost;
                break;
            case SelectedItem.Masks:
                cost = kMastCost;
                break;
            case SelectedItem.SocialDistance:
                cost = kSocialDistancingCost;
                break;
            case SelectedItem.Test:
                cost = kTestCost;
                break;
            case SelectedItem.StayAtHome:
                cost = kStayAtHomeCost;
                break;
        }
        if (cash - cost >=  0) {
            cash -= cost;
            cashText.text = "$" + cash;
            return true;
        }
        insufficientFundsAudioSource.Play();
        return false;
    }

    private bool PosIsInWorldBounds(Vector3 pos) {
        return pos.y >= -3.7; // hardcoded
    }  

    public void PressVerticalWalls() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.VerticalWall;
        activeItemSprite.transform.position = new Vector3(-7.27281f, -4.37307f, 0f); // hardcoded, come at me
    }

    public void PressHorizontalWalls() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.HorizontalWall;
        activeItemSprite.transform.position = new Vector3(-5.97f, -4.37307f, 0f);
    }

    public void PressMasks() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.Masks;
        activeItemSprite.transform.position = new Vector3(-4.672f, -4.37307f, 0f);
    }

    public void PressTest() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.Test;
        activeItemSprite.transform.position = new Vector3(-3.373f, -4.37307f, 0f);
    }

    public void PressStayAtHome() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.StayAtHome;
        activeItemSprite.transform.position = new Vector3(-2.073f, -4.37307f, 0f);
    }

    public void PressSocialDistance() {
        itemSelectAudioSource.Play();
        selectedItem = SelectedItem.SocialDistance;
        activeItemSprite.transform.position = new Vector3(-0.841f, -4.37307f, 0f);
    }
}
