using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public void Init()
    {
        
    }

    public void OnClickNewGameBtn()
    {
        //AudioManager.Instance.PlaySFX(SFX.ui_button_click);
        //AudioManager.Instance.StopBGM();
    }

    public void OnClickSettingsBtn()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }
}
