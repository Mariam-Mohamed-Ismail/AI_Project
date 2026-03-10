using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DeliveryDriver.Audio;

namespace DeliveryDriver.UI
{
    /// <summary>
    /// Main menu manager for the game
    /// Handles scene loading, settings, and menu navigation
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings")]
        [SerializeField] private AudioSettingsMenu settingsMenu;

        [Header("Scene Management")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Audio")]
        [SerializeField] private string menuMusicName = "MenuMusic";
        [SerializeField] private string buttonClickSound = "ButtonClick";
        [SerializeField] private string buttonHoverSound = "UIHover";

        private void Start()
        {
            InitializeMenu();
            SetupButtonListeners();
            PlayMenuMusic();
        }

        private void InitializeMenu()
        {
            ShowMainMenu();

            // Ensure AudioManager exists
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("AudioManager not found! Creating one...");
                GameObject audioManagerGO = new GameObject("AudioManager");
                audioManagerGO.AddComponent<AudioManager>();
            }
        }

        private void SetupButtonListeners()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
                AddHoverSound(playButton);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
                AddHoverSound(settingsButton);
            }

            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(OnCreditsClicked);
                AddHoverSound(creditsButton);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
                AddHoverSound(quitButton);
            }
        }

        private void AddHoverSound(Button button)
        {
            // Add EventTrigger for hover sound
            UnityEngine.EventSystems.EventTrigger trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }

            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(entry);
        }

        private void PlayMenuMusic()
        {
            if (!string.IsNullOrEmpty(menuMusicName))
            {
                AudioManager.Instance?.Play(menuMusicName);
            }
        }

        private void PlayClickSound()
        {
            if (!string.IsNullOrEmpty(buttonClickSound))
            {
                AudioManager.Instance?.PlayOneShot(buttonClickSound);
            }
        }

        private void PlayHoverSound()
        {
            if (!string.IsNullOrEmpty(buttonHoverSound))
            {
                AudioManager.Instance?.PlayOneShot(buttonHoverSound);
            }
        }

        #region Button Handlers

        private void OnPlayClicked()
        {
            PlayClickSound();
            LoadGameScene();
        }

        private void OnSettingsClicked()
        {
            PlayClickSound();
            ShowSettings();
        }

        private void OnCreditsClicked()
        {
            PlayClickSound();
            ShowCredits();
        }

        private void OnQuitClicked()
        {
            PlayClickSound();
            QuitGame();
        }

        #endregion

        #region Menu Navigation

        private void ShowMainMenu()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }

        private void ShowSettings()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
            if (creditsPanel != null) creditsPanel.SetActive(false);

            if (settingsMenu != null)
            {
                settingsMenu.OpenSettings();
            }
        }

        private void ShowCredits()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        public void BackToMainMenu()
        {
            PlayClickSound();
            ShowMainMenu();
        }

        #endregion

        #region Scene Management

        private void LoadGameScene()
        {
            // Stop menu music
            if (!string.IsNullOrEmpty(menuMusicName))
            {
                AudioManager.Instance?.FadeOut(menuMusicName, 1f);
            }

            // Load game scene
            StartCoroutine(LoadSceneAsync(gameSceneName));
        }

        private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
        {
            // Optional: Show loading screen here

            yield return new WaitForSeconds(0.5f); // Brief delay for audio fade

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                // Optional: Update loading bar
                // float progress = asyncLoad.progress;
                yield return null;
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion

        private void OnDestroy()
        {
            // Clean up button listeners
            if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
            if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (creditsButton != null) creditsButton.onClick.RemoveListener(OnCreditsClicked);
            if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }
}