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

    public void OnClickContinueBtn()
    {
        // 저장된 게임을 로드
    }

    public void OnClickNewGameBtn()
    {
        // 새로운 게임 시작
        //AudioManager.Instance.PlaySFX(SFX.ui_button_click);
        //AudioManager.Instance.StopBGM();
    }

    public void OnClickSettingsBtn()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickAchievementsBtn()
    {
        // 도전과제 목록 보여주기
    }

    public void OnClickCreditsBtn()
    {
        // 크레딧 화면 열기
    }

    public void OnClickQuitBtn()
    {
        GameManager.Instance.Quit();
    }
}
