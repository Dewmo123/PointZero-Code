using Core.EventSystem;
using DG.Tweening;
using Scripts.Core.EventSystem;
using Scripts.Core.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scripts.UI.InGame
{
    public class RoundResultUI : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        [SerializeField] private TextMeshProUGUI text;
        private void Awake()
        {
            packetChannel.AddListener<HandleRoundEnd>(HandleRoundEndEvent);
            transform.localScale = new Vector3(0, 1, 1);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleRoundEnd>(HandleRoundEndEvent);
        }
        private async void HandleRoundEndEvent(HandleRoundEnd end)
        {
            if (end.isEnd)
                return;
            transform.DOScaleX(1, 0.5f);
            var player = PlayerManager.Instance.MyPlayer;
            Debug.Assert(player != null, "Player is null");
            text.text = player.MyTeam == end.winner ? "승리" : "패배";
            await Awaitable.WaitForSecondsAsync(3);
            transform.DOScaleX(0, 0.5f);
        }
    }
}
