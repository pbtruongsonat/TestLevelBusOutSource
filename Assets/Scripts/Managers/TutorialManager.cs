using Obvious.Soap;
using Spine.Unity;
using UnityEngine;

public class TutorialManager : SingletonBase<TutorialManager>
{
    [SerializeField] private SkeletonAnimation handTut;

    [SerializeField] private IntVariable currentLevel;
    [SerializeField] private ScriptableEventNoParam OnCheckTutorial;

    [HideInInspector] public bool isTut;

    private int colorIndex;

    private readonly Vector3 HAND_OFFSET = new Vector3(1.444f, -1.122f, 0);

    private void Start()
    {
        //OnCheckTutorial.OnRaised += OnCheckTutorial_OnRaised;
        //Car.OnMoveToParkingSpace += Car_OnMoveToParkingSpace;
    }

    private void Car_OnMoveToParkingSpace(object sender, System.EventArgs e)
    {
        if (!isTut) return;

        handTut.gameObject.SetActive(false);

        Invoke(nameof(OnCheckTutorial_OnRaised), 0.35f);
    }

    private void OnCheckTutorial_OnRaised()
    {
        handTut.gameObject.SetActive(false);

        if (currentLevel.Value > 1) return;

        if (isTut)
        {
            colorIndex++;
        }
        else
        {
            isTut = true;
            colorIndex = 0;
        }

        Car car = ParkingLotManager.Instance.GetTutCar(colorIndex);

        if (car == null)
        {
            isTut = false;

            //SonatTracking.LogCompleteTutGameplay(currentLevel.Value);

            return;
        }

        car.isTutCar = true;

        handTut.transform.position = car.transform.position + HAND_OFFSET;
        handTut.Initialize(true);
        handTut.gameObject.SetActive(true);
    }
}
