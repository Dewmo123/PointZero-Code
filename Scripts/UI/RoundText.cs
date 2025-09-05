using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class RoundText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI redText;
        [SerializeField] private TextMeshProUGUI BlueText;
        [SerializeField] private EventChannelSO packetChannel;
        private void Awake()
        {
            packetChannel.AddListener<HandleRoundEnd>(RoundEndHandler);
            packetChannel.AddListener<HandleLeaveRoom>(LeaveGameHandler);
        }

        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleRoundEnd>(RoundEndHandler);
            packetChannel.RemoveListener<HandleLeaveRoom>(LeaveGameHandler);
        }
        private void LeaveGameHandler(HandleLeaveRoom room)
        {
            redText.text = "0";
            BlueText.text = "0";
        }
        private void RoundEndHandler(HandleRoundEnd end)
        {
            redText.text = end.redCount.ToString();
            BlueText.text = end.blueCount.ToString();
        }
    }
}
