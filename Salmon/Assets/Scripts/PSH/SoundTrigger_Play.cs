using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class SoundTrigger_Play : MonoBehaviour
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !string.IsNullOrEmpty(audioClipName))
        {

            // ���尡 AudioManager�� �����ϴ��� Ȯ��
            audioManager.PlayMusic(audioClipName);
            Debug.Log($"[auidoClipName] ���");

        }
    }

}
