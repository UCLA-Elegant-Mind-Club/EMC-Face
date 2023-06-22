using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private enum ScreenMethod { timed, keyed };
    [Header("Should have one screen type per child canvas")]
    [SerializeField] private ScreenType[] screenTypes;
    private bool pauseControl = false, nextTrigger = false;

    public void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
            if (screenTypes[i].maxShowTime == 0 && screenTypes[i].continueKey != KeyCode.None)
                screenTypes[i].maxShowTime = float.MaxValue;
        }
    }
    public IEnumerator playScreens()
    {
        for (int i = 0; i < transform.childCount; i++)
            yield return StartCoroutine(playScreen(i));
    }

    private IEnumerator playScreen(int index)
    {
        Transform screen = transform.GetChild(index);
        screen.gameObject.SetActive(true);
        for (float time = 0; time < screenTypes[index].maxShowTime; time += pauseControl ? 0 : Time.deltaTime)
        {
            yield return null;
            if (Globals.GetKeyDown(screenTypes[index].continueKey) || nextTrigger)
                break;
        }
        screen.gameObject.SetActive(false);
        nextTrigger = false;
    }

    public void nextScreen()
    {
        nextTrigger = true;
    }
    public void setPause(bool pause)
    {
        pauseControl = pause;
    }

    public void Reset()
    {
        screenTypes = new ScreenType[transform.childCount];
    }

    [System.Serializable]
    private struct ScreenType
    {
        [SerializeField] public KeyCode continueKey;
        [SerializeField] public float maxShowTime;
    }
}
