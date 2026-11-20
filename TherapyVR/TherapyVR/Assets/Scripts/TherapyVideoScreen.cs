using System;
using UnityEngine;
using UnityEngine.Video;

// Wraps the VideoPlayer driving the wall screen in Scene3.
// Hidden until Play() is called; raises OnFinished when the clip ends or Skip() is pressed.
[RequireComponent(typeof(VideoPlayer))]
public class TherapyVideoScreen : MonoBehaviour
{
    [Header("Playback")]
    [SerializeField] private VideoPlayer player;
    [SerializeField] private Renderer screenRenderer;   // the quad; disabled while idle
    [SerializeField] private AudioSource videoAudioSource;
    [SerializeField] private GameObject skipButton;    // hidden unless a clip is playing

    [Header("Ambient ducking")]
    [SerializeField] private AudioSource ambientSource;                    // optional room ambience
    [SerializeField, Range(0f, 1f)] private float duckedAmbientVolume = 0.15f;

    // Raised when the clip finishes on its own or Skip() is called.
    public event Action OnFinished;

    public bool IsPlaying => _playing;

    private float _ambientDefaultVolume;
    private bool _playing;

    void Awake()
    {
        if (player == null) player = GetComponent<VideoPlayer>();

        // Route audio through an AudioSource so it is spatialised with the rest of the room.
        if (videoAudioSource != null)
        {
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.SetTargetAudioSource(0, videoAudioSource);
        }

        player.playOnAwake = false;
        player.isLooping = false;

        if (ambientSource != null) _ambientDefaultVolume = ambientSource.volume;
        if (screenRenderer != null) screenRenderer.enabled = false;
        if (skipButton != null) skipButton.SetActive(false);
    }

    void OnEnable()  => player.loopPointReached += HandleLoopPoint;
    void OnDisable() => player.loopPointReached -= HandleLoopPoint;

    // ── Public API ────────────────────────────────────────────────────────────

    public void Play(VideoClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{name}: Play called with no clip — finishing immediately.", this);
            OnFinished?.Invoke();
            return;
        }

        _playing = true;
        player.clip = clip;

        if (screenRenderer != null) screenRenderer.enabled = true;
        if (skipButton != null) skipButton.SetActive(true);
        if (ambientSource != null) ambientSource.volume = duckedAmbientVolume;

        player.Play();
    }

    // Cuts playback short — bound to a Skip button so a demo can be moved along.
    public void Skip()
    {
        if (!_playing) return;
        Stop();
        OnFinished?.Invoke();
    }

    public void Stop()
    {
        _playing = false;
        player.Stop();

        if (screenRenderer != null) screenRenderer.enabled = false;
        if (skipButton != null) skipButton.SetActive(false);
        if (ambientSource != null) ambientSource.volume = _ambientDefaultVolume;
    }

    // ── Internals ─────────────────────────────────────────────────────────────

    private void HandleLoopPoint(VideoPlayer source)
    {
        if (!_playing) return;
        Stop();
        OnFinished?.Invoke();
    }
}
