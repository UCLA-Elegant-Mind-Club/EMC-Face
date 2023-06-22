using UnityEngine;

public class ScalingStimulus : Stimulus
{
    public override void prepare<T>(T testValue)
    {
        if (testValue is float val)
        {
            float eccCalc = Globals.eccentCalc(val, (Camera.main.transform.position - this.transform.position).magnitude);
            eccCalc = Globals.eccentCalc(eccCalc, (Camera.main.transform.position - this.transform.position).magnitude);
            this.transform.localScale = Vector3.one * eccCalc;
            Debug.Log(eccCalc);
        }
        else
            throw new System.ArgumentException("Scaling value must be a number.");
    }
}