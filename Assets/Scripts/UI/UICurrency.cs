using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI valueText;
    [SerializeField] protected GameObject moreIcon;
    [SerializeField] private Button moreBtn;
    [SerializeField] private Image icon;

    [SerializeField] protected float timeTween = 0.5f;

    protected virtual void Awake()
    {
        if (moreBtn != null)
        {
            moreBtn.onClick.AddListener(DoMoreAction);
        }
    }

    public virtual void TweenText(int currentValue, int finalValue, float duration)
    {
        StartCoroutine(IEIncreaseAnyReference(currentValue, finalValue, duration));
    }

    private IEnumerator IEIncreaseAnyReference(float currentValue, float finalValue, float duration)
    {
        float elapsed = 0f;
        float process;

        float initValue = currentValue;

        while (elapsed <= duration)
        {
            process = Mathf.Clamp01(elapsed / duration);
            currentValue = initValue + process * (finalValue - initValue);

            valueText.text = Mathf.RoundToInt(currentValue).ToString();

            elapsed += Time.deltaTime;
            yield return null;
        }

        valueText.text = Mathf.RoundToInt(finalValue).ToString();
    }

    protected virtual void DoMoreAction()
    {

    }

    public virtual void Active(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public virtual void ActiveMoreIcon(bool isActive)
    {
        moreIcon.SetActive(isActive);
    }
}
