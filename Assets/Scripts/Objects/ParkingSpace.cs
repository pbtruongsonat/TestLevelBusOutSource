using DG.Tweening;
using Obvious.Soap;
using Obvious.Soap.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

public class ParkingSpace : MonoBehaviour
{
    [SerializeField] private ParkingSpaceStatus currentStatus;

    [SerializeField] private GameObject parkingLock;
    [SerializeField] private GameObject parkingVip;
    [SerializeField] private GameObject parkingOpen;
    [SerializeField] private SpriteRenderer display;
    [SerializeField] private SpriteRenderer vipDisplay;

    [SerializeField] private bool isVipParking;

    [ShowIf("isVipParking", true)]
    [SerializeField] private ParticleSystem vipIdleEffect;

    public Transform entrance;

    public ParkingSpaceStatus CurrentStatus => currentStatus;

    private bool isWarning;

    public void ChangeSpaceStatus(ParkingSpaceStatus status)
    {
        currentStatus = status;
    }

    public void Unlock()
    {
        currentStatus = ParkingSpaceStatus.Empty;

        parkingOpen.SetActive(true);
        parkingLock.SetActive(false);
        parkingVip.SetActive(false);
    }

    public void Lock()
    {
        currentStatus = ParkingSpaceStatus.Lock;

        parkingLock.SetActive(true);
        parkingOpen.SetActive(false);
        parkingVip.SetActive(false);
    }

    public void UnlockVip()
    {
        gameObject.SetActive(true);

        currentStatus = ParkingSpaceStatus.Empty;

        parkingLock.SetActive(false);
        parkingOpen.SetActive(false);
        parkingVip.SetActive(true);    
    }

    public void ActiveSortingGroup(bool isActive)
    {
        //if (!isVipParking) return;
        
        if (TryGetComponent(out SortingGroup sortingGroup))
        {
            sortingGroup.enabled = isActive;
        }
    }

    public void PlayIdleEffect(bool isPlay)
    {
        if (!isVipParking) return;

        if (isPlay)
        {
            vipIdleEffect.Play();
        }
        else
        {
            vipIdleEffect.Stop();
        }
    }

    public void Warning()
    {
        if (!isWarning)
        {
            isWarning = true;

            if (!isVipParking)
            {
                display.DOColor(new Color(1, 0.6f, 0.6f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine)
                                .OnKill(() => display.color = Color.white);
            }
            else
            {
                vipDisplay.DOColor(new Color(1, 0.6f, 0.6f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine)
                                .OnKill(() => vipDisplay.color = Color.white);
            }
        }
    }

    public void StopWarning()
    {
        isWarning = false;
        display.DOKill();
        vipDisplay.DOKill();
    }
}
