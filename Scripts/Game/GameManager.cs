using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public event Action OnGameStateChanged;
    private static GameManager instance;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text orderText;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button returnToMenuBtn;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private Button returnMenuBtn;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    public enum GameState
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState currentState;
    private float waitingToStartTimer = 1f;
    private float countDownTimer = 3f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        returnMenuBtn.onClick.AddListener(() => {
            Loader.Load(Loader.Scenes.MainMenu);
            Time.timeScale = 1f;
        });
    }

    private void Start()
    {
        GameInputMovement.OnPauseAction += PauseGame;
        currentState = GameState.WaitingToStart; //TODO 这里改成WaitingToStart
        gameOverPanel.SetActive(false);
        gameOverPanel.transform.Find("ContinueGame").GetComponent<Button>().onClick.AddListener(() => Loader.Load(Loader.Scenes.Game)
        );
        gameOverPanel.transform.Find("ReturnMenu").GetComponent<Button>().onClick.AddListener(() => Loader.Load(Loader.Scenes.MainMenu)
        );
    }

    private void OnDestroy()
    {
        GameInputMovement.OnPauseAction -= PauseGame;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer <= 0)
                {
                    currentState = GameState.CountDownToStart;
                    OnGameStateChanged?.Invoke();
                }

                break;
            case GameState.CountDownToStart:
                countDownTimer -= Time.deltaTime;
                if (countDownTimer <= 0)
                {
                    currentState = GameState.GamePlaying;
                    OnGameStateChanged?.Invoke();
                }

                break;
            case GameState.GamePlaying:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public float GetCountdownToStartTimer() => countDownTimer;
    public bool IsGamePlaying() => currentState == GameState.GamePlaying;
    public bool IsCountdownToStart() => currentState == GameState.CountDownToStart;

    public void GameOver(int orderCount)
    {
        AudioManager.Instance.StopAllSounds();
        currentState = GameState.GameOver;
        OnGameStateChanged?.Invoke();
        StartCoroutine(ShowGameOverCoroutine());
        orderText.text = "完成订单数: " + orderCount.ToString();
    }

    private IEnumerator ShowGameOverCoroutine()
    {
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.localScale = Vector3.zero;
        float duration = 1.0f; // 动画持续时间
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            gameOverPanel.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        gameOverPanel.transform.localScale = Vector3.one; // 确保最终缩放为1
    }

    private void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}