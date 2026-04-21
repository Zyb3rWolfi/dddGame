using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] walkingSfx;
    [SerializeField] private AudioSource walkingSfxSource;

    private void OnEnable()
    {
        AIController.playSfx += Walking;
    }

    private void OnDisable()
    {
        AIController.playSfx -= Walking;
    }

    private void Walking()
    {
        if (walkingSfxSource.isPlaying) return;
        AudioClip sfx = walkingSfx[UnityEngine.Random.Range(0, walkingSfx.Length)];
        walkingSfxSource.PlayOneShot(sfx);
    }
    
}
