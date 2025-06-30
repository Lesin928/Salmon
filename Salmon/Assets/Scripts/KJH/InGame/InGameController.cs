using TMPro;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool isGameCompleted;

    private void Start()
    {
        isGameCompleted = false;

        UpdateTimer();
        player.transform.position = GameManager.Instance.PlayerPosition;
        player.transform.rotation = Quaternion.Euler(GameManager.Instance.PlayerRotation);

        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    private void Update()
    {
        if (isGameCompleted)
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

        GameManager.Instance.PlayerPosition = player.transform.position;
        GameManager.Instance.PlayerRotation = player.transform.rotation.eulerAngles;

        int hours = Mathf.FloorToInt(GameManager.Instance.TotalPlayTime / 3600f);
        if(hours >= 10)
        {
            AchievementManager.Instance.SetAchievementProgress(AchievementKey.PLAY_FOR_TEN_HOURS.ToString(), 1);
        }
    }

    private void UpdateTimer()
    {
        int hours = Mathf.FloorToInt(GameManager.Instance.PlayTime / 3600f);
        int minutes = Mathf.FloorToInt(GameManager.Instance.PlayTime / 60f % 60f);
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

    public void CompleteGoal()
    {
        if (!isGameCompleted)
        {
            isGameCompleted = true;

            if (GameManager.Instance.PlayTime < GameManager.Instance.NewRecord)
            {
                GameManager.Instance.NewRecord = GameManager.Instance.PlayTime;
            }

            AchievementManager.Instance.SetAchievementProgress(AchievementKey.REACH_THE_TOP.ToString(), 1);
            AchievementManager.Instance.AddAchievementProgress(AchievementKey.REACH_THE_TOP_FIVE_TIMES.ToString(), 1);

            int minutes = Mathf.FloorToInt(GameManager.Instance.PlayTime / 60f);
            int seconds = Mathf.FloorToInt(GameManager.Instance.PlayTime % 60f);
            if (minutes < 5 || (minutes == 5 && seconds == 0))
            {
                AchievementManager.Instance.SetAchievementProgress(AchievementKey.SPEEDRUNNER.ToString(), 1);
            }

            GameManager.Instance.SavePlayData();

            var uiData = new UIBaseData();
            UIManager.Instance.OpenUI<CompleteUI>(uiData);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if (!GameManager.Instance.IsPaused && !isGameCompleted)
            {
                var uiData = new UIBaseData();
                UIManager.Instance.OpenUI<PauseUI>(uiData);

                GameManager.Instance.PauseGame(true);
            }
        }
    }
}
