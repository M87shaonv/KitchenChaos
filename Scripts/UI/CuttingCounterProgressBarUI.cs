using UnityEngine;
using UnityEngine.UI;

public class CuttingCounterProgressBarUI : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image progressBar;

    private void Start()
    {
        cuttingCounter.OnCutProgressChanged += HandleCutProgressChanged;
        HandleCutProgressChanged(0f); // 初始进度为0
    }

    private void HandleCutProgressChanged(float progress)
    {
        progressBar.fillAmount = progress;
        if (Mathf.Abs(progress) <= 0.01f || Mathf.Abs(progress - 1f) <= 0.01f)
            Hide();
        else
            Show();
    }

    private void OnDestroy()
    {
        cuttingCounter.OnCutProgressChanged -= HandleCutProgressChanged;
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}