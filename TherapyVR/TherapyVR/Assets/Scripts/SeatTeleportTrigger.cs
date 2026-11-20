using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

// Attach to the chair's TeleportationAnchor in Scene 2.
// When the player teleports to the seat this fires StartQuestionnaire() once.
[RequireComponent(typeof(TeleportationAnchor))]
public class SeatTeleportTrigger : MonoBehaviour
{
    [SerializeField] private GlowPulse seatGlowPulse;
    [SerializeField] private UnityEvent onSeated; // optional direct reference to PHQ9Manager

    private TeleportationAnchor _anchor;
    private bool _triggered;

    void Awake()
    {
        _anchor = GetComponent<TeleportationAnchor>();
    }

    void OnEnable()
    {
        _anchor.teleporting.AddListener(OnSeated);
    }

    void OnDisable()
    {
        _anchor.teleporting.RemoveListener(OnSeated);
    }

    private void OnSeated(TeleportingEventArgs args)
    {
        if (_triggered) return;
        _triggered = true;

        if (seatGlowPulse != null) seatGlowPulse.enabled = false;
        PHQ9Manager.Instance?.StartQuestionnaire();
        onSeated?.Invoke();
    }
}
