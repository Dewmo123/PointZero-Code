using Scripts.Network;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Room
{
    public class RoomAttributeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image outline;
        private Color _defaultBack;
        private void Awake()
        {
            _defaultBack = backgroundImage.color;
        }
        public void SetText(string roomName, int currentCount,int maxCount)
        {
            roomNameText.text = roomName;
            playerCountText.text = $"{currentCount} / {maxCount}";
            backgroundImage.color = _defaultBack;
            outline.color = Color.black;
        }
        public void ClearUI()
        {
            roomNameText.text = string.Empty;
            playerCountText.text = string.Empty;
            backgroundImage.color = Color.clear;
            outline.color = Color.clear;
        }
    }
}
