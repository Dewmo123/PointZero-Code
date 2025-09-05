using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI.InGame
{
    public class StateText : MonoBehaviour
    {
        [SerializeField] private EventChannelSO packetChannel;
        [SerializeField] private TextMeshProUGUI text;
        private void Awake()
        {
            packetChannel.AddListener<HandleUpdateRoomState>(HandleRoomState);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleUpdateRoomState>(HandleRoomState);
        }
        private void HandleRoomState(HandleUpdateRoomState state)
        {
            switch (state.state)
            {
                case Network.RoomState.Lobby:
                    text.text = "플레이어를 기다리는 중입니다";
                    break;
                case Network.RoomState.Prepare:
                    text.text = "준비 시간";
                    break;
                case Network.RoomState.InGame:
                    text.text = "라운드 시작";
                    break;
                case Network.RoomState.Between:
                    text.text = "다음 라운드 대기중";
                    break;
                case Network.RoomState.Bomb:
                    text.text = "폭탄이 설치되었습니다.";
                    break;
                case Network.RoomState.GameEnd:
                    text.text = "게임 종료";
                    break;
            }
        }
    }
}