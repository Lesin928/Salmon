using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private GameObject m_ContinueBtn;

    private void Start()
    {
        var userPlayData = UserDataManager.Instance.GetUserData<UserPlayData>();
        if (userPlayData != null && userPlayData.ExistsSavedPlayData)
        {
            m_ContinueBtn.SetActive(true);
        }
        else
        {
            m_ContinueBtn.SetActive(false);
        }
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

    public void OnClickQuitBtn()
    {
        GameManager.Instance.Quit();
    }
}
