using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step0Break : MonoBehaviour, ProtocolStep
{
    [SerializeField] private int waitTime;
    private WaitForSeconds secWaiter;

    [Header("Type %time where time should be substituted into string."), TextArea(4, 4)]
    [SerializeField] private string text;
        
    private ProtocolControl protoControl;

    private void Awake()
    {
        protoControl = this.transform.parent.GetComponentInParent<ProtocolControl>();
        secWaiter = new WaitForSeconds(1);
    }

    public void init(string str, int time)
    {
        text = str;
        waitTime = time;
    }

    public IEnumerator runStep()
    {
        for (int i = waitTime; i > 0; i--)
        {
            protoControl.genDisplay(text.Replace("%time", i + ""));
            yield return secWaiter;
        }
        protoControl.genDisplay("");
    }
}
