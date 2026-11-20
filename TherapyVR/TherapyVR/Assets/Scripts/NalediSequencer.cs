using System.Collections;
using UnityEngine;

// Orchestrates Naledi's full welcome sequence triggered when the player
// arrives at the front desk. Call StartSequence() from ReceptionManager's
// OnPlayerArrivedAtDesk UnityEvent (wire up in the Inspector).
public class NalediSequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Naledi naledi;
    [SerializeField] private Transform playerHead;       // XR Origin Camera transform

    [Header("Doors — assign left to right")]
    [SerializeField] private Transform[] doorTransforms; // [0]=Door1, [1]=Door2, [2]=Door3

    [Header("Audio — placeholder clips")]
    [SerializeField] private AudioClip welcomeClip;          // "Welcome to Pathfyndr Connect…"
    [SerializeField] private AudioClip roomExplanationClip;  // explains the three rooms

    [Header("Door markers to activate after sequence")]
    [SerializeField] private GameObject[] doorMarkers;  // start disabled in the scene

    [Header("Return Path")]
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform returnSpawnPoint;

    [Header("Timing")]
    [SerializeField] private float pauseBetweenClips = 0.5f;
    [SerializeField] private float gestureRaiseDuration = 1f;
    [SerializeField] private float gestureLowerDuration = 0.5f;
    [SerializeField] private float gestureHoldDuration = 0.8f;  // how long arm stays up per door
    [SerializeField] private float doorIndicateAngle = 40f;     // max degrees she turns toward each door

    private bool _sequenceStarted;

    void Start()
    {
        if (!ScreenFader.IsReturning) return;

        ScreenFader.IsReturning = false;

        if (xrOrigin != null && returnSpawnPoint != null)
            xrOrigin.SetPositionAndRotation(returnSpawnPoint.position, returnSpawnPoint.rotation);

        foreach (var marker in doorMarkers)
            if (marker != null) marker.SetActive(true);

        if (ReceptionManager.Instance != null)
            ReceptionManager.Instance.SetState(ReceptionManager.State.SequenceDone);
    }

    // Called by ReceptionManager.OnPlayerArrivedAtDesk via UnityEvent.
    public void StartSequence()
    {
        if (_sequenceStarted) return;
        _sequenceStarted = true;
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        // 1. Turn to face the player.
        yield return StartCoroutine(naledi.TurnToFace(playerHead));

        // 2. Play welcome clip and wait for it to finish.
        if (welcomeClip != null)
        {
            naledi.Talk(welcomeClip);
            yield return new WaitWhile(() => naledi.IsAudioPlaying);
        }

        yield return new WaitForSeconds(pauseBetweenClips);

        // 3. Play room explanation clip while Naledi keeps facing the player.
        if (roomExplanationClip != null)
        {
            naledi.Talk(roomExplanationClip);
            yield return new WaitWhile(() => naledi.IsAudioPlaying);
        }

        yield return new WaitForSeconds(pauseBetweenClips);

        // 4. Single door indication: slight lean + arm raise toward the middle door, then back to player.
        int indicateIndex = Mathf.Clamp(1, 0, doorTransforms.Length - 1);
        if (doorTransforms.Length > 0 && doorTransforms[indicateIndex] != null)
        {
            yield return StartCoroutine(naledi.TurnTowardPartially(doorTransforms[indicateIndex], doorIndicateAngle));
            yield return StartCoroutine(naledi.RaiseArm(gestureRaiseDuration));
            yield return new WaitForSeconds(gestureHoldDuration);
            yield return StartCoroutine(naledi.LowerArm(gestureLowerDuration));
            yield return StartCoroutine(naledi.TurnToFace(playerHead));
        }

        // 5. Return fully to idle facing.
        yield return StartCoroutine(naledi.ReturnToIdleRotation());

        // 5. Activate all three door markers simultaneously.
        foreach (var marker in doorMarkers)
        {
            if (marker != null)
                marker.SetActive(true);
        }

        ReceptionManager.Instance?.SetState(ReceptionManager.State.SequenceDone);
    }
}
