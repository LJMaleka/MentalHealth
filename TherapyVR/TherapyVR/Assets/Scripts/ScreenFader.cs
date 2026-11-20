using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    public static bool IsReturning = false;
    public float fadeWaitTime = 1.0f;
    public Color fadeColor = Color.black;

    private Texture2D fadeTexture;
    private float alpha = 1.0f;
    private int drawDepth = -1000;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, fadeColor);
        fadeTexture.Apply();
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ScreenFadeIn(1);

    public void ScreenFadeIn(float fadeDuration)
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    public void ScreenFadeOut(float fadeDuration)
    {
        StartCoroutine(FadeOut(fadeDuration));
    }

    public void ScreenFadeInAndOut()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        yield return FadeOut(2.5f);
        yield return new WaitForSeconds(fadeWaitTime);
        yield return FadeIn(1);
    }

    private IEnumerator FadeIn(float fadeDuration)
    {
        alpha = 1.0f;
        float timer = 0.0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            alpha = 1.0f - (timer / fadeDuration);
            yield return null;
        }
        alpha = 0.0f;
    }

    private IEnumerator FadeOut(float fadeDuration)
    {
        alpha = 0.0f;
        float timer = 0.0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            alpha = timer / fadeDuration;
            yield return null;
        }
        alpha = 1.0f;
    }

    private void OnGUI()
    {
        GUI.depth = drawDepth;
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }
}
