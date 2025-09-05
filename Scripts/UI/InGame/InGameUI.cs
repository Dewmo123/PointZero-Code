using Core.EventSystem;
using Scripts.Core.EventSystem;
using System;
using UnityEngine;

namespace Scripts.UI.InGame
{
    public class InGameUI : ChangeableUI
    {
        [SerializeField] private EventChannelSO packetChannel;
        public override void Init(EventChannelSO channel)
        {
            base.Init(channel);
            packetChannel.AddListener<HandleLeaveRoom>(LeaveRoom);
        }

        private void LeaveRoom(HandleLeaveRoom room)
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.RoomList;
            uiChannel.InvokeEvent(evt);
        }
    }
}
