using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// One seated group member in Scene3. Owns their topic, spoken intro line and therapy video.
//
// Note: this deliberately does NOT reuse Naledi.cs for speaking. Naledi.Awake() looks up
// hardcoded blendshape names ("Facial_Blends.*") that only exist on the Thandi mesh; on a
// Mixamo character GetBlendShapeIndex returns -1 and SetBlendShapeWeight(-1, ...) throws.
[RequireComponent(typeof(AudioSource))]
public class TherapyCharacter : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string topicLabel = "Depression";

    [Header("Intro line")]
    [SerializeField] private AudioClip introClip;
    [SerializeField, TextArea(2, 5)] private string introText;

    [Header("Therapy video")]
    [SerializeField] private VideoClip video;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterBoard board;
    [SerializeField] private Transform confirmPanelAnchor;  // optional: where the Yes/No panel appears

    [Header("Animation")]
    [SerializeField] private string talkingBoolName = "IsTalking";

    // Fallback duration when no intro clip has been recorded yet.
    [SerializeField] private float silentIntroDuration = 3f;

    public string TopicLabel        => topicLabel;
    public string IntroText         => introText;
    public VideoClip Video          => video;
    public CharacterBoard Board     => board;
    public Transform ConfirmAnchor  => confirmPanelAnchor;

    private Coroutine _introRoutine;

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (animator == null)    animator    = GetComponentInChildren<Animator>();
        if (board == null)       board       = GetComponentInChildren<CharacterBoard>();

        audioSource.playOnAwake = false;
    }

    // Plays the one-line intro, then invokes onComplete.
    public void PlayIntro(Action onComplete)
    {
        if (_introRoutine != null) StopCoroutine(_introRoutine);
        _introRoutine = StartCoroutine(IntroRoutine(onComplete));
    }

    public void StopIntro()
    {
        if (_introRoutine != null)
        {
            StopCoroutine(_introRoutine);
            _introRoutine = null;
        }

        audioSource.Stop();
        SetTalking(false);
    }

    private IEnumerator IntroRoutine(Action onComplete)
    {
        SetTalking(true);

        if (introClip != null)
        {
            audioSource.clip = introClip;
            audioSource.Play();
            // Wait on isPlaying rather than clip.length so a manual Stop() ends the line cleanly.
            while (audioSource.isPlaying)
                yield return null;
        }
        else
        {
            // No voiceover recorded yet — hold long enough for the subtitle to be read.
            yield return new WaitForSeconds(silentIntroDuration);
        }

        SetTalking(false);
        _introRoutine = null;
        onComplete?.Invoke();
    }

    private void SetTalking(bool talking)
    {
        if (animator != null && !string.IsNullOrEmpty(talkingBoolName))
            animator.SetBool(talkingBoolName, talking);
    }
}
