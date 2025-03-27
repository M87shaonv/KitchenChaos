using System;
using UnityEngine;
using UnityEngine.UI;

public class StoveCounterProgressUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Image progressBar1;
    [SerializeField] private Image progressBar2;

    private void Start()
    {
        stoveCounter.OnCookProgressChanged += HandleCookProgressChanged;
        HandleCookProgressChanged(0f, 0f);
    }

    private void HandleCookProgressChanged(float progress1, float progress2)
    {
        progressBar1.fillAmount = progress1;
        progressBar2.fillAmount = progress2;
        // 只有当两个进度条都接近完成（100%）或接近停止（0%）时才隐藏
        if ((Mathf.Abs(progress1 - 1) <= 0.01f && Mathf.Abs(progress2 - 1) <= 0.01f) ||
            (Mathf.Abs(progress1) <= 0.01f && Mathf.Abs(progress2) <= 0.01f))
            Hide();
        else
            Show();
    }

    private void OnDestroy()
    {
        stoveCounter.OnCookProgressChanged -= HandleCookProgressChanged;
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}