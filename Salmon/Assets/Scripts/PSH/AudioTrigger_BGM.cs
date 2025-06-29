using UnityEngine;

public class AudioTrigger_BGM : MonoBehaviour
{
    public AudioManager audioManager;

    public string audioClipName;

    public void Start()
    {
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;

        }
        audioManager.PlayMusic(audioClipName);
    }
}
