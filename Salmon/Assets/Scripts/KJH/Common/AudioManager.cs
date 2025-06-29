using Singleton.Component;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static AudioManager;
using static Unity.VisualScripting.Member;

public class AudioManager : SingletonComponent<AudioManager>
{
    public enum SFX { UI, Player, Bear, Voice }
    public enum Music { BGM }

    [System.Serializable]
    public class AudioClipData
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public Transform MusicTrs;
    public Transform SFXTrs;

    [SerializeField, Header("BGM Clips")]
    private List<AudioClipData> bgmClips;

    [SerializeField, Header("UI Clips")]
    private List<AudioClipData> uiClips;

    [SerializeField, Header("Player Clips")]
    private List<AudioClipData> playerClips;

    [SerializeField, Header("Bear Clips")]
    private List<AudioClipData> bearClips;

    [SerializeField, Header("Voice Clips")]
    private List<AudioClipData> voiceClips;

    private Dictionary<Music, AudioSource> m_MusicPlayer = new Dictionary<Music, AudioSource>();
    private AudioSource m_CurrBGMSource;

    private Dictionary<SFX, AudioSource> m_SFXPlayer = new Dictionary<SFX, AudioSource>();


    #region Singleton
    protected override void AwakeInstance()
    {
        Initialize();
    }

    protected override bool InitInstance()
    {
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

    public void PlayMusic(string musicName)
    {

        var clipData = bgmClips.Find(c => c.name == musicName);

        if (clipData == null)
        {
            Debug.LogError($"Music clip '{musicName}' not found.");
            return;
        }

        if (m_CurrBGMSource != null && m_CurrBGMSource.clip != null && m_CurrBGMSource.clip.name == musicName)
        {
            Debug.Log($"[AudioManager] Music is already playing: {musicName}");
            return;
        }

        if (m_CurrBGMSource != null)
        {
            m_CurrBGMSource.Stop();
            Destroy(m_CurrBGMSource.gameObject);
        }

        var newGO = new GameObject($"Music {musicName}");
        AudioSource newAudioSource = newGO.AddComponent<AudioSource>();
        newAudioSource.clip = clipData.clip;
        newAudioSource.volume = clipData.volume;
        newAudioSource.loop = true;
        newAudioSource.playOnAwake = false;
        newGO.transform.parent = MusicTrs;


        var userSettings = UserDataManager.Instance.GetUserData<UserSettingsData>();

        float userVolume = userSettings != null ? userSettings.Music_Volume : 1f;
        newAudioSource.volume = CalculateFinalVolume(clipData.volume, userVolume);

        m_CurrBGMSource = newAudioSource;
        m_CurrBGMSource.Play();
    }

    public void PlaySFX(string sfxName)
    {

        var clipData = playerClips.Find(c => c.name == sfxName) 
            ?? bearClips.Find(c => c.name == sfxName) 
            ?? voiceClips.Find(c => c.name == sfxName)
            ?? uiClips.Find(c => c.name == sfxName);

        if (clipData == null)
        {
            Debug.LogError($"Music clip '{sfxName}' not found.");
            return;
        }

        var newGO = new GameObject(sfxName);
        AudioSource newAudioSource = newGO.AddComponent<AudioSource>();
        newAudioSource.clip = clipData.clip;
        newAudioSource.volume = clipData.volume;
        newAudioSource.loop = false;
        newAudioSource.playOnAwake = false;
        newGO.transform.parent = SFXTrs;

        newAudioSource.Play();
        GameObject.Destroy(newGO, clipData.clip.length);
    }

    public void OnLoadUserData()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            SetVolume(userSettingsData);
        }
    }

 

    public void PauseMusic()
    {
        if (m_CurrBGMSource) m_CurrBGMSource.Pause();
    }

    public void ResumeMusic()
    {
        if (m_CurrBGMSource) m_CurrBGMSource.UnPause();
    }

    public void StopMusic()
    {
        if (m_CurrBGMSource) m_CurrBGMSource.Stop();
    }


    public void RandomPlaySFX(string name)
    {
        List<AudioClipData> targetList = null;

        // 문자열로 리스트 이름 비교
        switch (name)
        {
            case "bgmClips":
                targetList = bgmClips;
                break;
            case "uiClips":
                targetList = uiClips;
                break;
            case "playerClips":
                targetList = playerClips;
                break;
            case "bearClips":
                targetList = bearClips;
                break;
            case "voiceClips":
                targetList = voiceClips;
                break;
            default:
                Debug.LogWarning($"[AudioManager] '{name}'이라는 리스트를 찾을 수 없습니다.");
                return;
        }

        if (targetList == null || targetList.Count == 0)
        {
            Debug.LogWarning($"[AudioManager] '{name}' 리스트가 비어 있습니다.");
            return;
        }

        int randomIndex = Random.Range(0, targetList.Count);
        AudioClip clip = targetList[randomIndex].clip;

        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] 선택된 클립이 null입니다.");
            return;
        }

        if(targetList == bgmClips )
        {
            PlayMusic(clip.name);  // BGM 또는 UI 클립 재생
            Debug.Log($"[AudioManager] {name}에서 Music 랜덤 재생: {clip.name}");
            return;
        }
        else if (targetList == playerClips || targetList == bearClips || targetList == voiceClips || targetList == uiClips)
        {
            PlaySFX(clip.name);  // SFX 클립 재생
            Debug.Log($"[AudioManager] {name}에서 SFX 랜덤 재생: {clip.name}");
            return; 
        }
    }



    public void SetVolume(UserSettingsData userSettingsData)
    {
        foreach (var pair in m_MusicPlayer)
        {
            AudioSource newAudioSource = pair.Value;

            // 개별 clipData에서 볼륨 정보를 가져올 수 있어야 함
            string name = newAudioSource.clip.name;

            var clipData = bgmClips.Find(c => c.clip.name == name);

            float clipVolume = clipData != null ? clipData.volume : 1f;
            newAudioSource.volume = CalculateFinalVolume(clipVolume, userSettingsData.Music_Volume);
        }

        foreach (var pair in m_SFXPlayer)
        {
            AudioSource newAudioSource = pair.Value;

            string name = newAudioSource.clip.name;

            var clipData = playerClips.Find(c => c.clip.name == name)
                        ?? bearClips.Find(c => c.clip.name == name)
                        ?? voiceClips.Find(c => c.clip.name == name)
                        ?? uiClips.Find(c => c.clip.name == name);


            float clipVolume = clipData != null ? clipData.volume : 1f;
            newAudioSource.volume = CalculateFinalVolume(clipVolume, userSettingsData.SFX_Volume);
        }
    }

    private float CalculateFinalVolume(float clipVolume, float userVolume)
    {
        return Mathf.Clamp01(clipVolume * userVolume);
    }
}
