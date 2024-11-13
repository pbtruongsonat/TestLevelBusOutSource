using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class UILoading : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private float currentValue;

    private const int INGAME_SCENE = 1;

    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
        //TimeManager.Instance.StartGetRealDateTime();
        Loading(INGAME_SCENE);
    }

    private void Loading(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        operation.allowSceneActivation = false;

        float timeWait = 0f;

        while (!operation.isDone && currentValue < 1 )/*|| !Kernel.Resolve<BasePurchaser>().IsInitialized()*/
            //|| !Kernel.Resolve<DownloadLevelManager>().Initialized/* || !TimeManager.Instance.isTimeLoaded*/)
        {
            if (currentValue >= 1 /*&& TimeManager.Instance.isTimeLoaded*/)
            {
                if (Helper.IsPurchaserInitFailed())
                {
                    while (timeWait < 1.1f)
                    {
                        timeWait += Time.deltaTime;
                        yield return null;
                    }

                    Debug.Log("Purchaser init: " + Helper.IsPurchaserInitFailed());
                    break;
                }
            }

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            currentValue = Mathf.MoveTowards(currentValue, progress, 0.7f * Time.deltaTime);

            slider.value = currentValue;

            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        //GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => Destroy(gameObject));

        operation.allowSceneActivation = true;
    }
}
