using Singleton.Component;
using UnityEngine;

public class GameManager : SingletonComponent<GameManager>
{
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
