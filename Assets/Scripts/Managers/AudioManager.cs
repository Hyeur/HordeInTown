using UnityEngine;
using System.Collections.Generic;

namespace HordeInTown.Managers
{
    /// <summary>
    /// Manages all audio: BGM and SFX
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("BGM Clips")]
        [SerializeField] private AudioClip mainMenuBGM;
        [SerializeField] private AudioClip defeatedBGM;
        
        [Header("SFX Clips - Single")]
        [SerializeField] private AudioClip buttonPressSFX;
        [SerializeField] private AudioClip bowLoadingSFX;
        
        [Header("SFX Clips - Multiple Variations")]
        [SerializeField] private AudioClip[] zombieWalkSFX; // Array for multiple variations
        [SerializeField] private AudioClip[] zombieDeathSFX; // Array for multiple variations
        [SerializeField] private AudioClip[] arrowShotSFX; // Array for multiple variations
        
        [Header("Volume Settings")]
        [SerializeField] private float bgmVolume = 1f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private bool bgmMuted = false;
        [SerializeField] private bool sfxMuted = false;
        
        private Dictionary<string, AudioClip[]> sfxArrayDictionary;
        private Dictionary<string, AudioClip> sfxSingleDictionary;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Create audio sources if not assigned
                if (bgmSource == null)
                {
                    bgmSource = gameObject.AddComponent<AudioSource>();
                    bgmSource.loop = true;
                }
                
                if (sfxSource == null)
                {
                    sfxSource = gameObject.AddComponent<AudioSource>();
                }
                
                // Build SFX dictionaries
                BuildSFXDictionaries();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void BuildSFXDictionaries()
        {
            // Dictionary for single clips
            sfxSingleDictionary = new Dictionary<string, AudioClip>
            {
                { "button_press", buttonPressSFX },
                { "bow_loading", bowLoadingSFX }
            };
            
            // Dictionary for multiple variations (arrays)
            sfxArrayDictionary = new Dictionary<string, AudioClip[]>
            {
                { "zombie_walk", zombieWalkSFX },
                { "zombie_dying", zombieDeathSFX },
                { "arrow_shot", arrowShotSFX }
            };
        }
        
        public void PlayBGM(string bgmName, bool loop = true)
        {
            AudioClip clip = null;
            
            switch (bgmName.ToLower())
            {
                case "mainmenu":
                case "mainmenu.bgm":
                    clip = mainMenuBGM;
                    break;
                case "defeated":
                case "defeated.bgm":
                    clip = defeatedBGM;
                    break;
            }
            
            if (clip != null && bgmSource != null)
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            }
        }
        
        public void PlaySFX(string sfxName)
        {
            if (sfxSource == null) return;
            
            AudioClip clip = null;
            
            // Check if it's a single clip
            if (sfxSingleDictionary.ContainsKey(sfxName))
            {
                clip = sfxSingleDictionary[sfxName];
            }
            // Check if it's an array with multiple variations
            else if (sfxArrayDictionary.ContainsKey(sfxName))
            {
                AudioClip[] clips = sfxArrayDictionary[sfxName];
                if (clips != null && clips.Length > 0)
                {
                    // Filter out null clips
                    var validClips = new System.Collections.Generic.List<AudioClip>();
                    foreach (var c in clips)
                    {
                        if (c != null) validClips.Add(c);
                    }
                    
                    if (validClips.Count > 0)
                    {
                        // Randomly select one variation
                        clip = validClips[Random.Range(0, validClips.Count)];
                    }
                }
            }
            
            // Play the selected clip (only if not muted)
            if (clip != null && !sfxMuted)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }
        
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            if (bgmSource != null)
            {
                bgmSource.volume = bgmMuted ? 0f : bgmVolume;
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        
        public void ToggleBGM()
        {
            bgmMuted = !bgmMuted;
            if (bgmSource != null)
            {
                bgmSource.volume = bgmMuted ? 0f : bgmVolume;
            }
        }
        
        public void ToggleSFX()
        {
            sfxMuted = !sfxMuted;
        }
        
        public bool IsBGMMuted() => bgmMuted;
        public bool IsSFXMuted() => sfxMuted;
        
        public float GetBGMVolume() => bgmVolume;
        public float GetSFXVolume() => sfxVolume;
        
        public void StopBGM()
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }
    }
}

