using Core.EventSystem;
using Scripts.Core.EventSystem;
using Scripts.Network;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TimerText : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        private TextMeshProUGUI _textUI;
        private void Awake()
        {
            packetChannel.AddListener<HandleTimerElapsed>(HandleTimer);
            _textUI = GetComponent<TextMeshProUGUI>();
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleTimerElapsed>(HandleTimer);
        }
        private void HandleTimer(HandleTimerElapsed elapsed)
        {
            _textUI.text = string.Format("{0:F1}", elapsed.remainTime);
        }
    }
}
