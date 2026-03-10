using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DeliveryDriver.Audio;

namespace DeliveryDriver.UI
{
    /// <summary>
    /// Settings menu for controlling audio and other game settings
    /// Can be used in Main Menu or Pause Menu
    /// </summary>
    public class AudioSettingsMenu : MonoBehaviour
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider uiVolumeSlider;
        [SerializeField] private Slider vehicleVolumeSlider;
        [SerializeField] private Slider ambientVolumeSlider;

        [Header("Volume Labels (Optional)")]
        [SerializeField] private TextMeshProUGUI masterVolumeLabel;
        [SerializeField] private TextMeshProUGUI musicVolumeLabel;
        [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
        [SerializeField] private TextMeshProUGUI uiVolumeLabel;
        [SerializeField] private TextMeshProUGUI vehicleVolumeLabel;
        [SerializeField] private TextMeshProUGUI ambientVolumeLabel;

        [Header("UI References")]
        [SerializeField] private Button resetButton;

        private void OnEnable()
        {
            LoadCurrentSettings();
            SetupListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            // Add slider listeners
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

            if (uiVolumeSlider != null)
                uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);

            if (vehicleVolumeSlider != null)
                vehicleVolumeSlider.onValueChanged.AddListener(OnVehicleVolumeChanged);

            if (ambientVolumeSlider != null)
                ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChanged);

            // Button listeners
            if (resetButton != null)
                resetButton.onClick.AddListener(ResetToDefaults);
        }

        private void RemoveListeners()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);

            if (uiVolumeSlider != null)
                uiVolumeSlider.onValueChanged.RemoveListener(OnUIVolumeChanged);

            if (vehicleVolumeSlider != null)
                vehicleVolumeSlider.onValueChanged.RemoveListener(OnVehicleVolumeChanged);

            if (ambientVolumeSlider != null)
                ambientVolumeSlider.onValueChanged.RemoveListener(OnAmbientVolumeChanged);

            if (resetButton != null)
                resetButton.onClick.RemoveListener(ResetToDefaults);
        }

        private void LoadCurrentSettings()
        {
            if (AudioManager.Instance == null) return;

            // Load current values from AudioManager
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
                UpdateVolumeLabel(masterVolumeLabel, AudioManager.Instance.MasterVolume);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
                UpdateVolumeLabel(musicVolumeLabel, AudioManager.Instance.MusicVolume);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
                UpdateVolumeLabel(sfxVolumeLabel, AudioManager.Instance.SFXVolume);
            }

            if (uiVolumeSlider != null)
            {
                uiVolumeSlider.value = AudioManager.Instance.UIVolume;
                UpdateVolumeLabel(uiVolumeLabel, AudioManager.Instance.UIVolume);
            }

            if (vehicleVolumeSlider != null)
            {
                vehicleVolumeSlider.value = AudioManager.Instance.VehicleVolume;
                UpdateVolumeLabel(vehicleVolumeLabel, AudioManager.Instance.VehicleVolume);
            }

            if (ambientVolumeSlider != null)
            {
                ambientVolumeSlider.value = AudioManager.Instance.AmbientVolume;
                UpdateVolumeLabel(ambientVolumeLabel, AudioManager.Instance.AmbientVolume);
            }
        }

        #region Volume Change Handlers

        private void OnMasterVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMasterVolume(value);
            UpdateVolumeLabel(masterVolumeLabel, value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
            UpdateVolumeLabel(musicVolumeLabel, value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
            UpdateVolumeLabel(sfxVolumeLabel, value);
        }

        private void OnUIVolumeChanged(float value)
        {
            AudioManager.Instance?.SetUIVolume(value);
            UpdateVolumeLabel(uiVolumeLabel, value);
        }

        private void OnVehicleVolumeChanged(float value)
        {
            AudioManager.Instance?.SetVehicleVolume(value);
            UpdateVolumeLabel(vehicleVolumeLabel, value);
        }

        private void OnAmbientVolumeChanged(float value)
        {
            AudioManager.Instance?.SetAmbientVolume(value);
            UpdateVolumeLabel(ambientVolumeLabel, value);
        }

        #endregion

        private void UpdateVolumeLabel(TextMeshProUGUI label, float value)
        {
            if (label != null)
            {
                label.text = Mathf.RoundToInt(value * 100f) + "%";
            }
        }

        private void ResetToDefaults()
        {
            AudioManager.Instance?.ResetToDefaultVolumes();
            LoadCurrentSettings();
        }

        public void OpenSettings()
        {
            gameObject.SetActive(true);
            LoadCurrentSettings();
        }
    }
}