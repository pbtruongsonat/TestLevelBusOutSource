using DG.Tweening;
using Obvious.Soap.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickedButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private KeySound key = KeySound.None;
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private Vector3 clickScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private bool isScale = true;
    [SerializeField] private float duration = 0.09f;
    [SerializeField] private bool tracking;

    [ShowIf("tracking", true)]
    [SerializeField] private string iconName;
    [ShowIf("tracking", true)]
    [SerializeField] private string screen;
    [ShowIf("tracking", true)]
    [SerializeField] private string placement;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound(key);

        if (tracking)
        {
            //SonatTracking.ClickIcon(iconName, DataManager.Instance.playerData.saveLevelData.currentLevel, screen, placement);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isScale)
        {
            transform.DOScale(hoverScale, duration).SetUpdate(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isScale)
        {
            transform.DOScale(defaultScale, duration).SetUpdate(true);       
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isScale) 
        { 
            transform.DOScale(defaultScale, duration).SetUpdate(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isScale)
        {
            transform.DOScale(clickScale, duration).SetUpdate(true);
        }
    }
}
