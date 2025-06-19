using UnityEngine;

public class PauseUI : UIBase
{
    public override void OnClickCloseButton()
    {
        base.OnClickCloseButton();
    }

    public void OnClickSettingsButton()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickAchievementsButton()
    {

    }

    public void OnClickReturnToLobbyButton()
    {

    }

    public void OnClickQuitButton()
    {
        GameManager.Instance.Quit();
    }
}
