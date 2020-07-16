using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {

    [SerializeField] private float Deadzone = .5f;

    public string phaseText { get; private set; }
    public float timeTouchEnded { get; private set; }
    public float touchDistance { get => Vector2.Distance (touchStartPosition, touchEndPosition); }
    public float touchTime { get; private set; }
    public Vector2 touchDirection { get; private set; }
    public Vector2 touchPosition { get; private set; }

    private Touch theTouch;
    private float dipslayTime = .5f;
    private Vector2 touchStartPosition, touchEndPosition;
    private GUIStyle labelStyle = new GUIStyle ();

    private void Start () {
        labelStyle.fontSize = 75;
    }

    private void Update () {
        if (Input.touchCount > 0) {
            // TOUCH PHASE
            theTouch = Input.GetTouch (0);
            phaseText = theTouch.phase.ToString ();
            touchPosition = theTouch.position;
            if (theTouch.phase == TouchPhase.Began) {
                touchStartPosition = theTouch.position;
                touchTime = 0;
            } else if (theTouch.phase == TouchPhase.Moved) {
                touchTime += Time.deltaTime;
                touchDirection = theTouch.position - touchStartPosition;
            } else if (theTouch.phase == TouchPhase.Ended) {
                timeTouchEnded = Time.time;
                touchEndPosition = theTouch.position;
                touchDirection = theTouch.position - touchStartPosition;
            }
        } else if (Time.time - timeTouchEnded > dipslayTime) {
            phaseText = string.Empty;
            touchEndPosition = touchStartPosition = touchDirection = touchPosition = Vector2.zero;
        }
    }
    public TouchState GetTouchState () {
        if (Click ()) return TouchState.clicked;

        if (SwipeVertical () && SwipeHorizontal ()) {
            if (SwipeUp ()) {
                if (SwipeRight ()) return TouchState.swipeUpRight;
                else return TouchState.swipeUpLeft;
            } else {
                if (SwipeRight ()) return TouchState.swipeDownRight;
                else return TouchState.swipeDownLeft;
            }
        }

        if (SwipeHorizontal ()) {
            return SwipeRight () ? TouchState.swipeRight : TouchState.swipeLeft;
        }
        if (SwipeVertical ()) {
            return SwipeUp () ? TouchState.swipeUp : TouchState.swipeDown;
        }

        return TouchState.noTouch;
    }

    public Vector3 TouchToWorldPoint (Vector3 position) {
        if (GetTouchState () == TouchState.noTouch) return Vector3.zero;
        Vector3 _pos = position;
        // OFFSET
        _pos -= Camera.main.transform.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint (_pos);
        return worldPosition;

    }

    private bool Click () {
        return touchDistance > 0 && touchDistance < Deadzone * 1000;
    }

    private void OnDrawGizmos () {
        // DRAW
        Vector3 pos = TouchToWorldPoint (theTouch.position);
        Vector3 startPos = TouchToWorldPoint (touchStartPosition);
        Gizmos.DrawLine (startPos, pos);
        Gizmos.DrawSphere (startPos, 0.1f);
        Gizmos.DrawSphere (pos, 0.1f);
        //Debug.DrawLine (startPos, pos, Color.green);
    }
    private void OnGUI () {
        GUI.Label (new Rect (50, 25, 150, 200), "Phase:" + phaseText, labelStyle);
        GUI.Label (new Rect (50, 100, 150, 200), "Touch Position:" + touchPosition, labelStyle);
        GUI.Label (new Rect (50, 175, 150, 200), "Time Touched: " + timeTouchEnded.ToString (), labelStyle);
        GUI.Label (new Rect (50, 250, 150, 200), "Distance: " + touchDistance.ToString (), labelStyle);
        GUI.Label (new Rect (50, 325, 150, 200), "Last Touch Time: " + touchTime.ToString (), labelStyle);
        GUI.Label (new Rect (50, 400, 150, 200), "Touch: " + touchDirection, labelStyle);
        GUI.Label (new Rect (50, 475, 150, 200), "Touch Info: " + GetTouchState (), labelStyle);
    }

    private bool SwipeUp () {
        return touchDirection.y > 0;
    }

    private bool SwipeRight () {
        return touchDirection.x > 0;
    }

    private bool SwipeVertical () {
        return Mathf.Abs (touchDirection.normalized.y) > Deadzone;
    }

    private bool SwipeHorizontal () {
        return Mathf.Abs (touchDirection.normalized.x) > Deadzone;
    }
}

public enum TouchState {
    noTouch,
    clicked,
    swipeUp,
    swipeDown,
    swipeRight,
    swipeLeft,
    swipeUpRight,
    swipeUpLeft,
    swipeDownRight,
    swipeDownLeft
}