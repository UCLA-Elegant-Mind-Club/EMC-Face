using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    [Header("List of togglable buttons")]
    [SerializeField] GameObject[] buttons;

    [Header("Key to go to next option")]
    [SerializeField] KeyCode[] nextKeys = { KeyCode.RightArrow, KeyCode.DownArrow };
    [Header("Key to go to previous option")]
    [SerializeField] KeyCode[] backKeys = { KeyCode.LeftArrow, KeyCode.UpArrow };
    [Header("Key to select option")]
    [SerializeField] KeyCode[] selectKeys = { KeyCode.Space };

    private int index;
    private Button[] buttonComps;
    private int numButtons;

    void Start()
    {
        index = 0;
        numButtons = buttons.Length;
        buttonComps = new Button[numButtons];
        for (int i = 0; i < numButtons; i++)
            buttonComps[i] = buttons[i].GetComponent<Button>();
        StartCoroutine(selectFirst());
    }

    // Update is called once per frame
    void Update()
    {
        int res = 0;
        foreach (KeyCode key in nextKeys)
            res += Globals.GetKeyDown(key) ? 1 : 0;
        foreach (KeyCode key in backKeys)
            res += Globals.GetKeyDown(key) ? -1 : 0;

        if (res != 0)
        {
            buttonComps[index].turnOff();
            index = (index + numButtons + res) % numButtons;
            buttonComps[index].turnOn();
        }

        foreach (KeyCode key in selectKeys)
            if (Globals.GetKeyDown(key))
                buttonComps[index].act();
    }

    private IEnumerator selectFirst()
    {
        yield return new WaitForFixedUpdate();
        buttonComps[0].turnOn();
    }
}
