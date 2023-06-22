using UnityEngine;
using UnityEngine.Events;

public abstract class Button : MonoBehaviour
{
    private bool on;

    [Header("Function to run when selected")]
    [SerializeField] private UnityEvent action;

    // Start is called before the first frame update
    public void Start() { on = false; init(); }
    protected abstract void init();
    protected void __turnOn() { on = true; }
    protected void __turnOff() { on = false; }
    public abstract void turnOn();
    public abstract void turnOff();
    public bool isOn() { return on; }
    public void act() { action.Invoke(); }
}