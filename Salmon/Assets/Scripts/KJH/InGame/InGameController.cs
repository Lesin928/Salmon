using TMPro;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private bool isGameCleared = false;

    private void Start()
    {
        UpdateTimer();

        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    private void Update()
    {
        if (isGameCleared)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        GameManager.Instance.PlayTime += Time.deltaTime;
        GameManager.Instance.TotalPlayTime += Time.deltaTime;
        UpdateTimer();

        int hours = Mathf.FloorToInt(GameManager.Instance.TotalPlayTime / 3600f);
        if(hours > 10)
        {
            AchievementManager.Instance.SetAchievementProgress(AchievementKey.PLAY_FOR_TEN_HOURS.ToString(), 1);
        }
    }

    private void UpdateTimer()
    {
        int hours = Mathf.FloorToInt(GameManager.Instance.PlayTime / 3600f);
        int minutes = Mathf.FloorToInt(GameManager.Instance.PlayTime / 60f);
        int seconds = Mathf.FloorToInt(GameManager.Instance.PlayTime % 60f);
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    private void TogglePause()
    {
        bool pause = !GameManager.Instance.IsPaused;

        if(pause)
        {
            var uiData = new UIBaseData();
            UIManager.Instance.OpenUI<PauseUI>(uiData);
        }
        else
        {
            UIManager.Instance.CloseAllOpenUI();
        }

        GameManager.Instance.PauseGame(pause);
    }

    public void SetGameCleared()
    {
        isGameCleared = true;
        GameManager.Instance.SavePlayData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if (!GameManager.Instance.IsPaused)
            {
                var uiData = new UIBaseData();
                UIManager.Instance.OpenUI<PauseUI>(uiData);

                GameManager.Instance.PauseGame(true);
            }
        }
    }
}
