using DarkTonic.PoolBoss;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Car : MonoBehaviour
{
    public static event EventHandler OnMoveToParkingSpace;
    public static event EventHandler OnParking;
    public static event EventHandler OnMoveAway;

    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private PolygonCollider2D subCollider;
    [SerializeField] private PolygonCollider2D touchArea;
    [SerializeField] private PolygonCollider2D staticCollider;
    [SerializeField] private SpriteRenderer carDisplay;
    [SerializeField] private SpriteRenderer arrow;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private Animator carAnimator;
    [SerializeField] private ParticleSystem smokeEffect;

    public Transform entrance;

    [SerializeField] private CarSO carSO;

    [Header("Prop")]
    [SerializeField] private float speed;
    [SerializeField] private ParkingSpace parkingSpace;
    [SerializeField] private CarData carData;
    [SerializeField] private CarSizeConfig carSizeConfig;
    [HideInInspector] public bool isLastCar;
    [HideInInspector] public bool isTutCar;

    public List<Guest> guests = new();

    private Vector2 vectorDir;
    private bool isMovingToWall;
    private bool isBlocking;
    private bool isMoving;
    private bool canClick;
    private List<PointDirect> pathNodes = new();
    private Vector3 initPos;
    private ColliderConfig colliderConfig;
    private Transform blockObj;

    private const string HORIZONTAL_PARAM_NAME = "Horizontal";
    private const string VERTICAL_PARAM_NAME = "Vertical";
    private const string PHASE2_PARAM_NAME = "Phase2";

    public ObjectColor Color => carData.eColor;
    public Vector2 VectorDir => vectorDir;
    public CarSize Size => carData.size;
    public int Slot => carSizeConfig.slotPositions.Count;
    public bool IsMoving => isMoving;

    public void Init(CarData carData, bool isSetInitPos = true)
    {
        if (isSetInitPos)
        {
            initPos = transform.position;
        }

        isMovingToWall = false;
        isBlocking = false;
        isMoving = false;
        isLastCar = false;
        isTutCar = false;
        canClick = true;

        smokeEffect.Stop();

        guests.Clear();
        parkingSpace = null;

        touchArea.enabled = true;
        polygonCollider.enabled = true;
        subCollider.enabled = false;
        staticCollider.enabled = true;

        ActiveSortingGroup(false);

        this.carData = carData;
        carSizeConfig = carSO.GetCarSlot(carData.size);
        colliderConfig = carSO.GetColliderConfig(carData.direction, carData.size);

        carAnimator.runtimeAnimatorController = carSO.GetCarConfig(carData.eColor, carData.size).carAnimation;

        ChangeDirection(carData.direction, true);

        Invoke(nameof(SetTouchCollider), 0.1f);
    }

    private void SetTouchCollider()
    {
        //touchArea.points = colliderConfig.touchCollider.ToArray();
        polygonCollider.points = colliderConfig.touchCollider.ToArray();
        staticCollider.points = colliderConfig.touchCollider.ToArray();

        int numPath = carDisplay.sprite.GetPhysicsShapeCount();
        touchArea.pathCount = numPath;

        float ratio = /*carAnimator.transform.localScale.x*/0.7f;

        for (int i = 0; i < numPath; i++)
        {
            List<Vector2> physicsShape = new List<Vector2>();

            int pointCount = carDisplay.sprite.GetPhysicsShape(i, physicsShape);

            Vector2[] fixedPoint = physicsShape.Select(p => p * ratio).ToArray();

            if (pointCount > 0)
            {
                touchArea.SetPath(i, fixedPoint);
            }
        }
    }

    private void SetMoveCollider()
    {
        subCollider.points = colliderConfig.colliderPoints.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBlocking && collision.transform == blockObj)
        {
            isMovingToWall = false;

            ResetToInitPos();

            SoundManager.Instance.PlaySound(KeySound.Car_Hit);

            if (collision.TryGetComponent(out Car car))
            {
                car.transform.DOPunchPosition(vectorDir * 0.2f, 0.15f, 1).SetDelay(0f).OnComplete(() =>
                {
                    car.transform.position = car.initPos;
                    car.canClick = true;
                });
            }

        }
        else if (collision.TryGetComponent(out Boundary boundary))
        {
            if (!isMovingToWall) return;

            isMovingToWall = false;

            List<Transform> points = boundary.NodePathBoundary;

            pathNodes.Clear();
            List<Vector3> listPos = new List<Vector3>();

            foreach (Transform point in points)
            {
                listPos.Add(new Vector3(point.position.x, point.position.y, transform.position.z));

                if (point.TryGetComponent(out PointDirect pointDirection))
                {
                    pathNodes.Add(pointDirection);
                }
            }

            transform.DOPath(listPos.ToArray(), speed).SetSpeedBased().SetEase(Ease.Linear)
                    .OnWaypointChange(ChangeDirection).OnComplete(EnterParkingLot);
        }
    }

    private void ChangeDirection(int index)
    {
        if (index >= pathNodes.Count) return;

        ChangeDirection(pathNodes[index].direction);
    }

    private void ChangeDirection(Direction direction, bool isShowArrow = false)
    {
        carData.direction = direction;
        SetVectorDir(direction);
        SetMoveCollider();

        if (isShowArrow)
        {
            arrow.gameObject.SetActive(true);
            arrow.sprite = carSO.GetCarDirection(direction).arrowSprite;
            arrow.transform.localPosition = carSO.GetOffsetArrow(direction, Size);
        }
    }

    public void OnClick()
    {
        if (!isTutCar && TutorialManager.Instance.isTut) return;
        if (!canClick) return;

        ParkingSpace emptySpace = ParkingLotManager.Instance.GetEmptyParkingSpace();
        if (emptySpace == null) return;

        touchArea.enabled = false;
        polygonCollider.enabled = false;
        staticCollider.transform.parent = null;

        arrow.gameObject.SetActive(false);

        SetVectorDir(carData.direction);

        Transform closestBlockObj = null;
        float closestDistance = Mathf.Infinity;

        var points = polygonCollider.GetFrontPointsCollider(carData.direction);

        points.Add((points[0] + points[1]) / 2f);

        foreach (Vector2 point in points)
        {
            var pos = transform.TransformPoint(point);

            RaycastHit2D hit = Physics2D.Raycast(pos, vectorDir, Mathf.Infinity, GameplayManager.Instance.carLayer);
#if UNITY_EDITOR
            Debug.DrawRay(pos, vectorDir * 100f, UnityEngine.Color.red, 4f);
#endif
            if (hit.collider != null)
            {              
                float distance = Vector2.Distance(pos, hit.point);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlockObj = hit.collider.transform;
                }
            }

            hit = Physics2D.Raycast(pos, vectorDir, Mathf.Infinity, GameplayManager.Instance.obstacleLayer);

            if (hit.collider != null)
            {
                float distance = Vector2.Distance(pos, hit.point);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlockObj = hit.collider.transform;
                }
            }
        }

        if (closestBlockObj != null)
        {
            blockObj = closestBlockObj;

            if (blockObj.TryGetComponent(out Car car))
            {
                car.canClick = false;
            }

            subCollider.enabled = false;
            polygonCollider.enabled = true;
            isBlocking = true;
            isMovingToWall = true;

            polygonCollider.enabled = true;

            return;
        }

        TakeParkingSpace(emptySpace);

        staticCollider.transform.SetParent(transform);
        staticCollider.enabled = false;

        subCollider.enabled = true;
        polygonCollider.enabled = false;
        isBlocking = false;

        OnMoveToParkingSpace?.Invoke(this, EventArgs.Empty);
    }

    public bool CanMove()
    {
        polygonCollider.enabled = false;

        SetVectorDir(carData.direction);

        var points = polygonCollider.GetFrontPointsCollider(carData.direction);

        points.Add((points[0] + points[1]) / 2f);

        foreach (Vector2 point in points)
        {
            var pos = transform.TransformPoint(point);

            RaycastHit2D hit = Physics2D.Raycast(pos, vectorDir, Mathf.Infinity, GameplayManager.Instance.carLayer);

            Debug.DrawRay(pos, vectorDir * 100f, UnityEngine.Color.red, 4f);

            if (hit.collider != null)
            {
                polygonCollider.enabled = true;

                return false;
            }
        }

        polygonCollider.enabled = true;

        return true;
    }

    private void SetVectorDir(Direction direction)
    {
        float angleRelativeToY = 51.6f;

        float v = Mathf.Tan(Mathf.Deg2Rad * angleRelativeToY);

        switch (direction)
        {
            case Direction.Top:
                vectorDir = Vector2.up;
                break;
            case Direction.Down:
                vectorDir = Vector2.down;
                break;
            case Direction.Left:
                vectorDir = Vector2.left;
                break;
            case Direction.Right:
                vectorDir = Vector2.right;
                break;
            case Direction.TopLeft:
                vectorDir = new Vector2(-v, 1);
                break;
            case Direction.DownLeft:
                vectorDir = new Vector2(-v, -1);
                break;
            case Direction.TopRight:
                vectorDir = new Vector2(v, 1);
                break;
            case Direction.DownRight:
                vectorDir = new Vector2(v, -1);
                break;
        }

        //carAnimator.SetFloat(HORIZONTAL_PARAM_NAME, Mathf.Ceil(vectorDir.x));
        carAnimator.SetFloat(HORIZONTAL_PARAM_NAME, vectorDir.x);
        carAnimator.SetFloat(VERTICAL_PARAM_NAME, vectorDir.y);

        transform.SetScaleX(vectorDir.x > 0 ? -transform.localScale.y : transform.localScale.y);
    }

    private void FixedUpdate()
    {
        if (!isMovingToWall) return;

        Vector3 distance = speed * Time.fixedDeltaTime * vectorDir.normalized;

        transform.position += distance;
    }

    private void ResetToInitPos()
    {
        isBlocking = false;

        transform.DOMove(initPos, speed).SetDelay(0.05f).SetEase(Ease.Linear).SetSpeedBased().OnComplete(() =>
        {
            arrow.gameObject.SetActive(true);

            touchArea.enabled = true;
            polygonCollider.enabled = true;
            subCollider.enabled = false;
            staticCollider.transform.SetParent(this.transform);
        });
    }

    public void TakeParkingSpace(ParkingSpace parkingSpace)
    {
        if (parkingSpace == null)
        {
            ResetToInitPos();
            return;
        }

        this.parkingSpace = parkingSpace;
        parkingSpace.ChangeSpaceStatus(ParkingSpaceStatus.Busy);

        isMovingToWall = true;
        isMoving = true;

        smokeEffect.Play();

        SoundManager.Instance.PlaySound(KeySound.Car_Run, false, 0.5f);
    }

    public void EnterParkingLot()
    {
        subCollider.enabled = false;

        if (parkingSpace.entrance.position.x < transform.position.x)
        {
            ChangeDirection(Direction.Left);
        }
        else if (parkingSpace.entrance.position.x > transform.position.x)
        {
            ChangeDirection(Direction.Right);
        }

        Vector3 endPos = new Vector3(parkingSpace.entrance.position.x, parkingSpace.entrance.position.y, transform.position.z);

        transform.SetPosY(endPos.y);

        transform.DOMove(endPos, speed).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                ChangeDirection(Direction.TopLeft);

                transform.localEulerAngles = new Vector3(0, 0, -27);

                Vector3 endPos1 = new Vector3(parkingSpace.transform.position.x, parkingSpace.transform.position.y, transform.position.z);

                transform.DOMove(endPos1, speed).SetSpeedBased().SetEase(Ease.Linear)
                .OnComplete(ReadyToSit);
            });
    }

    private void ReadyToSit()
    {
        isMoving = false;

        transform.localEulerAngles = Vector3.zero;

        arrow.gameObject.SetActive(false);

        //carDisplay.sprite = carSizeConfig.carSprite;
        carAnimator.SetBool(PHASE2_PARAM_NAME, true);

        ParkingLotManager.Instance.carsInParking.Add(this);

        OnParking?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetFreeSlotPos()
    {
        if (guests.Count >= carSizeConfig.slotPositions.Count) return Vector3.zero;

        return carSizeConfig.slotPositions[guests.Count];
    }

    public bool IsFull()
    {
        return guests.Count == carSizeConfig.slotPositions.Count;
    }

    public void AddGuest(Guest guest)
    {
        guests.Add(guest);

        if (IsFull())
        {
            StartCoroutine(IeMoveAway());
        }
    }

    private IEnumerator IeMoveAway()
    {
        while (true)
        {
            bool canBreak = true;

            foreach (var guest in guests)
            {
                if (!guest.IsInCar)
                {
                    canBreak = false;
                    break;
                }
            }

            if (canBreak)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }

        yield return new WaitForEndOfFrame();

        MoveAway();
    }

    private void MoveAway()
    {
        transform.DOKill();

        parkingSpace.ChangeSpaceStatus(ParkingSpaceStatus.Empty);

        OnMoveAway?.Invoke(this, EventArgs.Empty);

        ChangeDirection(Direction.DownRight);

        transform.localEulerAngles = new Vector3(0, 0, -27);

        Vector3 endPos = new Vector3(parkingSpace.entrance.position.x, parkingSpace.entrance.position.y, transform.position.z);

        if (smokeEffect.isStopped) smokeEffect.Play();

        transform.DOMove(endPos, speed).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                parkingSpace = null;

                ChangeDirection(Direction.Right);

                transform.localEulerAngles = Vector3.zero;

                Vector3 endPos1 = new Vector3(ParkingLotManager.Instance.parkingEntrance.position.x,
                    ParkingLotManager.Instance.parkingEntrance.position.y, transform.position.z);

                transform.DOMove(endPos1, speed).SetSpeedBased().SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (isLastCar)
                    {
#if use_winlose
                        GameplayManager.Instance.ShowWin();
#endif
                    }

                    Destroy();
                });
            });

        DespawnGuestInCar();
    }

    public void Destroy()
    {
        transform.DOKill();
        PoolBoss.Despawn(transform);
    }

    public void DespawnGuestInCar()
    {
        foreach (Guest guest in guests)
        {
            guest.Destroy();
        }

        guests.Clear();
    }

    public void MoveOutOfGarage(Vector3 endPos)
    {
        Debug.Log(name + " move out of garage");

        canClick = false;

        Vector3 scale = vectorDir.x > 0 ? new Vector3(-1, 1, 1) : Vector3.one;

        transform.DOScale(scale, 0.3f).SetDelay(0.2f).SetEase(Ease.Linear);
        transform.DOMove(endPos, speed).SetDelay(0.2f).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                canClick = true;
                initPos = transform.position;
            });
    }

    public void ActiveSortingGroup(bool isActive)
    {
        sortingGroup.enabled = isActive;
    }

    public void MoveToVipParking(Action callback)
    {
        arrow.gameObject.SetActive(false);

        polygonCollider.enabled = false;
        touchArea.enabled = false;
        subCollider.enabled = false;

        parkingSpace = ParkingLotManager.Instance.vipParkingSpace;
        parkingSpace.ChangeSpaceStatus(ParkingSpaceStatus.Busy);

        EffectManager.Instance.ShowCarDisappearEffect(transform.position);

        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            transform.localEulerAngles = new Vector3(0, 0, -27);

            ChangeDirection(Direction.TopLeft);

            transform.localPosition = new Vector3(parkingSpace.transform.position.x, parkingSpace.transform.position.y, transform.position.z);

            EffectManager.Instance.ShowCarAppearEffect(parkingSpace.transform.position);

            parkingSpace.PlayIdleEffect(false);

            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                callback?.Invoke();
                ReadyToSit();
            });
        });
    }

    public void MoveToNormalParking(Action callback)
    {
        arrow.gameObject.SetActive(false);

        polygonCollider.enabled = false;
        touchArea.enabled = false;
        subCollider.enabled = false;

        parkingSpace = ParkingLotManager.Instance.GetEmptyParkingSpace();
        parkingSpace.ChangeSpaceStatus(ParkingSpaceStatus.Busy);

        Vector3 scale = transform.localScale.x > 0 ? Vector3.one * 1.3f : new Vector3(-1.3f, 1.3f, 1.3f);

        transform.DOScale(scale, 0.3f).OnComplete(() =>
        {
            transform.localEulerAngles = new Vector3(0, 0, -27);

            ChangeDirection(Direction.TopLeft);

            Vector3 endPos = new Vector3(parkingSpace.transform.position.x, parkingSpace.transform.position.y, transform.position.z);

            transform.DOMove(endPos, speed * 1.5f).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                scale = transform.localScale.x > 0 ? Vector3.one : new Vector3(-1f, 1f, 1f);

                transform.DOScale(scale, 0.3f).OnComplete(() =>
                {
                    callback?.Invoke();
                    ReadyToSit();
                });
            });
        });
    }

    public void ChangeColor(ObjectColor color)
    {
        carData.eColor = color;

        carAnimator.runtimeAnimatorController = carSO.GetCarConfig(color, carData.size).carAnimation;
    }
}
