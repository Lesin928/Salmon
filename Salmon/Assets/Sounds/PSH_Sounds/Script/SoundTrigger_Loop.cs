using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class SoundTrigger_Loop : MonoBehaviour
{
    public GameObject player;
    public AudioManager audioManager;
    public string audioClipName;

    private bool hasPlayed = false;

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
            audioManager.PlayLoop(audioClipName);
            hasPlayed = true;
            Debug.Log($"[auidoClipName] ���");

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player") && !string.IsNullOrEmpty(audioClipName))
        {

            // ���尡 AudioManager�� �����ϴ��� Ȯ��
            audioManager.StopSFX(audioClipName);
            hasPlayed = false;
            Debug.Log($"[auidoClipName] ��� ����");


        }

    }
}
