using Singleton.Component;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    COUNT
}

public enum SFX
{
    COUNT
}

public class AudioManager : SingletonComponent<AudioManager>
{
    public Transform MusicTrs;
    public Transform SFXTrs;

    private const string AUDIO_PATH = "Sounds";

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

    private void LoadBGMPlayer()
    {
        for (int i = 0; i < (int)Music.COUNT; i++)
        {
            var audioName = ((Music)i).ToString();
            var pathStr = $"{AUDIO_PATH}/{audioName}";
            var audioClip = Resources.Load(pathStr, typeof(AudioClip)) as AudioClip;
            if (!audioClip)
            {
                Debug.LogError($"{audioName} clip does not exist.");
                continue;
            }

            var newGO = new GameObject(audioName);
            var newAudioSource = newGO.AddComponent<AudioSource>();
            newAudioSource.clip = audioClip;
            newAudioSource.loop = true;
            newAudioSource.playOnAwake = false;
            newGO.transform.parent = MusicTrs;

            m_MusicPlayer[(Music)i] = newAudioSource;
        }
    }

    private void LoadSFXPlayer()
    {
        for (int i = 0; i < (int)SFX.COUNT; i++)
        {
            var audioName = ((SFX)i).ToString();
            var pathStr = $"{AUDIO_PATH}/{audioName}";
            var audioClip = Resources.Load(pathStr, typeof(AudioClip)) as AudioClip;
            if (!audioClip)
            {
                Debug.LogError($"{audioName} clip does not exist.");
                continue;
            }

            var newGO = new GameObject(audioName);
            var newAudioSource = newGO.AddComponent<AudioSource>();
            newAudioSource.clip = audioClip;
            newAudioSource.loop = false;
            newAudioSource.playOnAwake = false;
            newGO.transform.parent = SFXTrs;

            m_SFXPlayer[(SFX)i] = newAudioSource;
        }
    }

    public void OnLoadUserData()
    {
        var userSettingsData = UserDataManager.Instance.GetUserData<UserSettingsData>();
        if (userSettingsData != null)
        {
            SetVolume(userSettingsData);
        }
    }

    public void PlayMusic(Music music)
    {
        if (m_CurrBGMSource)
        {
            m_CurrBGMSource.Stop();
            m_CurrBGMSource = null;
        }

        if (!m_MusicPlayer.ContainsKey(music))
        {
            Debug.LogError($"Invalid clip name. {music}");
            return;
        }

        m_CurrBGMSource = m_MusicPlayer[music];
        m_CurrBGMSource.Play();
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

    public void PlaySFX(SFX sfx)
    {
        if (!m_SFXPlayer.ContainsKey(sfx))
        {
            Debug.LogError($"Invalid clip name. ({sfx})");
            return;
        }

        m_SFXPlayer[sfx].Play();
    }

    public void SetVolume(UserSettingsData userSettingsData)
    {
        foreach (var audioSourceItem in m_MusicPlayer)
        {
            audioSourceItem.Value.volume = userSettingsData.Music_Volume;
        }

        foreach (var audioSourceItem in m_SFXPlayer)
        {
            audioSourceItem.Value.volume = userSettingsData.SFX_Volume;
        }
    }
}
