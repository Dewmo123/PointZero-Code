using Core.EventSystem;
using DewmoLib.Dependencies;
using Scripts.Core.EventSystem;
using Scripts.Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI.Room
{
    public class RoomListUI : ChangeableUI
    {
        [SerializeField] private GameObject attribute;
        [SerializeField] private int countPerPage = 5;
        [SerializeField] private EventChannelSO packetChannel;
        [SerializeField] private Transform contentsParent;
        private List<RoomAttributeUI> _attributes;
        private List<RoomInfoPacket> _roomInfos;

        private int _currentCount = 0;
        private int _currentAttCount = 0;
        private int _currentRoomId;
        private void Awake()
        {
            _attributes = new List<RoomAttributeUI>();
            for (int i = 0; i < countPerPage; i++)
            {
                _attributes.Add(Instantiate(attribute, contentsParent).GetComponent<RoomAttributeUI>());
                int k = i;
                _attributes[i].GetComponent<Button>().onClick.AddListener(() => SetRoomId(_roomInfos[k].roomId));
                _attributes[i].gameObject.SetActive(false);
            }
            ResetAttributes();
            packetChannel.AddListener<HandleRoomList>(SetList);
        }
        public override void Open()
        {
            base.Open();
            packetChannel.AddListener<HandleEnterRoom>(HandleRoomEnter);
            Cursor.lockState = CursorLockMode.None;
            Reload();
        }
        public override void Close()
        {
            base.Close();
            packetChannel.RemoveListener<HandleEnterRoom>(HandleRoomEnter);
        }
        private void OnDestroy()
        {
            packetChannel.RemoveListener<HandleRoomList>(SetList);
            foreach (var item in _attributes)
                item.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        private void HandleRoomEnter(HandleEnterRoom room)
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.InGame;
            uiChannel.InvokeEvent(evt);
        }

        private void SetRoomId(int roomId)
        {
            _currentRoomId = roomId;
            Debug.Log(roomId);
        }
        public void ResetAttributes()
            => _attributes.ForEach(att => att.ClearUI());
        public void SetList(HandleRoomList roomInfos)
        {
            _roomInfos = roomInfos.packet.roomInfos;
            int cnt = Mathf.Clamp(_roomInfos.Count, 0, countPerPage);
            for (int i = 0; i < cnt; i++)
                _attributes[i].gameObject.SetActive(true);
            _currentCount = 0;
            SetNextAttributes();
        }
        public void SetNextAttributes()
        {
            ResetAttributes();
            int i;
            for (i = _currentCount; i < _currentCount + countPerPage; i++)
            {
                if (i >= _roomInfos.Count)
                    break;
                _attributes[i - _currentCount]
                    .SetText(_roomInfos[i].roomName, _roomInfos[i].currentCount, _roomInfos[i].maxCount);
            }
            if (i == _currentCount && _currentAttCount > 0)
            {
                _currentCount -= _currentAttCount;
                SetNextAttributes();
                return;
            }
            _currentAttCount = i;
            _currentCount += i;
        }
        public void SetPrevAttributes()
        {
            ResetAttributes();
            _currentCount -= (_currentAttCount + 5);
            if (_currentCount < 0)
                _currentCount = 0;
            SetNextAttributes();
        }
        public void EnterRoom()
        {
            C_RoomEnter roomEnter = new C_RoomEnter() { roomId = _currentRoomId };
            NetworkManager.Instance.SendPacket(roomEnter);
        }
        public void CreateRoom()
        {
            var evt = UIEvents.ChangeUIEvent;
            evt.type = UIType.CreateRoom;
            uiChannel.InvokeEvent(evt);
        }
        public void Reload()
        {
            C_RoomList roomList = new();
            NetworkManager.Instance.SendPacket(roomList);
        }
    }
}
