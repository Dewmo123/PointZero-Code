using Scripts.Entities.Players;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Core.Managers
{
    [DefaultExecutionOrder(-10)]
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance => _instance;
        public int MyIndex { get; private set; } = 0;
        public Player MyPlayer => GetPlayerById(MyIndex);
        [SerializeField] private GameObject myPlayer;
        [SerializeField] private GameObject otherPlayer;
        private Dictionary<int, Player> _players;
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);
            _players = new Dictionary<int, Player>();
        }
        public void FirstInitPlayer(S_EnterRoomFirst packet)
        {
            MyIndex = packet.myIndex;
            Debug.Log(MyIndex);
            foreach (var info in packet.playerLocations)
            {
                if (info.index == MyIndex)
                    InitMyPlayer(info);
                else
                    InitOtherPlayer(info);
            }
            foreach (var info in packet.playerNames)
            {
                SetPlayerName(info);
            }
        }

        public void SetPlayerName(PlayerNamePacket info)
        {
            var player = GetPlayerById(info.index);
            player.Name.Value = info.nickName;
        }

        public void InitOtherPlayer(LocationInfoPacket packet)
        {
            Debug.Log($"InitOtherPlayer: {packet.index}");
            if (MyIndex == 0 || MyIndex == packet.index)
                return;
            Player player = ObjectManager.Instance.CreateObject<Player>(packet.index, otherPlayer, packet.position.ToVector3(), packet.rotation.ToQuaternion());
            player.Init(packet, false);
            _players.Add(packet.index, player);
        }
        public void InitMyPlayer(LocationInfoPacket packet)
        {
            Debug.Log($"InitMyPlayer: {packet.index}");
            Player player = ObjectManager.Instance.CreateObject<Player>(packet.index, myPlayer, packet.position.ToVector3(), packet.rotation.ToQuaternion());
            player.Init(packet, true);
            _players.Add(packet.index, player);
            player.GetCompo<PlayerMovement>(true).SetPosition(packet.position.ToVector3());
        }
        public void ExitOtherPlayer(int sessionId)
        {
            ObjectManager.Instance.RemoveObject(sessionId);
            _players.Remove(sessionId);
        }
        public void DestroyAll()
        {
            foreach (var item in _players)
                ObjectManager.Instance.RemoveObject(item.Key);
            _players.Clear();
        }
        public Player GetPlayerById(int index)
        {
            var player = _players.GetValueOrDefault(index);
            Debug.Assert(player != null, $"{index} is not contained");
            return player;
        }
        public IEnumerable<Player> GetPlayers()
            => _players.Values;
    }
}