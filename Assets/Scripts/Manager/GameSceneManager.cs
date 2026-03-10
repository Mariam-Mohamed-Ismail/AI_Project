using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DeliveryDriver.Audio;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool _isTransitioning = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure the fade starts transparent when the game launches
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0;
    }

    private void Start()
    {
        // Optional: Fade in the very first scene of the game
        StartCoroutine(Fade(0));
    }

    public void ReloadLevel()
    {
        TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            TransitionToScene(nextIndex);
        else
            TransitionToScene(0);
    }

    public void LoadLevelByIndex(int index)
    {
        AudioManager.Instance.Stop("background");
        TransitionToScene(index);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        TransitionToScene(0);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void TransitionToScene(int index)
    {
        if (_isTransitioning) return;
        StartCoroutine(ExecuteTransition(index));
    }

    private IEnumerator ExecuteTransition(int sceneIndex)
    {
        _isTransitioning = true;

        // 1. Fade Out (To Black)
        yield return StartCoroutine(Fade(1));

        // 2. Load Scene
        yield return SceneManager.LoadSceneAsync(sceneIndex);

        // 3. Fade In (To Gameplay)
        yield return StartCoroutine(Fade(0));

        _isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}