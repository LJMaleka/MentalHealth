using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Scene3 (Group Therapy Room) state machine.
//
// Flow: player teleports into the empty chair → doctor welcomes them → the six boards
// unlock → player clicks a board → that character speaks their intro line → Yes/No prompt
// → Yes plays the topic video on the wall screen → back to browsing.
//
// The room is informational only; no assessment data is read and no results are written.
public class GroupTherapyManager : MonoBehaviour
{
    public static GroupTherapyManager Instance { get; private set; }

    public enum State { Idle, DoctorWelcome, Browsing, CharacterIntro, AwaitingConfirm, VideoPlaying }

    [Header("Cast")]
    [SerializeField] private TherapyCharacter doctor;          // welcome line lives on its introClip
    [SerializeField] private TherapyCharacter[] characters;    // the six seated members

    [Header("Video")]
    [SerializeField] private TherapyVideoScreen videoScreen;

    [Header("Confirm prompt")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TMP_Text yesButtonText;
    [SerializeField] private TMP_Text noButtonText;

    [Header("Subtitles")]
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Behaviour")]
    [SerializeField] private bool faceConfirmPanelAtPlayer = true;
    [SerializeField] private Transform playerHead;             // XR Origin main camera

    public State CurrentState { get; private set; } = State.Idle;

    private TherapyCharacter _activeCharacter;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (playerHead == null && Camera.main != null)
            playerHead = Camera.main.transform;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void OnEnable()
    {
        if (videoScreen != null) videoScreen.OnFinished += OnVideoFinished;
    }

    void OnDisable()
    {
        if (videoScreen != null) videoScreen.OnFinished -= OnVideoFinished;
    }

    void Start()
    {
        if (yesButton != null) yesButton.onClick.AddListener(OnYesPressed);
        if (noButton != null) noButton.onClick.AddListener(OnNoPressed);

        if (yesButtonText != null) yesButtonText.text = "Yes";
        if (noButtonText != null) noButtonText.text = "Not right now";

        HideConfirmPanel();
        HideSubtitle();
        SetBoardsInteractable(false);   // locked until the doctor has spoken
    }

    // ── Entry point ───────────────────────────────────────────────────────────

    // Wired to SeatTeleportTrigger.onSeated on the empty chair.
    public void OnPlayerSeated()
    {
        if (CurrentState != State.Idle) return;

        if (doctor == null)
        {
            // No facilitator wired — go straight to browsing so the room still works.
            EnterBrowsing();
            return;
        }

        CurrentState = State.DoctorWelcome;
        ShowSubtitle(doctor.IntroText);
        doctor.PlayIntro(() =>
        {
            HideSubtitle();
            EnterBrowsing();
        });
    }

    // ── Board interaction ─────────────────────────────────────────────────────

    public void OnBoardClicked(TherapyCharacter character)
    {
        // Ignore clicks during the welcome, an intro, the prompt or a video.
        if (CurrentState != State.Browsing || character == null) return;

        _activeCharacter = character;
        CurrentState = State.CharacterIntro;

        SetBoardsInteractable(false);
        ShowSubtitle(character.IntroText);

        character.PlayIntro(() =>
        {
            HideSubtitle();
            ShowConfirmPanel(character);
        });
    }

    public void OnYesPressed()
    {
        if (CurrentState != State.AwaitingConfirm) return;

        HideConfirmPanel();

        if (videoScreen == null || _activeCharacter == null || _activeCharacter.Video == null)
        {
            Debug.LogWarning($"{name}: no video assigned for " +
                             $"{_activeCharacter?.TopicLabel ?? "unknown topic"} — returning to browsing.", this);
            EnterBrowsing();
            return;
        }

        CurrentState = State.VideoPlaying;
        videoScreen.Play(_activeCharacter.Video);
    }

    public void OnNoPressed()
    {
        if (CurrentState != State.AwaitingConfirm) return;

        HideConfirmPanel();
        EnterBrowsing();
    }

    private void OnVideoFinished()
    {
        if (CurrentState != State.VideoPlaying) return;
        EnterBrowsing();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void EnterBrowsing()
    {
        _activeCharacter = null;
        CurrentState = State.Browsing;
        SetBoardsInteractable(true);
    }

    private void SetBoardsInteractable(bool value)
    {
        if (characters == null) return;

        foreach (var c in characters)
        {
            if (c == null || c.Board == null) continue;
            c.Board.SetInteractable(value);
        }
    }

    private void ShowConfirmPanel(TherapyCharacter character)
    {
        CurrentState = State.AwaitingConfirm;

        if (confirmPanel == null) return;

        if (confirmText != null)
            confirmText.text = $"{character.TopicLabel}\n\nAre you ready to get started?";

        // Bring the prompt to whoever was clicked, so it reads as part of that conversation.
        if (character.ConfirmAnchor != null)
        {
            confirmPanel.transform.SetPositionAndRotation(
                character.ConfirmAnchor.position, character.ConfirmAnchor.rotation);
        }

        // if (faceConfirmPanelAtPlayer && playerHead != null)
        // {
        //     Vector3 dir = confirmPanel.transform.position - playerHead.position;
        //     dir.y = 0f;
        //     // if (dir.sqrMagnitude > 0.0001f)
        //     //     confirmPanel.transform.rotation = Quaternion.LookRotation(dir);
        // }

        confirmPanel.SetActive(true);
    }

    private void HideConfirmPanel()
    {
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    private void ShowSubtitle(string text)
    {
        if (subtitleText != null) subtitleText.text = text;
        if (subtitlePanel != null) subtitlePanel.SetActive(!string.IsNullOrEmpty(text));
    }

    private void HideSubtitle()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }
}
