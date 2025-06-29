using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : UIBase
{
    public override void Init(Transform anchor)
    {
        base.Init(anchor);
        Time.timeScale = 0f; // 게임 시간 정지
    }

    public override void OnClickCloseButton()
    {
        Time.timeScale = 1f; // 게임 시간 재개
        base.OnClickCloseButton();
    }

    public void OnClickSettingsButton()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickAchievementsButton()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<AchievementsUI>(uiData);
    }

    public void OnClickReturnToLobbyButton()
    {
        GameManager.Instance.SavePlayData(); // 플레이 데이터 저장
        SceneLoader.Instance.LoadScene(SceneType.Lobby); // 로비 씬으로 이동
        OnClickCloseButton(); // UI 닫기
    }

    public void OnClickQuitButton()
    {
        GameManager.Instance.SavePlayData(); // 플레이 데이터 저장
        GameManager.Instance.Quit();
    }
}
