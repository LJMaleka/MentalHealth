using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// The labelled board each group member holds in Scene3.
// Ray hover  → highlights the board + plays a chime.
// Trigger press → asks GroupTherapyManager to run that character's intro.
[RequireComponent(typeof(XRSimpleInteractable))]
public class CharacterBoard : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private TherapyCharacter owner;

    [Header("Board face")]
    [SerializeField] private TMP_Text titleText;      // topic written on the board
    [SerializeField] private Renderer boardRenderer;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Material defaultMat;

    [Header("Audio")]
    [SerializeField] private AudioSource hoverToneSource;
    [SerializeField] private AudioClip hoverTone;

    private XRSimpleInteractable _interactable;

    void Awake()
    {
        _interactable = GetComponent<XRSimpleInteractable>();
        if (owner == null) owner = GetComponentInParent<TherapyCharacter>();
    }

    void Start()
    {
        // Keep the printed label in sync with the character's topic.
        if (titleText != null && owner != null)
            titleText.text = owner.TopicLabel;
    }

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

    // Locked by the manager while an intro or video is running.
    public void SetInteractable(bool value)
    {
        _interactable.enabled = value;
        if (!value) ClearHighlight();
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (boardRenderer != null && highlightMat != null)
            boardRenderer.material = highlightMat;

        if (hoverToneSource != null && hoverTone != null && !hoverToneSource.isPlaying)
            hoverToneSource.PlayOneShot(hoverTone);
    }

    private void OnHoverExit(HoverExitEventArgs args) => ClearHighlight();

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (owner == null)
        {
            Debug.LogWarning($"{name}: board has no TherapyCharacter owner assigned.", this);
            return;
        }

        GroupTherapyManager.Instance?.OnBoardClicked(owner);
    }

    private void ClearHighlight()
    {
        if (boardRenderer != null && defaultMat != null)
            boardRenderer.material = defaultMat;

        if (hoverToneSource != null)
            hoverToneSource.Stop();
    }
}
