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

    //<summary> 접촉시 해당 SFX재생 
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !string.IsNullOrEmpty(audioListName))
        {
            audioManager.RandomPlaySFX(audioListName);
            Debug.Log($"[auidoClipName] 재생");

        }
    }

}
