using DewmoLib.Dependencies;
using Scripts.Core.EventSystem;
using Scripts.Network;
using TMPro;
using UnityEngine;

namespace Scripts.UI.Room
{
    public static class DropdownExtension
    {
        public static string GetValue(this TMP_Dropdown dropdown)
        {
            int index = dropdown.value;
            return dropdown.options[index].text;
        }
    }
    public class CreateRoomUI : ChangeableUI
    {
        [SerializeField] private TMP_InputField setRoomName;
        [SerializeField] private TMP_Dropdown setPlayerCount;
        [SerializeField] private TMP_Dropdown setRoundCount;
        [SerializeField] private TMP_Dropdown setPlayTime;
        [SerializeField] private TMP_Dropdown setBombTime;
        [Inject] private PacketResponsePublisher _publisher;
        public override void Open()
        {
            base.Open();
            _publisher.AddListener(PacketID.C_CreateRoom, RoomEnterHandler);
        }
        public override void Close()
        {
            base.Close();
            _publisher.RemoveListener(PacketID.C_CreateRoom, RoomEnterHandler);
        }
        private void RoomEnterHandler(bool obj)
        {
            if (obj)
            {
                var evt = UIEvents.ChangeUIEvent;
                evt.type = UIType.InGame;
                uiChannel.InvokeEvent(evt);
            }
        }

        public void Apply()
        {
            C_CreateRoom createRoom = new()
            {
                roomName = setRoomName.text,
                bombTime = ushort.Parse(setBombTime.GetValue()),
                playerCount = ushort.Parse(setPlayerCount.GetValue()),
                playTime = ushort.Parse(setPlayTime.GetValue()),
                roundCount = ushort.Parse(setRoundCount.GetValue())
            };
            NetworkManager.Instance.SendPacket(createRoom);
        }
        public void Cancel()
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.RoomList;
            uiChannel.InvokeEvent(evt);
        }
    }
}
