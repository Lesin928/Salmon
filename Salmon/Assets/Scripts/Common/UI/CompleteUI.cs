using System.Collections;
using TMPro;
using UnityEngine;

public class CompleteUI : UIBase
{
    [SerializeField] private TextMeshProUGUI timerText;

    public override void SetInfo(UIBaseData uiData)
    {
        base.SetInfo(uiData);

        int hours = Mathf.FloorToInt(GameManager.Instance.PlayTime / 3600f);
        int minutes = Mathf.FloorToInt(GameManager.Instance.PlayTime / 60f % 60f);
        int seconds = Mathf.FloorToInt(GameManager.Instance.PlayTime % 60f);
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

        StartCoroutine(ReturnToLobby());
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(3f);

        UIManager.Instance.CloseAllOpenUI();
        GameManager.Instance.ResetPlayData();
        GameManager.Instance.LoadPlayData();
        SceneLoader.Instance.LoadScene(SceneType.Lobby);
    }
}
