using Core.EventSystem;
using DewmoLib.Dependencies;
using Scripts.Core.EventSystem;
using Scripts.Network;
using System;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class SetNickNameUI : ChangeableUI
    {
        [SerializeField] private TMP_InputField nickNameField;
        [Inject] private PacketResponsePublisher _publisher = null;
        public override void Init(EventChannelSO channel)
        {
            base.Init(channel);
        }
        private void Start()
        {
            Debug.Assert(_publisher != null, "PacketResponsePublisher injection failed.");
            if (_publisher == null)
                return;

            _publisher.AddListener(PacketID.C_SetName, HandleResponse);
        }

        private void OnDestroy()
        {
            if (_publisher != null)
                _publisher.RemoveListener(PacketID.C_SetName, HandleResponse);
        }
        private void HandleResponse(bool obj)
        {
            if (obj)
            {
                var evt = UIEvents.ChangeUIEvent;
                evt.type = UIType.RoomList;
                uiChannel.InvokeEvent(evt);
            }
        }

        public void SendNickName()
        {
            C_SetName name = new();
            name.nickName = nickNameField.text;
            NetworkManager.Instance.SendPacket(name);

        }
    }
}
