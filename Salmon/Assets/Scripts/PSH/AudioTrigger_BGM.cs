using UnityEngine;

public class AudioTrigger_BGM : MonoBehaviour
{
    public string audioClipName;

    public void Start()
    {
        AudioManager.Instance.PlayMusic(audioClipName);
    }
}
