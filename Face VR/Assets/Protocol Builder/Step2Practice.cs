using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class Step2Practice : MonoBehaviour, ProtocolStep
{
    [SerializeField] private int prePracticeBreak;
    [SerializeField] private int postPracticeBreak;

    [SerializeField] private int numTrials;
    [SerializeField] private int setNumber;
    private ProtocolControl protoControl;

    public void Start()
    {
        protoControl = this.transform.parent.GetComponentInParent<ProtocolControl>();
    }
    public IEnumerator runStep()
    {
        Debug.Log("Practice Set " + setNumber);

        numTrials = (int)Mathf.Min(numTrials, 6);
        int[] practiceStim = Globals.RandomArray(protoControl.getSet(setNumber).Length, numTrials);

        Step0Break breakScreen = this.gameObject.AddComponent<Step0Break>();

        breakScreen.init("Practice round starts in %time seconds\n\nNumber of practice trials: "
            + numTrials + "\n\nNumber of trials left in experiment:", prePracticeBreak);
        yield return breakScreen.runStep();

        foreach (int stimNum in practiceStim)
            yield return StartCoroutine(protoControl.practiceStimTest(setNumber, stimNum));

        breakScreen.init("Experiment starts in %time seconds\n\nNumber of trials left in experiment:", prePracticeBreak);
        yield return breakScreen.runStep();

        Destroy(breakScreen);
    }
}
