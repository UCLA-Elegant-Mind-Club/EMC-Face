using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private AudioClip RightAudio;
    [SerializeField] private AudioClip WrongAudio;
    private AudioSource audioSource;

    [SerializeField] private Transform feedbackScreen;
    [SerializeField] private Transform highscoreScreen;
    private CoolText hsCoolText;

    private int streak = 0;
    private float score = 0;

    private TextMeshProUGUI messageText;
    private TextMeshProUGUI scoreChangeText;
    private TextMeshProUGUI scoreText;

    private static string[] RightMessages = { "Correct!" };
    private static string[] WrongMessages = { "Incorrect!" };
    private static string[] OvertimeMessages = { "Out of Time!" };

    [SerializeField] private Color RightColor = Color.green;
    [SerializeField] private Color WrongColor = Color.red;
    [SerializeField] private Color NeutralColor = Color.white;

    public static string ScorePath = Directory.GetCurrentDirectory() + "/Assets/Scores/";
    private string recentWinners, recentScores, topWinners, topScores;

    [SerializeField] private Transform inputDlg;
    private TextMeshProUGUI inputPrompt;
    private TMP_InputField inputField;
    private GameObject randomButton;
    private string[] randomNames;

    private void Start()
    {
        audioSource = feedbackScreen.GetComponent<AudioSource>();
        feedbackScreen.gameObject.SetActive(false);

        messageText = feedbackScreen.GetChild(0).GetComponent<TextMeshProUGUI>();
        scoreChangeText = feedbackScreen.GetChild(2).GetComponent<TextMeshProUGUI>();
        scoreText = feedbackScreen.GetChild(1).GetComponent<TextMeshProUGUI>();

        hsCoolText = highscoreScreen.GetComponent<CoolText>();
        highscoreScreen.gameObject.SetActive(false);

        loadScores();

        inputPrompt = inputDlg.GetChild(1).GetComponent<TextMeshProUGUI>();
        inputField = inputDlg.GetChild(2).GetComponent<TMP_InputField>();
        randomButton = inputDlg.GetChild(3).gameObject;
        inputDlg.gameObject.SetActive(false);

        randomNames = new string[] {"cm600286", "Powell Cat Sushi",
            "Bot" + string.Format("{0:0000}", Random.Range(0, 1000)), "Baby Monster", "Scroptimus Prime",
            "Michael Scrott", "Scrotation Crustacean", "AmogUs", "PikachYOU", "Chuck Norris", "Fast and Curious",
            "Daddy Gene", "Russian Spy", "Ariana Grande\"s Ponytail"};
    }

    public IEnumerator showInputDialog(string prompt, string placeholder, bool showRandom)
    {
        inputPrompt.text = prompt;
        inputField.text = "";
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeholder;
        randomButton.SetActive(showRandom);
        inputDlg.gameObject.SetActive(true);
        Globals.keyPressEnable = false;
        Globals.PAUSED = true;

        while (inputDlg.gameObject.activeSelf)
            yield return null;
        Globals.keyPressEnable = true;
        Globals.PAUSED = false;
    }

    public void randomizeName()
    {
        inputField.text = randomNames[Random.Range(0, randomNames.Length)];
    }

    public string inputText { get { return inputField.text; } }

    public IEnumerator showFeedback(float timeLeft, bool correct, bool practice)
    {
        feedbackScreen.gameObject.SetActive(true);
        string[] messages;
        float scoreChange = 0;
        if (correct) {
            scoreChange = Mathf.Min(timeLeft / 2 + 600 + streak * 5, 1000);
            streak = Mathf.Min(streak + 1, 10);
            messages = RightMessages;
            messageText.color = RightColor;
        }
        else {
            streak = 0;
            messageText.color = WrongColor;
            if (timeLeft <= 0)      //overtime
            {
                scoreChange = Mathf.Min(400, score);
                messages = OvertimeMessages;
            }
            else
            {
                scoreChange = Mathf.Min(timeLeft / 2, 400, score);
                messages = WrongMessages;
            }
        }

        int mult = practice || scoreChange == 0 ? 0 : correct ? 1 : -1;
        StartCoroutine(showScore(messages, mult, scoreChange));

        /*
        audioSource.clip = correct ? RightAudio : WrongAudio;
        audioSource.pitch = streak;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
        */
        yield return new WaitForSeconds(1.5f);
        feedbackScreen.gameObject.SetActive(false);
    }

    private IEnumerator showScore(string[] messages, int correct, float scoreChange)
    {
        scoreText.text = "";
        scoreChangeText.text = "(Still Warming Up)";
        messageText.text = messages[Random.Range(0, messages.Length)];
        float scoreInc = Mathf.Pow(scoreChange, 2.0f / 3);
        if (correct == 1)
        {
            scoreChangeText.color = RightColor;
            float endScore = score + scoreChange;
            scoreChangeText.text = "+" + (int)scoreChange;
            while (score < endScore)
            {
                score = Mathf.Min(score + scoreInc, endScore);
                scoreText.text = (int)score + "";
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (correct == -1)
        {
            scoreChangeText.color = WrongColor;
            float endScore = score - scoreChange;
            scoreChangeText.text = "-" + (int)scoreChange;
            while (score > endScore)
            {
                score = Mathf.Max(score - scoreInc, endScore);
                scoreText.text = (int)score + "";
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
            scoreChangeText.color = NeutralColor;
    }


    private void loadScores()
    {
        if (!Directory.Exists(ScorePath))
            Directory.CreateDirectory(ScorePath);
        FileInfo[] files = new DirectoryInfo(ScorePath).GetFiles("*.txt");
        int numFiles = files.Length;
        string[] names = new string[numFiles];
        int[] scores = new int[numFiles];

        for (int i = 0; i < numFiles; i++)
        {
            scores[i] = int.Parse(File.ReadAllText(files[i].FullName));
            names[i] = files[i].Name.Substring(14);
        }

        int len = Mathf.Min(5, numFiles);

        for (int winIndex = 0; winIndex < len; winIndex++)
        {
            recentScores += scores[numFiles - len + winIndex] + "\n";
            recentWinners += names[numFiles - len + winIndex] + "\n";
        }

        for (int winIndex = 0; winIndex < len; winIndex++)
        {
            int topIndex = 0;
            for (int i = 0; i < numFiles; i++)
                if (scores[topIndex] < scores[i])
                    topIndex = i;
            topScores += scores[topIndex] + "\n";
            topWinners += names[topIndex] + "\n";
        }

        for (int winIndex = len; winIndex < 5; winIndex++)
        {
            recentScores += "0\n";
            recentWinners += "cm600286\n";
            topScores += "0\n";
            topWinners += "cm600286\n";
        }
    }
    public IEnumerator saveScore()
    {
        yield return showInputDialog("Player Name:", "Enter username...", true);
        File.WriteAllText(ScorePath + System.DateTime.UtcNow.ToString("yyyyMMddhhmmss") + inputField.text, score + "");
    }

    public IEnumerator showHighscores(bool showRecent)
    {
        highscoreScreen.gameObject.SetActive(true);
        highscoreScreen.GetChild(0).GetComponent<TextMeshProUGUI>().text = (showRecent ? "Recent" : "Top") + " Highscores";
        highscoreScreen.GetChild(1).GetComponent<TextMeshProUGUI>().text = showRecent ? recentWinners : topWinners;
        highscoreScreen.GetChild(2).GetComponent<TextMeshProUGUI>().text = showRecent ? recentScores : topScores;
        if (hsCoolText)
        {
            hsCoolText.hideAll();
            yield return StartCoroutine(hsCoolText.play());
        }
    }

    public void hideScores()
    {
        highscoreScreen.gameObject.SetActive(false);
    }
}
