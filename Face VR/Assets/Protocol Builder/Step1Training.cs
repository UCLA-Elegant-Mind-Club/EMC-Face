using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Step1Training : MonoBehaviour, ProtocolStep
{
    [SerializeField] private float showTime = 5;
    [SerializeField] private int repetitions = 2;
    [SerializeField] private int setNumber;
    private WaitForSeconds showWaiter;
    private ProtocolControl protoControl;

    public void Start()
    {
        showWaiter = new WaitForSeconds(showTime);
        protoControl = this.transform.parent.GetComponent<ProtocolControl>();
    }

    public IEnumerator runStep()
    {
        Debug.Log("Training Set " + setNumber);
        yield return null;
        for (int i = repetitions; i > 0; i--)
        {
            foreach (Stimulus stim in protoControl.getSet(setNumber))
            {
                protoControl.genDisplay("The following stimulus will be mapped to the keyboard button \'" +
                    stim.mapKey + "\'\n\n" + "Press \'" + stim.mapKey + "\' to continue.");
                yield return Globals.waitKey(stim.mapKey);
                protoControl.genDisplay("");
                stim.prepare(protoControl.refValue);
                stim.gameObject.SetActive(true);
                yield return showWaiter;
                stim.gameObject.SetActive(false);
            }
            if (i > 1)
            {
                protoControl.genDisplay("This training will repeat " + (i - 1) +
                    " more time" + (i > 2 ? "s" : "") + ".\n\nPress [space] to continue.");
                yield return Globals.waitKey(KeyCode.Space);
            }
        }
        protoControl.genDisplay("");
    }
}
