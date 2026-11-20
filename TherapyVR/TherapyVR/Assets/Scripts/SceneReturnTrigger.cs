using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Attach to any collider in Scene2 that should return the player to Scene1.
// Ray hover  → highlights door mesh + plays chime.
// Trigger press → fade to black → load target scene.
[RequireComponent(typeof(XRSimpleInteractable))]
public class SceneReturnTrigger : MonoBehaviour
{
    [Header("Scene transition")]
    [SerializeField] private string targetScene = "Scene1";
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Door highlight")]
    [SerializeField] private Renderer doorRenderer;
    [SerializeField] private Material doorHighlightMat;
    [SerializeField] private Material doorDefaultMat;

    [Header("Audio")]
    [SerializeField] private AudioSource hoverToneSource;
    [SerializeField] private AudioClip hoverTone;

    private XRSimpleInteractable _interactable;
    private bool _triggered;

    void Awake() => _interactable = GetComponent<XRSimpleInteractable>();

    void OnEnable()
    {
        _interactable.hoverEntered.AddListener(OnHoverEnter);
        _interactable.hoverExited.AddListener(OnHoverExit);
        _interactable.selectEntered.AddListener(OnSelected);
    }

    void OnDisable()
    {
        _interactable.hoverEntered.RemoveListener(OnHoverEnter);
        _interactable.hoverExited.RemoveListener(OnHoverExit);
        _interactable.selectEntered.RemoveListener(OnSelected);
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
        if (_triggered) return;
        _triggered = true;
        StartCoroutine(ReturnToScene());
    }

    private IEnumerator ReturnToScene()
    {
        ScreenFader.IsReturning = true;
        if (ScreenFader.instance != null)
            ScreenFader.instance.ScreenFadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration + 0.1f);
        SceneManager.LoadScene(targetScene);
    }
}
