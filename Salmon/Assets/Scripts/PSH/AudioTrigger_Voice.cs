using UnityEngine;
using System.Collections.Generic;
using static AudioManager;

public class AudioTrigger_Voice : MonoBehaviour
{
    public GameObject player;
    public AudioManager audioManager;

    public string audioListName;


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
        if (collision.CompareTag("Player"))
        {
            audioManager.RandomPlaySFX(audioListName);
            Debug.Log($"[auidoClipName] 재생");

        }
    }

}
