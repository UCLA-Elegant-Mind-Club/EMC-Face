using System.Collections;
using UnityEngine;
using TMPro;
using System.IO;

public class ProtocolControl : MonoBehaviour
{
    [SerializeField] private string ExperimentName;
    [SerializeField] private StimulusSet[] stimulusSets;
    [SerializeField] private Cross cross;
    [SerializeField] private TextMeshProUGUI genMessage;

    private ProtocolStep[] protocol;

    [Header("If this don't work, comment out line 10 in ScalingStimulus.cs")]
    [SerializeField] private float[] _testValues;
    private float[] testArray;
    [SerializeField] private float _refValue = 0;
    [SerializeField] private float timeOut = 1.2f;

    [SerializeField] private ScoreManager scoreMan;
    private string fileName;

    public TrialResult trialRes = new TrialResult();
    private int trialNum, totalTrials;

    public float refValue { get { return _refValue; } }
    public float[] testValues { get { return _testValues; } }

    public void Awake()
    {
        Globals.activateAllChildren(this.transform.parent);
    }

    public void Start()
    {
        foreach (StimulusSet set in stimulusSets)
            set.init();
        cross.init();

        protocol = this.GetComponentsInChildren<ProtocolStep>();
        foreach (ProtocolStep step in protocol)
            if (step is Step3Experiment expStep)
                totalTrials += expStep.numTrials;

        testArray = Globals.RandomArray(testValues, totalTrials);
        StartCoroutine(main());
    }

    private void initFile(string expName, string partName)
    {
        if (!Directory.Exists(Globals.dataPath))
            Directory.CreateDirectory(Globals.dataPath);
        expName = Path.Combine(Globals.dataPath, expName);
        if (!Directory.Exists(expName))
            Directory.CreateDirectory(expName);
        this.fileName = Path.Combine(expName, partName + '(' + System.DateTime.UtcNow.ToString("MM-dd-HH-mm") + ')') + ".csv";
        Debug.Log(this.fileName);

        File.WriteAllText(fileName, TrialResult.headerString + "\n");
    }

    public void genDisplay(string str)
    {
        genMessage.text = str;
    }

    private IEnumerator stimTest(int set, int target, float testValue, bool practice)
    {
        yield return StartCoroutine(cross.showCross());
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        Stimulus stim = getSet(set).getStim(target);
        trialRes.target = stim.idNum;
        trialRes.testValue = testValue;
        stim.prepare(testValue);
        stim.gameObject.SetActive(true);
        yield return StartCoroutine(timeKeyPress(getSet(set), timeOut));
        stim.gameObject.SetActive(false);
        trialRes.correct = trialRes.keyPressed == stim.mapKey;
        yield return StartCoroutine(scoreMan.showFeedback(timeOut - trialRes.responseTime,
            trialRes.correct, practice));
    }
    #region helperStimTests
    //For practice trials , only target is specified
    public IEnumerator practiceStimTest(int set, int target)
    {
        yield return StartCoroutine(stimTest(set, target, _refValue, true));
    }

    //For dummy trials, both target and testValue are random
    public IEnumerator dummyStimTest(int set)
    {
        yield return StartCoroutine(stimTest(set, Random.Range(0, getSet(set).Length),
            _testValues[Random.Range(0, _testValues.Length)], true));
    }

    //For experimental trials, testValue is derived from trial number
    public IEnumerator expStimTest(int set)
    {
        yield return StartCoroutine(stimTest(set, Random.Range(0, getSet(set).Length),
            testArray[trialNum], false));
    }
    #endregion

    public void trialResponse()
    {
        csvOutput(trialRes.recordString);

        if (!trialRes.correct)
        {
            int index = Random.Range(trialNum, totalTrials);
            testArray[trialNum] = testArray[index];
            testArray[index] = trialRes.testValue;
        }
        else
            trialNum++;
    }

    public void csvOutput(string output)
    {
        if (fileName != null)
            File.AppendAllText(fileName, output + "\n");
    }

    public StimulusSet getSet(int set)
    {
        return stimulusSets[set - 1];
    }
    
    public IEnumerator timeKeyPress(StimulusSet set, float timeOut)
    {
        trialRes.keyPressed = KeyCode.None;
        trialRes.startTime = Globals.time();
        for (float time = 0; time < timeOut;)
        {
            while (Globals.PAUSED) yield return null;
            trialRes.keyPressed = set.getKeyDown();
            if (trialRes.keyPressed == KeyCode.None)
                yield return null;
            else
                break;
            time += Time.deltaTime;
        }
        trialRes.endTime = Globals.time();
    }

    public IEnumerator main()
    {
        yield return scoreMan.showInputDialog("Participant:", "Enter code...", false);
        if (scoreMan.inputText != "")
            initFile(ExperimentName, scoreMan.inputText);

        foreach (ProtocolStep step in protocol)
        {
            Debug.Log(step);
            yield return step.runStep();
        }

        yield return scoreMan.saveScore();
    }

    [System.Serializable]
    public class Cross
    {
        [SerializeField] GameObject crossObject;
        [SerializeField] float prePause;
        [SerializeField] float onDuration;
        [SerializeField] float postPause;
        private WaitForSeconds preWaiter, onWaiter, postWaiter;

        public void init()
        {
            crossObject.SetActive(false);
            preWaiter = new WaitForSeconds(prePause);
            onWaiter = new WaitForSeconds(onDuration);
            postWaiter = new WaitForSeconds(postPause);
        }

        public IEnumerator showCross()
        {
            yield return preWaiter;
            crossObject.SetActive(true);
            yield return onWaiter;
            crossObject.SetActive(false);
            yield return postWaiter;
        }
    }
}
