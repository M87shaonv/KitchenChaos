using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        HideCountdownText();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged()
    {
        if (GameManager.Instance.IsCountdownToStart())
            ShowCountdownText();
        else
            HideCountdownText();
    }

    private void Update()
    {
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void ShowCountdownText() => countdownText.gameObject.SetActive(true);
    private void HideCountdownText() => countdownText.gameObject.SetActive(false);
}