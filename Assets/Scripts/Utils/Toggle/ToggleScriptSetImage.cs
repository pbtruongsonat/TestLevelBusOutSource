using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptSetImage : ToggleScript
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite imageOn;
    [SerializeField] private Sprite imageOff;

    public override void OnChanged(bool val)
    {
        image.sprite = val ? imageOn : imageOff;
    }

    public void SetSprite(Sprite imageOn, Sprite imageOff)
    {
        this.imageOn = imageOn;
        this.imageOff = imageOff;
    }
}
