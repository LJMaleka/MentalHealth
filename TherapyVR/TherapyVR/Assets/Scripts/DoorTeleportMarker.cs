using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

// Attach to the TeleportationAnchor marker in front of a door.
// Ray hover  → highlights door mesh + plays chime.
// Trigger press → fade to black → load target scene.
[RequireComponent(typeof(TeleportationAnchor))]
public class DoorTeleportMarker : MonoBehaviour
{
    [Header("Door highlight")]
    [SerializeField] private Renderer doorRenderer;
    [SerializeField] private Material doorHighlightMat;
    [SerializeField] private Material doorDefaultMat;

    [Header("Audio")]
    [SerializeField] private AudioSource hoverToneSource;
    [SerializeField] private AudioClip hoverTone;

    [Header("Scene transition")]
    [SerializeField] private string targetSceneName = "Scene2";
    [SerializeField] private float fadeDuration = 1.5f;

    private TeleportationAnchor _anchor;
    private bool _transitioning;

    void Awake()
    {
        _anchor = GetComponent<TeleportationAnchor>();
    }

    void OnEnable()
    {
        _anchor.hoverEntered.AddListener(OnHoverEnter);
        _anchor.hoverExited.AddListener(OnHoverExit);
        _anchor.selectEntered.AddListener(OnSelected);
    }

    void OnDisable()
    {
        _anchor.hoverEntered.RemoveListener(OnHoverEnter);
        _anchor.hoverExited.RemoveListener(OnHoverExit);
        _anchor.selectEntered.RemoveListener(OnSelected);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (doorRenderer != null && doorHighlightMat != null)
            doorRenderer.material = doorHighlightMat;

        if (hoverToneSource != null && hoverTone != null && !hoverToneSource.isPlaying)
            hoverToneSource.PlayOneShot(hoverTone);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (doorRenderer != null && doorDefaultMat != null)
            doorRenderer.material = doorDefaultMat;

        if (hoverToneSource != null)
            hoverToneSource.Stop();
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (_transitioning) return;
        _transitioning = true;
        StartCoroutine(HandleTransition());
    }

    private IEnumerator HandleTransition()
    {
        if (ScreenFader.instance != null)
            ScreenFader.instance.ScreenFadeOut(fadeDuration);

        yield return new WaitForSeconds(fadeDuration + 0.1f);

        ReceptionManager.Instance?.SetState(ReceptionManager.State.Departed);
        SceneManager.LoadScene(targetSceneName);
    }
}
