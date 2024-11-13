using UnityEngine;

public class MatchWidthCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private const float DEFAULT_RATIO = 0.5625f; // 16:9
    private const float DEFAULT_CAMSIZE = 14f;

    private void Start()
    {
        float ratio = (float)(Screen.width / (float)Screen.height);

        if (ratio < DEFAULT_RATIO)
        {
            cam.orthographicSize = DEFAULT_CAMSIZE * (DEFAULT_RATIO / ratio);
        }
    }
}
