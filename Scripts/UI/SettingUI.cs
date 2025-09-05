using Core.EventSystem;
using DewmoLib.Dependencies;
using Scripts.Core.EventSystem;
using Scripts.Core.GameSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
namespace Scripts.UI
{


    public class SettingUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject settingUI;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider smoothSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Audio")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private PlayerInputSO playerInput;
        // PlayerPrefs Keys
        public const string MASTER_KEY = "Volume_Master";
        public const string SMOOTH_KEY = "Smooth";
        public const string SFX_KEY = "Volume_SFX";
        [SerializeField] private TextMeshProUGUI smoothTxt;
        [SerializeField] private EventChannelSO packetChannel;
        private void OnEnable()
        {
            LoadVolumes();

            // Add listeners
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            smoothSlider.onValueChanged.AddListener(SetSmooth);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        private void Awake()
        {
            playerInput.OnSettingEvent += HandleSetting;
            packetChannel.AddListener<HandleLeaveRoom>(LeaveRoom);
            OnHide();
        }

        private void LeaveRoom(HandleLeaveRoom room)
        {
            _beforeMode = CursorLockMode.Confined;
        }

        private void OnDestroy()
        {
            playerInput.OnSettingEvent -= HandleSetting;
            packetChannel.RemoveListener<HandleLeaveRoom>(LeaveRoom);
        }
        bool _isOpened;
        private void HandleSetting()
        {
            if (_isOpened)
                OnHide();
            else
                OnShow();
        }
        private CursorLockMode _beforeMode;
        protected void OnShow()
        {
            settingUI.SetActive(true);
            SetSlide();
            _beforeMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.Confined;
            _isOpened = true;
            playerInput.SetEnable(false);
        }

        protected void OnHide()
        {
            playerInput.SetEnable(true);
            settingUI.SetActive(false);
            Cursor.lockState = _beforeMode;
            _isOpened = false;
        }

        private void LoadVolumes()
        {
            float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
            float bgm = PlayerPrefs.GetFloat(SMOOTH_KEY, 0.5f);
            float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.5f);

            SetMasterVolume(master);
            SetSmooth(bgm);
            SetSFXVolume(sfx);
        }
        private void SetSlide()
        {
            float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
            float smooth = PlayerPrefs.GetFloat(SMOOTH_KEY, 0.5f);
            float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0.5f);

            masterSlider.value = master;
            smoothSlider.value = smooth;
            sfxSlider.value = sfx;
        }

        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
            PlayerPrefs.SetFloat(MASTER_KEY, value);
        }

        public void SetSmooth(float value)
        {
            PlayerPrefs.SetFloat(SMOOTH_KEY, value);
            smoothTxt.text = String.Format("{0:F2}",value);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
            PlayerPrefs.SetFloat(SFX_KEY, value);
        }

        public void OnConfirmButton()
        {
            PlayerPrefs.Save(); // 저장 명시적으로 실행
            HandleSetting();
        }
        public void Quit()
        {
            Application.Quit();
        }
    }
}
