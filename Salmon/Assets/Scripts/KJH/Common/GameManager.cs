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
        // 유저 데이터 로드 주석
        //유저 데이터 로드
        // 유저 데이터 매니저를 통한 데이터 로드
        UserDataManager.Instance.LoadUserData();

        // 저장된 유저 데이터가 없으면 기본데이터 생성 후 저장 주석
        //저장된 유저 데이터가 없으면 기본데이터 생성 후 저장
        // 저장된 데이터가 존재하지 않는 경우
        if (!UserDataManager.Instance.ExistsSavedData)
        {
            // 기본 유저 데이터 설정
            UserDataManager.Instance.SetDefaultUserData();
            // 유저 데이터 저장
            UserDataManager.Instance.SaveUserData();
        }

        // 오디오 매니저에 유저 데이터 로드 알림
        AudioManager.Instance.OnLoadUserData();

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


}
