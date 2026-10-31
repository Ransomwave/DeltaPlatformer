using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace deltaformer {
    public class UIOptions : MonoBehaviour {
        [Header("Audio")]
        public AudioMixer audioMixer; // Opcional: Arrastra tu AudioMixer aquí si lo usas

        [Header("UI References")]
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Toggle fullScreenToggle;

        private void Start() {
            // Cargar el volumen general guardado (por defecto a 1 si es la primera vez)
            if (masterSlider != null) {
                float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 1f);
                masterSlider.value = savedMaster;
                SetMasterVolume(savedMaster);
            }

            // Cargar volumen de música
            if (musicSlider != null) {
                float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
                musicSlider.value = savedMusic;
                SetMusicVolume(savedMusic);
            }

            // Cargar volumen de efectos (SFX)
            if (sfxSlider != null) {
                float savedSfx = PlayerPrefs.GetFloat("SfxVolume", 1f);
                sfxSlider.value = savedSfx;
                SetSfxVolume(savedSfx);
            }

            // Configurar el estado inicial y guardado del Toggle de pantalla completa
            if (fullScreenToggle != null) {
                bool isFullScreen = PlayerPrefs.GetInt("FullScreen", Screen.fullScreen ? 1 : 0) == 1;
                fullScreenToggle.isOn = isFullScreen;
                Screen.fullScreen = isFullScreen;
            }
        }

        // Método para el Master Volume
        public void SetMasterVolume(float volume) {
            PlayerPrefs.SetFloat("MasterVolume", volume);

            if (audioMixer != null) {
                audioMixer.SetFloat("Master", volume > 0 ? Mathf.Log10(volume) * 20 : -80f);
            } else {
                AudioListener.volume = volume; // Si no usas AudioMixer, afecta al volumen global
            }
        }

        // Método para el Music Volume
        public void SetMusicVolume(float volume) {
            PlayerPrefs.SetFloat("MusicVolume", volume);

            if (audioMixer != null) {
                audioMixer.SetFloat("Music", volume > 0 ? Mathf.Log10(volume) * 20 : -80f);
            }
        }

        // Método para el SFX Volume
        public void SetSfxVolume(float volume) {
            PlayerPrefs.SetFloat("SfxVolume", volume);

            if (audioMixer != null) {
                audioMixer.SetFloat("SFX", volume > 0 ? Mathf.Log10(volume) * 20 : -80f);
            }
        }

        // Método para la Pantalla Completa
        public void SetFullScreen(bool isFullScreen) {
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        }
    }
}