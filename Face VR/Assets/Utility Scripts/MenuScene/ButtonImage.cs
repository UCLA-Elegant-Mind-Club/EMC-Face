using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonImage : Button
{
    private Image image;
    private Sprite origImage;

    [Header("Image when being selected")]
    [SerializeField] Sprite selectImage;

    // Start is called before the first frame update

    protected override void init()
    {
        image = this.GetComponent<Image>();
        origImage = image.sprite;
    }

    public override void turnOn()
    {
        __turnOn();
        image.sprite = selectImage;
    }

    public override void turnOff()
    {
        __turnOff();
        image.sprite = origImage;

    }
}
