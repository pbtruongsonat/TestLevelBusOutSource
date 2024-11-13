using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public static event EventHandler OnReadyToEnterCar;

    [SerializeField] private SpriteRenderer guestDisplay;
    [SerializeField] private Transform emojiWait;
    [SerializeField] private Transform emojiSit;
    [SerializeField] private Animator guestAnimator;
    [SerializeField] private GuestSO guestSO;

    [Header("Prop")]
    [SerializeField] private float speed;
    [SerializeField] private bool isInCar;
    [SerializeField] private ObjectColor color;

    private bool isMovingInLine;
    private bool isMovingToCar;

    private readonly Vector3 SIT_SCALE = Vector3.one * 1.12f;
    private readonly Vector3 STAND_SCALE = Vector3.one * 0.8f;
    private const float DELAY_TO_CAR = 0.02f;
    private const string HORIZONTAL_PARAM_NAME = "Horizontal";
    private const string VERTICAL_PARAM_NAME = "Vertical";
    private const string SIT_PARAM_NAME = "Sit";
    private const string MOVE_PARAM_NAME = "Move";

    public bool IsInCar => isInCar;
    public bool IsMovingInLine => isMovingInLine;
    public bool IsMovingToCar => isMovingToCar;
    public ObjectColor Color => color;

    public void Init(ObjectColor color)
    {
        guestAnimator.transform.localScale = STAND_SCALE;
        transform.localScale = Vector3.one;

        isInCar = false;
        isMovingInLine = true;
        isMovingToCar = false;

        ChangeColor(color);
    }

    public void MoveTo(Vector3[] path)
    {
        guestAnimator.SetBool(MOVE_PARAM_NAME, true);

        transform.DOPath(path.ToArray(), speed).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMovingInLine = false;

                guestAnimator.SetBool(MOVE_PARAM_NAME, false);

                if (GuestManager.Instance.IsEdgeGuest(this))
                {
                    guestAnimator.SetFloat(HORIZONTAL_PARAM_NAME, -1);
                    guestAnimator.SetFloat(VERTICAL_PARAM_NAME, 0);
                }
            })
            .OnWaypointChange((index) =>
            {
                if (index > path.Length - 1)
                {

                    return;
                }

                SetDirection(path[index]);
            });
    }

    private void SetDirection(Vector3 endPos)
    {
        Vector3 vectorDir = endPos - transform.position;

        guestAnimator.SetFloat(HORIZONTAL_PARAM_NAME, vectorDir.x);
        guestAnimator.SetFloat(VERTICAL_PARAM_NAME, vectorDir.y);

        guestDisplay.flipX = vectorDir.x > 0.01f;
    }

    public void MoveTo(Vector3 endPos)
    {
        isMovingInLine = true;
        guestAnimator.SetBool(MOVE_PARAM_NAME, true);

        transform.DOMove(endPos, speed * 1.8f).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMovingInLine = false;
                guestAnimator.SetBool(MOVE_PARAM_NAME, false);

                if (GuestManager.Instance.IsEdgeGuest(this))
                {
                    guestAnimator.SetFloat(HORIZONTAL_PARAM_NAME, -1);
                    guestAnimator.SetFloat(VERTICAL_PARAM_NAME, 0);
                }

                DOVirtual.DelayedCall(DELAY_TO_CAR, () => OnReadyToEnterCar?.Invoke(this, EventArgs.Empty));
            });

        SetDirection(endPos);
    }

    public void MoveToCar(Car car)
    {
        SoundManager.Instance.PlaySound(KeySound.Guest_Enter);

        Vector3 inCarPos = car.GetFreeSlotPos();

        isMovingToCar = true;
        guestAnimator.SetBool(MOVE_PARAM_NAME, true);

        Vector3 endPos = new Vector2(car.entrance.position.x, car.entrance.position.y);

        SetDirection(endPos);

        transform.DOMove(endPos, speed * 2f).SetEase(Ease.Linear).SetSpeedBased()
            .OnComplete(() =>
            {
                guestAnimator.SetBool(SIT_PARAM_NAME, true);

                isInCar = true;
                isMovingToCar = false;

                transform.SetParent(car.transform);
                transform.localPosition = inCarPos;

                //guestDisplay.sprite = info.sitSprite;
                guestAnimator.transform.localScale = SIT_SCALE;
          
                car.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1).OnComplete(() =>
                {
                    car.transform.localScale = Vector3.one;
                });
            });

        car.AddGuest(this);
    }

    public void ShowEmoji()
    {
        if (isMovingInLine || isMovingToCar) return;

        if (!isInCar)
        {
            EffectManager.Instance.ShowEmoji(emojiWait.position, true);
        }
        else
        {
            EffectManager.Instance.ShowEmoji(emojiWait.position, false);
        }
    }

    public void Destroy()
    {
        transform.DOKill();
        GuestManager.Instance.RemoveGuest(this);
        PoolBoss.Despawn(transform);
    }

    public void ChangeColor(ObjectColor color)
    {
        this.color = color;
        guestAnimator.runtimeAnimatorController = guestSO.GetGuestAnimation(color);
    }
}
