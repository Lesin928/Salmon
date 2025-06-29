using Singleton.Component;
using UnityEngine;

public class GameManager : SingletonComponent<GameManager>
{
    public float PlayTime { get; set; }
    public Vector3 PlayerPosition { get; set; }
    public float NewRecord { get; set; }

    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
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
        }
    }

    public void SavePlayData()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null)
        {
            userPlayData.PlayTime = PlayTime;
            userPlayData.PlayerPosition = PlayerPosition;
            userPlayData.NewRecord = NewRecord;
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

    public void Quit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<PauseUI>(uiData);
    }
}
