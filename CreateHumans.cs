using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHumans : MonoBehaviour
{
    public GameObject kHumanPrefab;
    public int kNumberPerRows = 12;
    public int kNumberOfRows = 5;

    void Start() {
        int infectedRow = Random.Range(1, kNumberOfRows);
        int infectedCol = Random.Range(1, kNumberPerRows);

        for (int i = 0; i < kNumberPerRows; i++) {
            for (int j = 0; j < kNumberOfRows; j++) {
                GameObject human = Instantiate(kHumanPrefab) as GameObject;
                human.transform.position = new Vector2(i - 8, j - 3);
                if (i == infectedCol && j == infectedRow) {
                    HumanController hc = human.GetComponent<HumanController>();
                    hc.Infect();
                }
            }
        }
    }

    void Update() {
        
    }
}
