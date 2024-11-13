using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class SetCanvasScaler : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    void Start()
    {
        float ratio = (float)(Screen.width / (float)Screen.height);

        if (ratio >= 0.6f)
        {
            canvasScaler.matchWidthOrHeight = 1;

        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
    }

}
