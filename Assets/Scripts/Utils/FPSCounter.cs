using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FPSCounter : MonoBehaviour
{
    private TMP_Text text;
    private float elapsed;
    private float avg;
    private int updateCount;

    [SerializeField] private float updateGap = 1f;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        elapsed = 0f;
        avg = 0f;
        updateCount = 0;
    }

    private void Update()
    {
        if (elapsed > updateGap)
        {
            elapsed = 0f;
            updateCount++;
            avg = (avg * (updateCount - 1) + 1 / Time.deltaTime) / updateCount;
            text.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime) + "\nAVG: " + Math.Round(avg, 2);
        }

        elapsed += Time.deltaTime;
    }
}