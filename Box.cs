using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {

    private TouchInput touchInput;

    void Start () {
        touchInput = FindObjectOfType<TouchInput> ();
    }

    void FixedUpdate () {
        transform.position = touchInput.TouchToWorldPoint (touchInput.touchPosition);
    }
}