using UnityEngine;

// Pulses between two materials by swapping them on a sine-wave timer.
// Assign dimMat (default) and glowMat (bright) in the Inspector.
public class GlowPulse : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material dimMat;
    [SerializeField] private Material glowMat;
    [SerializeField] private float pulseSpeed = 1.5f; // cycles per second

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        bool glow = Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) > 0f;
        targetRenderer.material = glow ? glowMat : dimMat;
    }
}
