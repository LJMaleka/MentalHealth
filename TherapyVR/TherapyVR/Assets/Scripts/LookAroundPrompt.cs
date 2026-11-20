using System.Collections;
using TMPro;
using UnityEngine;

// Displays a brief instruction prompt when the scene starts.
// Hides automatically after hideAfterSeconds, or when Hide() is called externally.
public class LookAroundPrompt : MonoBehaviour
{
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private float hideAfterSeconds = 8f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private string message = "Point your controller at the glowing marker and pull the trigger";

    private CanvasGroup _canvasGroup;
    private Coroutine _hideRoutine;

    void Awake()
    {
        if (promptRoot == null)
            promptRoot = gameObject;

        _canvasGroup = promptRoot.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = promptRoot.AddComponent<CanvasGroup>();

        if (promptText != null)
            promptText.text = message;

        if (ScreenFader.IsReturning)
            promptRoot.SetActive(false);
    }

    void Start()
    {
        if (!promptRoot.activeSelf) return;
        _canvasGroup.alpha = 1f;
        promptRoot.SetActive(true);
        _hideRoutine = StartCoroutine(AutoHide());
    }

    public void Hide()
    {
        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(hideAfterSeconds);
        yield return FadeOut();
    }

    private IEnumerator FadeOut()
    {
        float start = _canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;
        promptRoot.SetActive(false);
    }
}
