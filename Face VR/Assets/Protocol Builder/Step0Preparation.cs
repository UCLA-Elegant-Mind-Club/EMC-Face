using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Step0Preparation : MonoBehaviour, ProtocolStep
{
    [Header("Leave fields empty to omit step.")]
    [SerializeField] private CanvasController instructScreens;
    [SerializeField] private Demo demo;
    [SerializeField] private Calibration calib;
    private ProtocolControl protoControl;
    private WaitForSeconds demoWaiter;
    private WaitForSeconds calibWaiter;

    public void Start()
    {
        protoControl = this.transform.parent.GetComponent<ProtocolControl>();
        if (demo.demoScreen)
        {
            demo.demoStim.gameObject.SetActive(false);
            demoWaiter = new WaitForSeconds(demo.demoFrameTime);
        }
        if (calib.calibrationScreen)
        {
            calib.calibStim.gameObject.SetActive(false);
            calibWaiter = new WaitForSeconds(calib.calibrationTime);
        }
    }

    public IEnumerator runStep()
    {
        if(instructScreens)
            yield return instructScreens.playScreens();
        if(demo.demoScreen)
        {
            yield return demo.demoScreen.playScreens();
            demo.demoStim.gameObject.SetActive(true);
            foreach(float val in protoControl.testValues)
            {
                demo.demoStim.prepare(val);
                yield return demoWaiter;
            }
            demo.demoStim.gameObject.SetActive(false);
        }
        if(calib.calibrationScreen)
        {
            yield return calib.calibrationScreen.playScreens();
            calib.calibStim.prepare(protoControl.refValue);
            calib.calibStim.gameObject.SetActive(true);
            long startTime = Globals.time();
            yield return calibWaiter;
            calib.calibStim.gameObject.SetActive(false);
            protoControl.csvOutput("-1,-1," + startTime + "," + Globals.time() + ",-1");
        }
    }

    [System.Serializable]
    private struct Demo
    {
        public CanvasController demoScreen;
        public Stimulus demoStim;
        public float demoFrameTime;
    }

    [System.Serializable]
    private struct Calibration
    {
        public CanvasController calibrationScreen;
        public Stimulus calibStim;
        public float calibrationTime;
    }
}
