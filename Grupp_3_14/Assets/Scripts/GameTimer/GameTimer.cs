using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Timer Settings")]
    public float startTime = 300f;
    private float currentTime;
    private bool timerRunning = false; 

    [Header("UI (optional)")]
    public TextMeshProUGUI timerText;

    [Header("End Scene")]
    public string endSceneName = "EndScene";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            TimerEnded();
        }

        UpdateTimerUI();
    }

    
    public void StartTimer()
    {
        currentTime = startTime;
        timerRunning = true;
        UpdateTimerUI();
    }

    void TimerEnded()
    {
        timerRunning = false;
        SceneManager.LoadScene(endSceneName);
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
