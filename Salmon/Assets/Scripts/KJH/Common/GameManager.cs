using Singleton.Component;
using UnityEngine;

public class GameManager : SingletonComponent<GameManager>
{
    public bool IsPaused { get; private set; }

    public float PlayTime { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public float NewRecord { get; set; }
    public float TotalPlayTime { get; set; }

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
        IsPaused = false;
        return true;
    }

    protected override void ReleaseInstance()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (Instance != this)
            Destroy(gameObject);
    }
    #endregion

    public void LoadPlayData()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null)
        {
            PlayTime = userPlayData.PlayTime;
            PlayerPosition = userPlayData.PlayerPosition;
            NewRecord = userPlayData.NewRecord;
            TotalPlayTime = userPlayData.TotalPlayTime;
        }
    }

    public void SavePlayData()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null)
        {
            userPlayData.ExistsSavedPlayData = true;
            userPlayData.PlayTime = PlayTime;
            userPlayData.PlayerPosition = PlayerPosition;
            userPlayData.NewRecord = NewRecord;
            userPlayData.TotalPlayTime = TotalPlayTime;
            userPlayData.SaveData();
        }
    }

    public void ResetPlayData()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null)
        {
            userPlayData.SoftResetData();
            userPlayData.SaveData();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame(bool _pause)
    {
        IsPaused = _pause;

        if (IsPaused)
        {
            Time.timeScale = 0f; // 게임 시간 정지
        }
        else
        {
            Time.timeScale = 1f; // 게임 시간 재개
        }
    }
}
