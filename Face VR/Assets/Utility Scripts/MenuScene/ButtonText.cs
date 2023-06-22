using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ButtonText : Button
{
    private TextMeshProUGUI textComp;
    private string text;
    private Color origColor;

    [Header("Color when text is being selected")]
    [SerializeField] Color selectColor;

    protected override void init()
    {
        textComp = this.GetComponent<TextMeshProUGUI>();
        text = textComp.text;
        origColor = textComp.color;
    }

    public override void turnOn()
    {
        __turnOn();
        textComp.text = "[" + text + "]";
        textComp.color = selectColor;
    }

    public override void turnOff()
    {
        __turnOff();
        textComp.text = text;
        textComp.color = origColor;
    }

    public void setText(string txt)
    {
        if (isOn())
            txt = "[" + txt + "]";
        textComp.text = txt;
    }
}
