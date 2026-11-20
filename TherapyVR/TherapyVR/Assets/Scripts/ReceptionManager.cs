using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

// Scene-level state machine for the reception flow.
// Wire up: assign frontDeskAnchor and lookAroundPrompt in the Inspector.
public class ReceptionManager : MonoBehaviour
{
    public static ReceptionManager Instance { get; private set; }

    public enum State { Spawned, AtFrontDesk, SequenceDone, Departed }

    [Header("References")]
    [SerializeField] private TeleportationAnchor frontDeskAnchor;
    [SerializeField] private LookAroundPrompt lookAroundPrompt;
    [SerializeField] private GlowPulse frontDeskGlowPulse; // disabled once player arrives

    [Header("Events")]
    public UnityEvent OnPlayerArrivedAtDesk;

    public State CurrentState { get; private set; } = State.Spawned;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (ScreenFader.IsReturning)
            CurrentState = State.SequenceDone;
    }

    void OnEnable()
    {
        if (frontDeskAnchor != null)
            frontDeskAnchor.teleporting.AddListener(HandleFrontDeskTeleport);
    }

    void OnDisable()
    {
        if (frontDeskAnchor != null)
            frontDeskAnchor.teleporting.RemoveListener(HandleFrontDeskTeleport);
    }

    private void HandleFrontDeskTeleport(TeleportingEventArgs args)
    {
        if (CurrentState != State.Spawned) return;

        SetState(State.AtFrontDesk);
        lookAroundPrompt?.Hide();
        if (frontDeskGlowPulse != null) frontDeskGlowPulse.enabled = false;
        OnPlayerArrivedAtDesk?.Invoke();
    }

    public void SetState(State newState)
    {
        CurrentState = newState;
    }
}
