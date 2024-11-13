using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected GameObject offObj;
    [SerializeField] protected ToggleScript toggle;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Tracking();
            DoToggle();
        });
    }

    private void OnEnable()
    {
        UpdateVisual();
    }

    protected virtual void DoToggle()
    {

    }

    protected virtual void UpdateVisual()
    {

    }

    protected virtual void Tracking()
    {

    }
}
