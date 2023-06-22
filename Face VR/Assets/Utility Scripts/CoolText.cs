using System.Collections;
using TMPro;
using UnityEngine;

public class CoolText : MonoBehaviour
{
    private WaitForSeconds charWaiter;
    [SerializeField] private float charDelay = 0.05f;
    private WaitForSeconds wordWaiter;
    [SerializeField] private float wordDelay = 0;
    private WaitForSeconds lineWaiter;
    [SerializeField] private float lineDelay = 0;
    void Start()
    {
        charWaiter = new WaitForSeconds(charDelay);
        wordWaiter = new WaitForSeconds(wordDelay);
        lineWaiter = new WaitForSeconds(lineDelay);
    }

    public void hideAll()
    {
        foreach (Transform child in this.transform)
            child.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        hideAll();
        StartCoroutine(play());
    }

    public IEnumerator play()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
            TextMeshProUGUI txtObj = child.GetComponent<TextMeshProUGUI>();
            if (txtObj && charDelay > 0) yield return StartCoroutine(playText(txtObj));
            yield return lineWaiter;
        }
    }

    private IEnumerator playText(TextMeshProUGUI txt)
    {
        int numChars = txt.text.Length;
        for (txt.maxVisibleCharacters = 1; txt.maxVisibleCharacters < numChars;
                txt.maxVisibleCharacters++)
        {
            if (txt.text[txt.maxVisibleCharacters] == ' ')
            {
                yield return wordWaiter;
                while (txt.maxVisibleCharacters < numChars && txt.text[txt.maxVisibleCharacters] == ' ')
                    txt.maxVisibleCharacters++;
            }
            if (txt.text[txt.maxVisibleCharacters] == '\n')
            {
                yield return lineWaiter;
                while (txt.maxVisibleCharacters < numChars && txt.text[txt.maxVisibleCharacters] == '\n')
                    txt.maxVisibleCharacters++;
            }
            yield return charWaiter;
        }
    }
}
