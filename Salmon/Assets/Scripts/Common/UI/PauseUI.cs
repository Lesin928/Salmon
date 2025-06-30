using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : UIBase
{
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
        GameManager.Instance.PauseGame(false); // 게임 시간 재개
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
        GameManager.Instance.PauseGame(false); // 게임 시간 재개
        UIManager.Instance.CloseAllOpenUI(); // 모든 UI 닫기
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            SceneLoader.Instance.LoadScene(SceneType.Lobby); // 로비 씬으로 이동
        });
    }

    public void OnClickQuitButton()
    {
        GameManager.Instance.SavePlayData(); // 플레이 데이터 저장
        GameManager.Instance.PauseGame(false); // 게임 시간 재개
        UIManager.Instance.CloseAllOpenUI(); // 모든 UI 닫기
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            GameManager.Instance.QuitGame();
        });
    }
}
