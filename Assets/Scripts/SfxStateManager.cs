using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxStateManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] walkingSfx;
    [SerializeField] private AudioClip[] sprintingSfx;
    [SerializeField] private AudioClip[] jumpingSfx;
    [SerializeField] private AudioClip[] keyboardSfx;
    [SerializeField] private AudioSource walkingSfxSource;

    private void OnEnable()
    {
        FirstPersonController.PlayWalkingSfx += Walking;
        FirstPersonController.PlaySprintingSfx += Sprinting;
        FirstPersonController.PlayJumpingSfx += Jumping;
        Interaction.PlayKeyboardSfx += SlammingKeyboard;
            DenaryPuzzle.StopKeyboardSfx += StopKeyboardSfx;
    }

    private void OnDisable()
    {
        FirstPersonController.PlayWalkingSfx -= Walking;
        FirstPersonController.PlaySprintingSfx -= Sprinting;
        FirstPersonController.PlayJumpingSfx -= Jumping;
        Interaction.PlayKeyboardSfx -= SlammingKeyboard;
        DenaryPuzzle.StopKeyboardSfx -= StopKeyboardSfx;
    }

    private void Walking()
    {
        if (walkingSfxSource.isPlaying) return;
        AudioClip sfx = walkingSfx[UnityEngine.Random.Range(0, walkingSfx.Length)];
        walkingSfxSource.PlayOneShot(sfx);
    }
    
    private void Sprinting()
    {
        if (walkingSfxSource.isPlaying) return;
        AudioClip sfx = sprintingSfx[UnityEngine.Random.Range(0, sprintingSfx.Length)];
        walkingSfxSource.PlayOneShot(sfx);
    }
    
    private void Jumping()
    {
        walkingSfxSource.Stop();
        AudioClip sfx = jumpingSfx[UnityEngine.Random.Range(0, jumpingSfx.Length)];
        walkingSfxSource.PlayOneShot(sfx);
    }

    private void SlammingKeyboard()
    {
        walkingSfxSource.Stop();
        AudioClip sfx = keyboardSfx[UnityEngine.Random.Range(0, keyboardSfx.Length)];
        // Play on repeaat until the puzzle is completed
        walkingSfxSource.clip = sfx;
        walkingSfxSource.loop = true;
        walkingSfxSource.Play();
    }
    
    private void StopKeyboardSfx()
    {
        walkingSfxSource.Stop();
        walkingSfxSource.loop = false;
    }
}
