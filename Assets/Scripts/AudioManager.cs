using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace DeliveryDriver.Audio
{
    /// <summary>
    /// Centralized audio management system with Audio Mixer integration
    /// Persists across all scenes using DontDestroyOnLoad
    /// Manages music, SFX, UI sounds, and vehicle sounds
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        private static bool _isQuitting = false;
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                // If the application is shutting down, don't return an instance 
                // and don't create a new GameObject.
                if (_isQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.1f, 3f)] public float pitch = 1f;
            public bool loop = false;
            public SoundCategory category = SoundCategory.SFX;

            [HideInInspector] public AudioSource source;
            [HideInInspector] public float lastPlayedTime; // <--- Add this line
        }

        public enum SoundCategory
        {
            Music,
            SFX,
            UI,
            Vehicle,
            Ambient,
            Weapon,      // For shooter game
            Combat,      // For shooter game
            Player       // For runner/general player sounds
        }

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup masterGroup;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup uiGroup;
        [SerializeField] private AudioMixerGroup vehicleGroup;
        [SerializeField] private AudioMixerGroup ambientGroup;
        [SerializeField] private AudioMixerGroup weaponGroup;   // Shooter sounds
        [SerializeField] private AudioMixerGroup combatGroup;   // Shooter combat
        [SerializeField] private AudioMixerGroup playerGroup;   // Player sounds

        [Header("Audio Mixer References")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Sound Library")]
        [SerializeField] private Sound[] sounds;

        [Header("Settings")]
        [SerializeField] private int maxSimultaneousSounds = 20;
        [SerializeField] private bool muteOnFocusLost = false;

        // Runtime sound pools
        private Dictionary<string, Sound> _soundDictionary = new Dictionary<string, Sound>();
        private List<AudioSource> _audioSourcePool = new List<AudioSource>();
        private int _poolIndex = 0;

        // Volume settings (0 to 1)
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;
        private float _uiVolume = 1f;
        private float _vehicleVolume = 1f;
        private float _ambientVolume = 1f;

        // Constants for PlayerPrefs keys
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string UI_VOLUME_KEY = "UIVolume";
        private const string VEHICLE_VOLUME_KEY = "VehicleVolume";
        private const string AMBIENT_VOLUME_KEY = "AmbientVolume";

        // Properties
        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public float SFXVolume => _sfxVolume;
        public float UIVolume => _uiVolume;
        public float VehicleVolume => _vehicleVolume;
        public float AmbientVolume => _ambientVolume;

        //private void Awake()
        //{
        //    // Singleton pattern with proper cleanup
        //    if (_instance == null)
        //    {
        //        _instance = this;
        //        DontDestroyOnLoad(gameObject);
        //        InitializeAudioSystem();
        //        Debug.Log("AudioManager initialized and set to DontDestroyOnLoad");
        //    }
        //    else if (_instance != this)
        //    {
        //        Debug.Log("Duplicate AudioManager found - destroying");
        //        Destroy(gameObject);
        //    }
        //}

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSystem();
        }


        private void InitializeAudioSystem()
        {
            // Create audio source pool
            CreateAudioSourcePool();

            // Build sound dictionary
            BuildSoundDictionary();

            // Load saved volume settings
            LoadVolumeSettings();
        }

        private void CreateAudioSourcePool()
        {
            for (int i = 0; i < maxSimultaneousSounds; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _audioSourcePool.Add(source);
            }
        }

        private void BuildSoundDictionary()
        {
            _soundDictionary.Clear(); // Good practice to clear first

            foreach (Sound sound in sounds)
            {
                if (string.IsNullOrEmpty(sound.name))
                {
                    Debug.LogWarning("Sound with no name found in AudioManager!");
                    continue;
                }

                if (_soundDictionary.ContainsKey(sound.name))
                {
                    Debug.LogWarning($"Duplicate sound name found: {sound.name}");
                    continue;
                }

                // REMOVED: source assignment logic from here
                // We only want to store the data, not assign the player yet.

                _soundDictionary.Add(sound.name, sound);
            }
        }

        private AudioSource GetAvailableAudioSource()
        {
            // Find non-playing source
            foreach (AudioSource source in _audioSourcePool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // Round-robin if all are playing
            AudioSource selectedSource = _audioSourcePool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _audioSourcePool.Count;
            return selectedSource;
        }

        private AudioMixerGroup GetMixerGroup(SoundCategory category)
        {
            return category switch
            {
                SoundCategory.Music => musicGroup,
                SoundCategory.SFX => sfxGroup,
                SoundCategory.UI => uiGroup,
                SoundCategory.Vehicle => vehicleGroup,
                SoundCategory.Ambient => ambientGroup,
                SoundCategory.Weapon => weaponGroup,
                SoundCategory.Combat => combatGroup,
                SoundCategory.Player => playerGroup,
                _ => masterGroup
            };
        }

        #region Play Sound Methods

        /// <summary>
        /// Play a sound by name
        /// </summary>
        public void Play(string soundName)
        {
            if (!_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                Debug.LogWarning($"Sound '{soundName}' not found in AudioManager!");
                return;
            }

            // NEW CHECK: If the sound is already playing, exit the method early.
            if (sound.source != null && sound.source.isPlaying)
            {
                Debug.Log($"Sound '{soundName}' is already playing. Waiting for it to finish.");
                return;
            }

            // 1. Get a fresh source from the pool if we don't have one or if it's inactive
            AudioSource source = GetAvailableAudioSource();

            // 2. Configure the source
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.outputAudioMixerGroup = GetMixerGroup(sound.category);

            // 3. Link the source to the sound so we can check 'isPlaying' next time
            sound.source = source;

            // 4. Play
            source.Play();
        }

        /// <summary>
        /// Play a one-shot sound (doesn't interrupt looping sounds)
        /// </summary>

        public void PlayOneShot(string soundName)
        {
            if (!_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
                return;
            }

            // NEW CHECK: Prevent overlapping if the sound hasn't finished its duration
            float currentTime = Time.time;
            if (currentTime < sound.lastPlayedTime + sound.clip.length)
            {
                // Still playing from a previous PlayOneShot call
                return;
            }

            // Grab a source 
            AudioSource source = GetAvailableAudioSource();

            // Configure and play
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.outputAudioMixerGroup = GetMixerGroup(sound.category);

            source.PlayOneShot(sound.clip);

            // Update the timestamp
            sound.lastPlayedTime = currentTime;
        }

        /// <summary>
        /// Play sound at a specific point in 3D space
        /// </summary>
        public void PlayAtPoint(string soundName, Vector3 position)
        {
            if (!_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
                return;
            }

            if (sound.clip != null)
            {
                AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume);
            }
        }

        /// <summary>
        /// Stop a playing sound
        /// </summary>
        public void Stop(string soundName)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null && sound.source.isPlaying)
                {
                    sound.source.Stop();
                }
            }
        }

        /// <summary>
        /// Stop all sounds in a category
        /// </summary>
        public void StopCategory(SoundCategory category)
        {
            foreach (var sound in _soundDictionary.Values)
            {
                if (sound.category == category && sound.source != null)
                {
                    sound.source.Stop();
                }
            }
        }

        /// <summary>
        /// Stop all playing sounds
        /// </summary>
        public void StopAll()
        {
            foreach (AudioSource source in _audioSourcePool)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// Pause a sound
        /// </summary>
        public void Pause(string soundName)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null && sound.source.isPlaying)
                {
                    sound.source.Pause();
                }
            }
        }

        /// <summary>
        /// Unpause a sound
        /// </summary>
        public void Unpause(string soundName)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null)
                {
                    sound.source.UnPause();
                }
            }
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Set master volume (0 to 1)
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            SetMixerVolume("MasterVolume", _masterVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Set music volume (0 to 1)
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            SetMixerVolume("MusicVolume", _musicVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Set SFX volume (0 to 1)
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            SetMixerVolume("SFXVolume", _sfxVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Set UI volume (0 to 1)
        /// </summary>
        public void SetUIVolume(float volume)
        {
            _uiVolume = Mathf.Clamp01(volume);
            SetMixerVolume("UIVolume", _uiVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Set vehicle volume (0 to 1)
        /// </summary>
        public void SetVehicleVolume(float volume)
        {
            _vehicleVolume = Mathf.Clamp01(volume);
            SetMixerVolume("VehicleVolume", _vehicleVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Set ambient volume (0 to 1)
        /// </summary>
        public void SetAmbientVolume(float volume)
        {
            _ambientVolume = Mathf.Clamp01(volume);
            SetMixerVolume("AmbientVolume", _ambientVolume);
            SaveVolumeSettings();
        }

        /// <summary>
        /// Internal method to set mixer volume (converts 0-1 to decibels)
        /// </summary>
        private void SetMixerVolume(string parameterName, float normalizedVolume)
        {
            if (audioMixer == null)
            {
                Debug.LogWarning("AudioMixer not assigned in AudioManager!");
                return;
            }

            // Convert 0-1 to decibels (-80dB to 0dB)
            // Formula: dB = 20 * log10(linear)
            float volumeDB = normalizedVolume > 0.0001f
                ? Mathf.Log10(normalizedVolume) * 20f
                : -80f;

            audioMixer.SetFloat(parameterName, volumeDB);
        }

        #endregion

        #region Save/Load Settings

        private void SaveVolumeSettings()
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _masterVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _musicVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _sfxVolume);
            PlayerPrefs.SetFloat(UI_VOLUME_KEY, _uiVolume);
            PlayerPrefs.SetFloat(VEHICLE_VOLUME_KEY, _vehicleVolume);
            PlayerPrefs.SetFloat(AMBIENT_VOLUME_KEY, _ambientVolume);
            PlayerPrefs.Save();
        }

        private void LoadVolumeSettings()
        {
            _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            _uiVolume = PlayerPrefs.GetFloat(UI_VOLUME_KEY, 1f);
            _vehicleVolume = PlayerPrefs.GetFloat(VEHICLE_VOLUME_KEY, 1f);
            _ambientVolume = PlayerPrefs.GetFloat(AMBIENT_VOLUME_KEY, 1f);

            // Apply loaded settings to mixer
            SetMixerVolume("MasterVolume", _masterVolume);
            SetMixerVolume("MusicVolume", _musicVolume);
            SetMixerVolume("SFXVolume", _sfxVolume);
            SetMixerVolume("UIVolume", _uiVolume);
            SetMixerVolume("VehicleVolume", _vehicleVolume);
            SetMixerVolume("AmbientVolume", _ambientVolume);
        }

        public void ResetToDefaultVolumes()
        {
            SetMasterVolume(1f);
            SetMusicVolume(1f);
            SetSFXVolume(1f);
            SetUIVolume(1f);
            SetVehicleVolume(1f);
            SetAmbientVolume(1f);
        }

        /// <summary>
        /// Set pitch for a specific sound (useful for engine sounds)
        /// </summary>
        public void SetPitch(string soundName, float pitch)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null)
                {
                    sound.source.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
                }
            }
        }

        /// <summary>
        /// Get the AudioSource for a sound (for advanced control)
        /// </summary>
        public AudioSource GetAudioSource(string soundName)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                return sound.source;
            }
            return null;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if a sound is currently playing
        /// </summary>
        public bool IsPlaying(string soundName)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                return sound.source != null && sound.source.isPlaying;
            }
            return false;
        }

        /// <summary>
        /// Fade in a sound over time
        /// </summary>
        public void FadeIn(string soundName, float duration)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null)
                {
                    StartCoroutine(FadeAudioSource(sound.source, 0f, sound.volume, duration, true));
                }
            }
        }

        /// <summary>
        /// Fade out a sound over time
        /// </summary>
        public void FadeOut(string soundName, float duration)
        {
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (sound.source != null)
                {
                    StartCoroutine(FadeAudioSource(sound.source, sound.source.volume, 0f, duration, false));
                }
            }
        }

        private System.Collections.IEnumerator FadeAudioSource(AudioSource source, float startVolume, float endVolume, float duration, bool playOnStart)
        {
            if (playOnStart && !source.isPlaying)
            {
                source.volume = startVolume;
                source.Play();
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
                yield return null;
            }

            source.volume = endVolume;

            if (endVolume == 0f)
            {
                source.Stop();
            }
        }

        #endregion

        #region Application Focus&Quit Handling

        private void OnApplicationFocus(bool hasFocus)
        {
            if (muteOnFocusLost)
            {
                AudioListener.pause = !hasFocus;
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void OnDestroy()
        {
            // If THIS specific instance is the singleton, clear the reference
            if (_instance == this)
            {
                _instance = null;
            }
        }

        #endregion
    }
}