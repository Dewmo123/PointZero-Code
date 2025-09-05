using Server.Objects;
using Server.Utiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Server.Rooms.States
{
    class LobbyState : SyncObjectsState
    {
        private Random _rand;
        private List<TeamInfoPacket> infos;

        public LobbyState(GameRoom room) : base(room)
        {
            _rand = new Random();
            infos = new List<TeamInfoPacket>();
        }

        public override void Enter()
        {
            base.Enter();
            infos.Clear();
        }
        public override void Update()
        {
            base.Update();
            if (!_room.CanAddPlayer)
                _room.ChangeState(RoomState.Prepare);
        }
        public override void Exit()
        {
            base.Exit();
            ushort[] teams = GetRamdomTeams();
            var keys = _room.GetSessionKeys();
            for (int i = 0; i < _room.SessionCount; i++)
            {
                ClientSession session = _room.GetSession(keys[i]);
                Player player = _room.GetObject<Player>(session.PlayerId);
                infos.Add(new TeamInfoPacket()
                {
                    index = player.index,
                    team = teams[i]
                });
                player.team = (Team)teams[i];
                Console.WriteLine($"index:{keys[i]}, Team:{(Team)teams[i]}");
            }
            _room.Broadcast(new S_TeamInfos() { teamInfos = infos });
            _room.GameStart();
        }

        private ushort[] GetRamdomTeams()
        {
            ushort[] arr = new ushort[_room.SessionCount];
            if (arr.Length % 2 == 1)
                throw new NullReferenceException();
            for (int i = 1; i <= arr.Length / 2; i++)
            {
                arr[i - 1] = (ushort)Team.Blue;
                arr[arr.Length - i] = (ushort)Team.Red;
            }
            for (int i = 0; i < 100; i++)
            {
                int idx = _rand.Next(1, arr.Length);
                (arr[0], arr[idx]) = (arr[idx], arr[0]);
            }

            return arr;
        }
    }
}
