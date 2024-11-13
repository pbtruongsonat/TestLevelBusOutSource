using DG.Tweening;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : View
{
    [Header("----------Base----------", order = 0)]
    [Space]
    [SerializeField] private bool showEffect;
    [SerializeField] private Transform targetTween;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private AnimationCurve introCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float blackMaskStartValue = 0;
    [SerializeField] private float blackMaskEndValue = 0.85f;
    [SerializeField] private Image blackMask;
    [SerializeField] private Button closeBtn;

    [SerializeField] protected IntVariable currentLevel;

    protected bool isAuto;

    public bool IsShowEffect => showEffect;

    public override void OnSetup()
    {
        closeBtn?.onClick.AddListener(Dismiss);
    }

    public override void OnPush(Data data)
    {
        isAuto = false;

        if (showEffect)
        {
            if (targetTween != null)
            {
                targetTween.localScale = Vector3.zero;
                targetTween.DOScale(Vector3.one, duration).SetEase(introCurve).SetUpdate(true).onComplete = OnShowEffectFinish;
            }

            if (blackMask != null)
            {
                blackMask.color = new Color(0, 0, 0, blackMaskStartValue);
                blackMask.DOFade(blackMaskEndValue, duration).SetUpdate(true);
            }
        }

        PushFinished();

    }

    public virtual void OnShowEffectFinish()
    {

    }

    public override void OnFocus()
    {

    }

    public override void OnFocusLost()
    {

    }

    public override void OnPop()
    {
        PopFinished();
    }

    public virtual void Dismiss()
    {
        if (showEffect)
        {          
            if (blackMask != null)
            {
                if (targetTween != null)
                    targetTween.localScale = Vector3.zero;

                blackMask.color = new Color(0, 0, 0, blackMaskEndValue);
                blackMask.DOFade(blackMaskStartValue, duration).OnComplete(() => PopupManager.Instance.Despawn(this));
            }
        }

        PopupManager.Instance.QueuePop();
    }
}
