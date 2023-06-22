using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tester : MonoBehaviour
{
    [SerializeField] private UnityEvent uEvent;

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
            uEvent.Invoke();
        Debug.Log("");
    }

    public void canvasControllerTest()
    {
        if (!Input.GetKeyDown(KeyCode.Alpha1)) return;
        CanvasController obj = gameObject.GetComponent<CanvasController>();
        StartCoroutine(obj.playScreens());
    }

    public void scoreManTest()
    {
        ScoreManager obj = gameObject.GetComponent<ScoreManager>();
        if (Globals.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(obj.showInputDialog("Test", "placeholder?", true));
        if (Globals.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(obj.showFeedback(600, true, false));
        if (Globals.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(obj.showFeedback(600, false, true));
        if (Globals.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(obj.showFeedback(600, true, true));
        if (Globals.GetKeyDown(KeyCode.Alpha5))
            StartCoroutine(obj.showFeedback(600, false, false));
    }
}
