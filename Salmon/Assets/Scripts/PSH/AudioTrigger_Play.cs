using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class AudioTrigger_Play : MonoBehaviour
{
    public GameObject player;
    public AudioManager audioManager;

    public string audioClipName;

    public void Start()
    {
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;

        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    //<summary> 접촉시 오디오 재생 </summary>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !string.IsNullOrEmpty(audioClipName))
        {

            // 사운드가 AudioManager에 존재하는지 확인
            audioManager.PlayMusic(audioClipName);
            Debug.Log($"[auidoClipName] 재생");

        }
    }

}
