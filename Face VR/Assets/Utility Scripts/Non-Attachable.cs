using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Varjo.XR;

public static class Globals
{
    public static string dataPath = Directory.GetCurrentDirectory() + "/Data/";
    public static bool PAUSED = false;
    public static bool keyPressEnable = true;
    // Start is called before the first frame update

    public static long time()
    {
        return System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public static long varjoTime(long varjoTime)
    {
         return ((System.DateTimeOffset)VarjoTime.ConvertVarjoTimestampToDateTime(varjoTime)).ToUnixTimeMilliseconds();
    }

    public static IEnumerator waitKey(KeyCode key)
    {
        while (!GetKeyDown(key)) yield return null;
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return keyPressEnable && Input.GetKeyDown(key);
    }

    public static float eccentCalc(float angle, float distance, bool symmetric)
    {
        if (symmetric)
            return 2 * Mathf.Tan(angle * Mathf.Deg2Rad / 2) * distance;
        else
            return Mathf.Tan(angle * Mathf.Deg2Rad) * distance;
    }

    public static float eccentCalc(float angle, float distance)
    {
        return eccentCalc(angle, distance, true);
    }

    public static T[] RandomArray<T>(T[] testValues, int totalTrials)
    {
        int trialsPerValue = (int)Mathf.Ceil(totalTrials / testValues.Length);
        T[] testArray = new T[totalTrials];
        int numValues = testValues.Length;
        int[] reps = new int[numValues];
        for (int i = 0; i < numValues; i++)
            reps[i] = i * 1000 + trialsPerValue;

        //Is this an elegant randomization + initialization combo or not? O_o
        for (int i = 0; i < totalTrials; i++)
        {
            int rand = Random.Range(0, numValues);
            testArray[i] = testValues[reps[rand] / 1000];
            if (--reps[rand] % 1000 <= 0 && numValues > 0)
                reps[rand] = reps[--numValues];
        }
        return testArray;
    }
    public static int[] RandomArray(int numValues, int totalTrials)
    {
        int[] nums = new int[numValues];
        for (int i = 0; i < numValues; i++) nums[i] = i;
        return RandomArray(nums, totalTrials);
    }

    public static void activateAllChildren(Transform trans)
    {
        trans.gameObject.SetActive(true);
        foreach (Transform child in trans)
            activateAllChildren(child);
    }
}

public interface ProtocolStep
{
    public abstract IEnumerator runStep();
}

public class TrialResult
{
    public static string headerString = "Correct,TestValue,StartTime,EndTime,Target";
    public bool correct;
    public float testValue;
    public long startTime;
    public long endTime;
    public KeyCode keyPressed;
    public int target;

    public long responseTime { get { return endTime - startTime; } }
    public string recordString
    {
        get { return string.Join(",", correct ? 1 : 0, testValue, startTime, endTime, target); }
    }

    public string debugString
    {
        get
        {
            return "Correct: " + correct + ", Test Value: " + testValue +
        ", Response Time: " + responseTime + ", Target: " + target;
        }
    }
}

[System.Serializable]
public class StimulusSet : IEnumerable<Stimulus>
{
    [SerializeField] private StimInitializer[] stimuli;
    public int Length { get { return stimuli.Length; } }

    public void init()
    {
        foreach(StimInitializer stim in stimuli)
        {
            stim.stim.mapKey = stim.mapKey;
            stim.stim.idNum = stim.idNum;
            stim.stim.gameObject.SetActive(false);
        }
    }

    public IEnumerator<Stimulus> GetEnumerator()
    {
        foreach (StimInitializer stim in stimuli)
            yield return stim.stim;
    }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public KeyCode getKeyDown()
    {
        foreach (StimInitializer stim in stimuli)
            if (Input.GetKeyDown(stim.mapKey))
                return stim.mapKey;
        return KeyCode.None;
    }

    public Stimulus getStim(int index)
    {
        return stimuli[index].stim;
    }

    [System.Serializable]
    private struct StimInitializer
    {
        [SerializeField] public Stimulus stim;
        [SerializeField] public KeyCode mapKey;
        [SerializeField] public int idNum;
    }
}

public abstract class Stimulus : MonoBehaviour
{
    [System.NonSerialized] public KeyCode mapKey;
    [System.NonSerialized] public int idNum;
    public abstract void prepare<T>(T testValue);

    private void Start()
    {
    }

    public void Reset()
    {
        idNum = this.transform.GetSiblingIndex();
    }
}