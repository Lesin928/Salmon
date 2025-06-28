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

        UIManager.Instance.Fade(Color.black, 1f, 0f, 0.5f, 0f, true);
    }

    public void OnClickContinueButton()
    {
        // 저장된 게임을 로드
    }

    public void OnClickNewGameButton()
    {
        // 새로운 게임 시작
        //AudioManager.Instance.PlaySFX(SFX.ui_button_click);
        //AudioManager.Instance.StopBGM();
    }

    public void OnClickSettingsButton()
    {
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<SettingsUI>(uiData);
    }

    public void OnClickAchievementsButton()
    {
        // 도전과제 목록 보여주기
        var uiData = new UIBaseData();
        UIManager.Instance.OpenUI<AchievementsUI>(uiData);
    }

    public void OnClickCreditsButton()
    {
        // 크레딧 화면 열기
    }

    public void OnClickQuitButton()
    {
        GameManager.Instance.Quit();
    }
}
