using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskApplicationController : MonoBehaviour
{
    public float duration = 0.5f;

    void Start() {
        Invoke("End", duration);
    }

    void End() {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D (Collider2D collision) {
        if (collision.gameObject.tag == "Human") {
            HumanController other_hc = collision.gameObject.GetComponent<HumanController>();
            other_hc.WearMask();
        }
    }
}
