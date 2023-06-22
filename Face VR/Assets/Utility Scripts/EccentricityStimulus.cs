using UnityEngine;

public class EccentricityStimulus : Stimulus
{
    public override void prepare<T>(T testValue)
    {
        if (testValue is float val)
        {
            Vector3 pos = this.transform.position;
            pos.x = val;
            this.transform.position = pos;
        }
        else
            throw new System.ArgumentException("Eccentricity value must be a number.");
    }
}
