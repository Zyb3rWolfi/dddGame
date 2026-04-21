using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioSource outsideAudioSource;
    [SerializeField] private AudioClip ambientAudioClip;
    [SerializeField] private AudioClip NoticedAudioSource;
    [SerializeField] private AudioClip ChasingAudioSource;
    [SerializeField] private float soundCooldown = 10f;
    [SerializeField] private float fadeDuration = 2f;
    private bool isNoticedAudioPlaying = false;
    private float lastSoundTime = -999f;

    private void Start()
    {
        playAmbientAudio();
    }

    private void OnEnable()
    {
        AIController.HAndleStateAudioChange += HandleStateAudioChange;
    }

    private void OnDisable()
    {
        AIController.HAndleStateAudioChange -= HandleStateAudioChange;
    }

    private void HandleStateAudioChange(AIController.AIState newState)
    {
        StopAllCoroutines();
        switch (newState)
        {
            case AIController.AIState.Chasing:
                StartCoroutine(CrossfadeMusic(ChasingAudioSource, false)); // Loop chase music
                break;
            case AIController.AIState.Investigating:
                StartCoroutine(CrossfadeMusic(NoticedAudioSource, false)); // Play sting
                break;
        }
    }
    
    private IEnumerator CrossfadeMusic(AudioClip newClip, bool shouldLoop)
    {
        // 1. Fade Out
        while (ambientAudioSource.volume > 0.0f)
        {
            ambientAudioSource.volume -= Time.deltaTime * fadeDuration;
            yield return null;
        }

        // 2. Switch Clip
        ambientAudioSource.clip = newClip;
        ambientAudioSource.loop = shouldLoop;
        ambientAudioSource.Play();

        // 3. Fade In
        while (ambientAudioSource.volume < 0.2f)
        {
            ambientAudioSource.volume += Time.deltaTime * fadeDuration;
            yield return null;
        }
    }
    
    private void playAmbientAudio()
    {
        outsideAudioSource.clip = ambientAudioClip;
        ambientAudioSource.loop = true;
        outsideAudioSource.Play();
    }
    
}
