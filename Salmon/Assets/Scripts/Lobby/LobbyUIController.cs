using TMPro;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_RecordText;
    [SerializeField] private GameObject m_ContinueBtn;

    private void Start()
    {
        if (GameManager.Instance.NewRecord != Mathf.Infinity)
        {
            int hours = Mathf.FloorToInt(GameManager.Instance.NewRecord / 3600f);
            int minutes = Mathf.FloorToInt(GameManager.Instance.NewRecord / 60f % 60f);
            int seconds = Mathf.FloorToInt(GameManager.Instance.NewRecord % 60f);
            m_RecordText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
        else
        {
            m_RecordText.gameObject.SetActive(false);
        }
        
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
        GameManager.Instance.LoadPlayData(); // 플레이 데이터 로드
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            SceneLoader.Instance.LoadScene(SceneType.MainScene);
        });
    }

    public void OnClickNewGameButton()
    {
        // 새로운 게임 시작
        GameManager.Instance.ResetPlayData(); // 플레이 데이터 초기화
        GameManager.Instance.LoadPlayData(); // 플레이 데이터 로드
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            SceneLoader.Instance.LoadScene(SceneType.MainScene);
        });
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

    public void OnClickQuitButton()
    {
        UIManager.Instance.Fade(Color.black, 0f, 1f, 0.5f, 0f, false, () =>
        {
            GameManager.Instance.QuitGame();
        });
    }
}
