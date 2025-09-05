using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.InGame
{
    public class GaugeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image barImage;
        [SerializeField] private EventChannelSO uiChannel;
        private void Awake()
        {
            uiChannel.AddListener<SetGaugeEvent>(HandleGaugeEvent);
            uiChannel.AddListener<SetGaugeActiveEvent>(HandleGaugeActiveEvent);
            gameObject.SetActive(false);
        }

        private async void HandleGaugeActiveEvent(SetGaugeActiveEvent @event)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            gameObject.SetActive(@event.active);
        }

        private void OnDestroy()
        {
            uiChannel.RemoveListener<SetGaugeActiveEvent>(HandleGaugeActiveEvent);
            uiChannel.RemoveListener<SetGaugeEvent>(HandleGaugeEvent);
        }
        private void HandleGaugeEvent(SetGaugeEvent @event)
        {
            text.text = @event.text;
            barImage.fillAmount = @event.fillAmount;
        }
    }
}
