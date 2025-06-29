using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static AudioManager;

public class AudioTrigger_Voice : MonoBehaviour
{
    public GameObject player;
    public AudioManager audioManager;

    public string audioListName;

    [Header("쿨타임 설정")]
    public float cooldownTime = 10f;  // 쿨타임 (초)
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f; // 남은 쿨타임 추적


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


    private void Update()
    {
        // 남은 쿨타임 갱신
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
        }
    }

    //<summary> 접촉시 해당 SFX재생 
    private void OnTriggerEnter(Collider collision)
    {
        if (isOnCooldown)
        {
            Debug.Log($"[AudioTrigger_Voice] 쿨타임 중입니다. 재사용까지 남은 시간: {cooldownTimer:F2}초");
            return;
        }

        if (collision.CompareTag("Player") && !string.IsNullOrEmpty(audioListName))
        {
            audioManager.RandomPlaySFX(audioListName);
            Debug.Log($"[auidoClipName] 재생");
            StartCoroutine(CooldownCoroutine());

                    cooldownTimer = cooldownTime;
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

}
