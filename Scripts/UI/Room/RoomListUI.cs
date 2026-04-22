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
        private int[] _visibleRoomIds;

        private int _pageStartIndex = 0;
        private int _currentRoomId;
        private void Awake()
        {
            _attributes = new List<RoomAttributeUI>();
            _visibleRoomIds = new int[countPerPage];
            for (int i = 0; i < countPerPage; i++)
            {
                _attributes.Add(Instantiate(attribute, contentsParent).GetComponent<RoomAttributeUI>());
                int slot = i;
                _attributes[i].GetComponent<Button>().onClick.AddListener(() => HandleAttributeClick(slot));
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
        private void HandleAttributeClick(int slot)
        {
            if (_roomInfos == null || slot < 0 || slot >= _visibleRoomIds.Length)
                return;

            int roomId = _visibleRoomIds[slot];
            if (roomId == 0)
                return;

            SetRoomId(roomId);
        }
        public void ResetAttributes()
        {
            _attributes.ForEach(att =>
            {
                att.ClearUI();
                att.gameObject.SetActive(false);
            });

            for (int i = 0; i < _visibleRoomIds.Length; i++)
                _visibleRoomIds[i] = 0;
        }
        public void SetList(HandleRoomList roomInfos)
        {
            _roomInfos = roomInfos.packet.roomInfos ?? new List<RoomInfoPacket>();
            _pageStartIndex = 0;
            RefreshPage();
        }

        private void RefreshPage()
        {
            ResetAttributes();
            if (_roomInfos == null || _roomInfos.Count == 0)
                return;

            int visibleCount = Mathf.Min(countPerPage, _roomInfos.Count - _pageStartIndex);
            for (int i = 0; i < visibleCount; i++)
            {
                RoomInfoPacket info = _roomInfos[_pageStartIndex + i];
                _visibleRoomIds[i] = info.roomId;
                _attributes[i].gameObject.SetActive(true);
                _attributes[i].SetText(info.roomName, info.currentCount, info.maxCount);
            }
        }
        public void SetNextAttributes()
        {
            if (_roomInfos == null || _roomInfos.Count == 0)
                return;
            if (_pageStartIndex + countPerPage >= _roomInfos.Count)
                return;

            _pageStartIndex += countPerPage;
            RefreshPage();
        }
        public void SetPrevAttributes()
        {
            if (_roomInfos == null || _roomInfos.Count == 0)
                return;

            _pageStartIndex = Mathf.Max(0, _pageStartIndex - countPerPage);
            RefreshPage();
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
