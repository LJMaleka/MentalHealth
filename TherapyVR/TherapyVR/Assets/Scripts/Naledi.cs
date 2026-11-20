using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Naledi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinnedMeshRenderer faceMesh;

    [Header("Body")]
    [SerializeField] private Transform armBone;                            // optional — right upper arm bone
    [SerializeField] private Vector3 armRaisedLocalEuler = new Vector3(-60f, 0f, 0f); // tune in Inspector
    [SerializeField] private float turnSpeed = 90f;

    [Header("Blink")]
    [SerializeField] private float blinkMinInterval = 2f;
    [SerializeField] private float blinkMaxInterval = 6f;
    [SerializeField] private float blinkDuration = 0.12f;

    [Header("Talk")]
    [SerializeField] private float talkMouthMaxOpen = 65f;
    [SerializeField] private float talkMouthSmoothing = 12f;

    [Header("Smile")]
    [SerializeField] private float smileMinInterval = 5f;
    [SerializeField] private float smileMaxInterval = 10f;
    [SerializeField] private float smileMaxValue = 75f;
    [SerializeField] private float smileHoldDuration = 2.5f;
    [SerializeField] private float smileFadeSpeed = 1.5f;

    private AudioSource audioSource;
    private Animator _animator;

    private int idxBlinkLeft, idxBlinkRight;
    private int idxMouthOpen, idxJawDown;
    private int idxSmileLeft, idxSmileRight;

    private bool isTalking;
    private float currentMouthOpen;
    private readonly float[] audioSamples = new float[256];

    private Quaternion _defaultBodyRotation;
    private Quaternion _defaultArmRotation;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _animator = GetComponentInChildren<Animator>();

        if (faceMesh == null)
            faceMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        var mesh = faceMesh.sharedMesh;
        idxBlinkLeft  = mesh.GetBlendShapeIndex("Facial_Blends.Blink_Left");
        idxBlinkRight = mesh.GetBlendShapeIndex("Facial_Blends.Blink_Right");
        idxMouthOpen  = mesh.GetBlendShapeIndex("Facial_Blends.MouthOpen");
        idxJawDown    = mesh.GetBlendShapeIndex("Facial_Blends.Jaw_Down");
        idxSmileLeft  = mesh.GetBlendShapeIndex("Facial_Blends.Smile_Left");
        idxSmileRight = mesh.GetBlendShapeIndex("Facial_Blends.Smile_Right");
    }

    void Start()
    {
        _defaultBodyRotation = transform.rotation;
        if (armBone != null) _defaultArmRotation = armBone.localRotation;

        StartCoroutine(BlinkLoop());
        StartCoroutine(SmileLoop());
    }

    void Update()
    {
        if (!isTalking) return;

        if (!audioSource.isPlaying)
        {
            StopTalking();
            return;
        }

        audioSource.GetOutputData(audioSamples, 0);
        float sum = 0f;
        for (int i = 0; i < audioSamples.Length; i++)
            sum += audioSamples[i] * audioSamples[i];

        float rms = Mathf.Sqrt(sum / audioSamples.Length);
        float target = Mathf.Clamp01(rms * 25f) * talkMouthMaxOpen;

        currentMouthOpen = Mathf.Lerp(currentMouthOpen, target, Time.deltaTime * talkMouthSmoothing);
        faceMesh.SetBlendShapeWeight(idxMouthOpen, currentMouthOpen);
        faceMesh.SetBlendShapeWeight(idxJawDown, currentMouthOpen * 0.4f);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Talk(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        isTalking = true;
    }

    public void StopTalking()
    {
        isTalking = false;
        audioSource.Stop();
        currentMouthOpen = 0f;
        faceMesh.SetBlendShapeWeight(idxMouthOpen, 0f);
        faceMesh.SetBlendShapeWeight(idxJawDown, 0f);
    }

    // ── Body movement API ─────────────────────────────────────────────────────

    // Rotates Naledi's body to face target on the Y axis at turnSpeed deg/s.
    // Waits one frame first so XR Origin has fully settled after a teleport.
    public IEnumerator TurnToFace(Transform target, float speed = -1f)
    {
        yield return null; // let XR Origin finish moving

        float deg = speed > 0f ? speed : turnSpeed;
        while (true)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) break;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            if (Quaternion.Angle(transform.rotation, targetRot) <= 0.5f) break;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, deg * Time.deltaTime);
            yield return null;
        }
    }

    // Raises armBone to the configured local pose over duration seconds.
    // Tune armRaisedLocalEuler in the Inspector to match your rig.
    // Safe no-op when armBone is not assigned.
    public IEnumerator RaiseArm(float duration = 1f)
    {
        if (armBone == null) yield break;

        if (_animator != null) _animator.enabled = false;

        Quaternion start = armBone.localRotation;
        Quaternion raised = Quaternion.Euler(armRaisedLocalEuler);

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            armBone.localRotation = Quaternion.Slerp(start, raised, t / duration);
            yield return null;
        }
        armBone.localRotation = raised;
    }

    // Returns armBone to its rest rotation over duration seconds, then re-enables the Animator.
    public IEnumerator LowerArm(float duration = 0.5f)
    {
        if (armBone == null) yield break;

        Quaternion start = armBone.localRotation;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            armBone.localRotation = Quaternion.Slerp(start, _defaultArmRotation, t / duration);
            yield return null;
        }
        armBone.localRotation = _defaultArmRotation;

        if (_animator != null) _animator.enabled = true;
    }

    // Rotates partially toward target — at most maxAngle degrees — then stops.
    // Used for the subtle "indicating a door" lean without fully facing away from the player.
    public IEnumerator TurnTowardPartially(Transform target, float maxAngle = 40f, float speed = -1f)
    {
        yield return null; // settle frame

        float deg = speed > 0f ? speed : turnSpeed;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) yield break;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        // Clamp so she never turns more than maxAngle away from current rotation.
        float angleDiff = Quaternion.Angle(transform.rotation, targetRot);
        if (angleDiff > maxAngle)
            targetRot = Quaternion.Slerp(transform.rotation, targetRot, maxAngle / angleDiff);

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, deg * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    // Rotates body back to the default idle rotation at speed deg/s.
    public IEnumerator ReturnToIdleRotation(float speed = -1f)
    {
        float deg = speed > 0f ? speed : turnSpeed;
        while (Quaternion.Angle(transform.rotation, _defaultBodyRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, _defaultBodyRotation, deg * Time.deltaTime);
            yield return null;
        }
        transform.rotation = _defaultBodyRotation;
    }

    // Exposes the AudioSource so NalediSequencer can poll isPlaying.
    public bool IsAudioPlaying => audioSource.isPlaying;

    // ── Blink ─────────────────────────────────────────────────────────────────

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkMinInterval, blinkMaxInterval));
            yield return StartCoroutine(DoBlink());
        }
    }

    IEnumerator DoBlink()
    {
        float half = blinkDuration * 0.5f;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(0f, 100f, t / half);
            faceMesh.SetBlendShapeWeight(idxBlinkLeft, v);
            faceMesh.SetBlendShapeWeight(idxBlinkRight, v);
            yield return null;
        }

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            float v = Mathf.Lerp(100f, 0f, t / half);
            faceMesh.SetBlendShapeWeight(idxBlinkLeft, v);
            faceMesh.SetBlendShapeWeight(idxBlinkRight, v);
            yield return null;
        }

        faceMesh.SetBlendShapeWeight(idxBlinkLeft, 0f);
        faceMesh.SetBlendShapeWeight(idxBlinkRight, 0f);
    }

    // ── Smile ─────────────────────────────────────────────────────────────────

    IEnumerator SmileLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(smileMinInterval, smileMaxInterval));
            yield return StartCoroutine(DoSmile());
        }
    }

    IEnumerator DoSmile()
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime * smileFadeSpeed)
        {
            float v = Mathf.Lerp(0f, smileMaxValue, t);
            faceMesh.SetBlendShapeWeight(idxSmileLeft, v);
            faceMesh.SetBlendShapeWeight(idxSmileRight, v);
            yield return null;
        }

        faceMesh.SetBlendShapeWeight(idxSmileLeft, smileMaxValue);
        faceMesh.SetBlendShapeWeight(idxSmileRight, smileMaxValue);

        yield return new WaitForSeconds(smileHoldDuration);

        for (float t = 0f; t < 1f; t += Time.deltaTime * smileFadeSpeed)
        {
            float v = Mathf.Lerp(smileMaxValue, 0f, t);
            faceMesh.SetBlendShapeWeight(idxSmileLeft, v);
            faceMesh.SetBlendShapeWeight(idxSmileRight, v);
            yield return null;
        }

        faceMesh.SetBlendShapeWeight(idxSmileLeft, 0f);
        faceMesh.SetBlendShapeWeight(idxSmileRight, 0f);
    }
}
