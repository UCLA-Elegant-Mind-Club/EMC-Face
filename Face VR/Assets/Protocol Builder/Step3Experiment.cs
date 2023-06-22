using System.Collections;
using UnityEngine;

public class Step3Experiment : MonoBehaviour, ProtocolStep
{
    [SerializeField] private int numDummyTrials;
    [SerializeField] public int numTrials;
    [SerializeField] private int setNumber;
    private ProtocolControl protoControl;
    TrialResult trialRes;

    public void Start()
    {
        protoControl = this.transform.parent.GetComponent<ProtocolControl>();
        trialRes = protoControl.trialRes;
    }

    public IEnumerator runStep()
    {
        yield return null;
        Debug.Log("Experimental Round");

        for (int dummyNum = 0; dummyNum < numDummyTrials; dummyNum++)
        {
            yield return StartCoroutine(protoControl.dummyStimTest(setNumber));
            Debug.Log("DummyTrial: " + trialRes.debugString);
        }

        for (int trialNum = 0; trialNum < numTrials; trialNum += trialRes.correct ? 1 : 0)
        {
            yield return StartCoroutine(protoControl.expStimTest(setNumber));
            Debug.Log("Trial " + (trialNum + 1) + ", " + trialRes.debugString);
            protoControl.trialResponse();
        }
    }
}
